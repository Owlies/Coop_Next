using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProgressBar;

public class EnemyBase : OverridableMonoBehaviour {
    public enum EEnemyState
    {
        IDLE,
        MOVING,
        ATTACKING
    }

    public enum EEnemyAttackState
    {
        IDLE,
        COOLING_DOWN
    }
    public float AttackDamage = 30.0f;
    public float AttackRange = 100.0f;
    public float MoveSpeed = 5.0f;
    public float MaxHitPoint = 100.0f;
    public float AttackCoolDownSeconds = 5.0f;
    public float SearchRange = 15.0f;

    private float currentHitPoint;
    private List<GameObject> targetGameOjbects;
    private EEnemyState enemyState;
    private EEnemyAttackState enemyAttackState;
    private BuildingBase attackingTarget;
    private float attackCoolDownStartTime;
    private EnemyTypeEnum type;
    

    private Animator animator;
    private string ANIMATION_IS_IDLE = "isIdle";
    private string ANIMATION_IS_ATTACKING = "isAttacking";
    private string ANIMATION_IS_DEAD = "isDead";

    private ProgressBarBehaviour hpProgressBar;

    public void Initialize(int currentWave, EnemyMetadataDBObject config, GameObject targetGameObject) {
        targetGameOjbects = new List<GameObject>();
        targetGameOjbects.Add(targetGameObject);

        MaxHitPoint = config.hp;
        AttackDamage = config.attack;
        AttackRange = config.attackRange;
        type = config.enemyType;
        AttackCoolDownSeconds = 1.0f / config.attackFrequency;
        MoveSpeed = config.moveSpeed;
        SearchRange = config.searchRange;

        enemyState = EEnemyState.IDLE;
        enemyAttackState = EEnemyAttackState.IDLE;
        currentHitPoint = MaxHitPoint;
        attackCoolDownStartTime = 0.0f;
        
        hpProgressBar = GetComponentInChildren<ProgressBarBehaviour>();
        hpProgressBar.Value = 100.0f;
        hpProgressBar.TransitoryValue = 0.0f;
        hpProgressBar.ProgressSpeed = 1000;

        animator = GetComponent<Animator>();
        SetStateToIdle();

        // ANIMATION_IS_DEAD should only be set to false at the beginning
        animator.SetBool(ANIMATION_IS_DEAD, false);
    }

    private void SetStateToIdle() {
        enemyState = EEnemyState.IDLE;
        enemyAttackState = EEnemyAttackState.IDLE;
        SetIdelAnimationState();
    }

    private void SetIdelAnimationState()
    {
        animator.SetBool(ANIMATION_IS_IDLE, true);
        animator.SetBool(ANIMATION_IS_ATTACKING, false);
    }

    public override void UpdateMe() {
        if (TryAttackBuilding()) {
            return;
        }

        TryFindBuildingToAttack();
        UpdateAttackCoolDown();
        MoveTowardsTarget();
        UpdateMovingTargets();
        UpdateHPBar();
    }

    /* Private Methods */
    private void UpdateHPBar() {
        float value = 100.0f * (currentHitPoint / MaxHitPoint);
        hpProgressBar.Value = value;
    }
    private void UpdateMovingTargets() {
        List<GameObject> newTargetList = new List<GameObject>();
        newTargetList.Add(targetGameOjbects[0]);
        for(int i = 1; i < targetGameOjbects.Count; i++) {
            if(targetGameOjbects[i] != null) {
                newTargetList.Add(targetGameOjbects[i]);
            }
        }

        targetGameOjbects = newTargetList;
    }
    private Vector3 GetCurrentMovingTargetPosition() {
        for(int i = targetGameOjbects.Count - 1; i >= 0; i--) {
            if (targetGameOjbects[i] == null) {
                continue;
            }

            return targetGameOjbects[i].transform.position;
        }

        return targetGameOjbects[0].transform.position;
    }
    private bool CanMoveTowardsTarget() {
        if (IsDead()) {
            return false;
        }

        if ((enemyState != EEnemyState.IDLE) && (enemyState != EEnemyState.MOVING)) {
            return false;
        }

        if (attackingTarget != null && Vector3.Distance(attackingTarget.transform.position, transform.position) <= AttackRange) {
            return false;
        }

        return true;
    }

