using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private float currentHitPoint;
    private Vector3 targetPosition;
    private EEnemyState enemyState;
    private EEnemyAttackState enemyAttackState;
    private BuildingBase attackingTarget;
    private float attackCoolDownStartTime;
    private EnemyTypeEnum type;

    private Animator animator;
    private string ANIMATION_IS_IDLE = "isIdle";
    private string ANIMATION_IS_ATTACKING = "isAttacking";
    private string ANIMATION_IS_DEAD = "isDead";

    public void Initialize(int currentWave, float hp, int attack, int attackFrequenccy, float attackRange, float moveSpeed, EnemyTypeEnum enemyType, Vector3 targetPos) {
        MaxHitPoint = hp;
        targetPosition = targetPos;
        AttackDamage = attack;

        AttackRange = attackRange;

        type = enemyType;
        AttackCoolDownSeconds = 1.0f / attackFrequenccy;
        MoveSpeed = moveSpeed;

        enemyState = EEnemyState.IDLE;
        enemyAttackState = EEnemyAttackState.IDLE;
        currentHitPoint = MaxHitPoint;
        attackCoolDownStartTime = 0.0f;

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
    }

    /* Private Methods */
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
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
        transform.LookAt(targetPosition);

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
        foreach (var item in MapManager.Instance.GetCollectionOfItemsOnMap<BuildingBase>()) {
            if (Vector3.Distance(item.transform.position, transform.position) > AttackRange) {
                continue;
            }

            if (highestAttackPriorityBuilding == null || highestAttackPriorityBuilding.AttackingPriority < item.AttackingPriority) {
                highestAttackPriorityBuilding = item;
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