    private void MoveTowardsTarget() {
        if (!CanMoveTowardsTarget()) {
            return;
        }

        float step = MoveSpeed * Time.deltaTime;

        Vector3 targetPos = GetCurrentMovingTargetPosition();
        transform.position = Vector3.MoveTowards(transform.position, targetPos, step);
        transform.LookAt(targetPos);

        enemyState = EEnemyState.MOVING;

        animator.SetBool(ANIMATION_IS_IDLE, false);
    }

    private void TryFindBuildingToAttack() {
        if (attackingTarget != null) {
            return;
        }

        BuildingBase buildingToAttack = GetHighestAttackPriorityBuildingsWithinRange();
        if (buildingToAttack == null) {
            return;
        }

        attackingTarget = buildingToAttack;
        targetGameOjbects.Add(attackingTarget.gameObject);
    }

    private bool CanAttckCurrentTarget() {
        if (attackingTarget == null) {
            return false;
        }

        if (IsDead()) {
            return false;
        }

        if (enemyAttackState == EEnemyAttackState.COOLING_DOWN) {
            return false;
        }

        if (enemyState == EEnemyState.ATTACKING) {
            return false;
        }

        if (Vector3.Distance(attackingTarget.transform.position, transform.position) > AttackRange) {
            return false;
        }

        return true;
    }

    private bool TryAttackBuilding() {
        if (!CanAttckCurrentTarget()) {
            animator.SetBool(ANIMATION_IS_IDLE, true);
            return false;
        }

        attackingTarget.TakeDamage(AttackDamage);
        attackCoolDownStartTime = Time.time;

        animator.SetBool(ANIMATION_IS_ATTACKING, true);

        enemyAttackState = EEnemyAttackState.COOLING_DOWN;
        enemyState = EEnemyState.ATTACKING;
        transform.LookAt(attackingTarget.transform);

        return true;
    }

    private void UpdateAttackCoolDown()
    {
        if (enemyAttackState != EEnemyAttackState.COOLING_DOWN)
        {
            return;
        }

        if (Time.time - attackCoolDownStartTime >= AttackCoolDownSeconds)
        {
            SetStateToIdle();
        }
    }

    private BuildingBase GetHighestAttackPriorityBuildingsWithinRange() {
        BuildingBase highestAttackPriorityBuilding = null;
        float minDistance = int.MaxValue;
        foreach (var item in MapManager.Instance.GetCollectionOfItemsOnMap<BuildingBase>()) {
            if (item.gameObject.tag != "Building" || Vector3.Distance(item.transform.position, transform.position) > SearchRange) {
                continue;
            }

            if (highestAttackPriorityBuilding == null || highestAttackPriorityBuilding.AttackingPriority < item.AttackingPriority) {
                highestAttackPriorityBuilding = item;
                continue;
            }

            if (highestAttackPriorityBuilding.AttackingPriority == item.AttackingPriority) {
                float dis = Vector3.Distance(transform.position, item.transform.position);
                if (minDistance > dis) {
                    highestAttackPriorityBuilding = item;
                    minDistance = dis;
                }
            }
        }

        return highestAttackPriorityBuilding;
    }

    public float GetCurrentHitPoint() {
        return currentHitPoint;
    }

    public void TakeDamage(float damage)
    {
        currentHitPoint -= damage;
        if (currentHitPoint <= Constants.EPS)
        {
            animator.SetBool(ANIMATION_IS_DEAD, true);
            MapManager.Instance.RemoveItemFromMap(this.gameObject);
            EnemyManager.Instance.OnEnemyKilled(this);
        }
    }

    public bool IsDead() {
        return currentHitPoint <= Constants.EPS;
    }
}
