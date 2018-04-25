using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : OverridableMonoBehaviour {
    public enum EEnemyState
    {
        IDLE,
        MOVING
    }

    public enum EEnemyAttackState
    {
        IDLE,
        COOLING_DOWN
    }
    public float AttackDamage = 1.0f;
    public float AttackRange = 10.0f;
    public float MoveSpeed = 5.0f;
    public float MaxHitPoint = 100.0f;
    public float AttackCoolDownSeconds = 5.0f;

    private float currentHitPoint;
    private Vector3 targetPosition;
    private EEnemyState enemyState;
    private EEnemyAttackState enemyAttackState;
    private BuildingBase attackingTarget;
    private float startTakingDamageTime;
    private float attackCoolDownStartTime;

    private Animator animator;
    private string ANIMATION_IS_IDLE = "isIdle";
    private string ANIMATION_IS_UNDER_STTACK = "isUnderAttack";
    private string ANIMATION_IS_ATTACKING = "isAttacking";
    private string ANIMATION_IS_DEAD = "isDead";

    public void Initialize(int currentWave, float enemyHPIncreasePercentage, Vector3 targetPos) {
        MaxHitPoint = MaxHitPoint * Mathf.Pow(enemyHPIncreasePercentage, currentWave);
        targetPosition = targetPos;
        enemyState = EEnemyState.IDLE;
        enemyAttackState = EEnemyAttackState.IDLE;
        currentHitPoint = MaxHitPoint;
        startTakingDamageTime = 0.0f;
        attackCoolDownStartTime = 0.0f;

        animator = GetComponent<Animator>();
        SetIdleAnimationState();
    }

    private void SetIdleAnimationState() {
        animator.SetBool(ANIMATION_IS_IDLE, true);
        animator.SetBool(ANIMATION_IS_UNDER_STTACK, false);
        animator.SetBool(ANIMATION_IS_ATTACKING, false);
        animator.SetBool(ANIMATION_IS_DEAD, false);
    }

    public override void UpdateMe() {
        if (TryAttackBuilding()) {
            return;
        }

        UpdateAttackCoolDown();
        TryFindBuildingToAttack();
        MoveTowardsTarget();
    }

    private void MoveTowardsTarget() {
        float step = MoveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

        animator.SetBool(ANIMATION_IS_IDLE, false);
        animator.SetBool(ANIMATION_IS_UNDER_STTACK, false);
        animator.SetBool(ANIMATION_IS_ATTACKING, false);
        animator.SetBool(ANIMATION_IS_DEAD, false);
    }

    private void TryFindBuildingToAttack() {
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

        if (enemyAttackState == EEnemyAttackState.COOLING_DOWN) {
            return false;
        }

        if (Vector3.Distance(attackingTarget.transform.position, transform.position) > AttackRange) {
            return false;
        }

        return true;
    }

    private bool TryAttackBuilding() {
        if (!CanAttckCurrentTarget()) {
            attackingTarget = null;
            SetIdleAnimationState();
            return false;
        }

        attackingTarget.TakeDamage(AttackDamage);
        attackCoolDownStartTime = Time.time;

        animator.SetBool(ANIMATION_IS_ATTACKING, true);

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
            enemyAttackState = EEnemyAttackState.IDLE;
        }
    }

    private BuildingBase GetHighestAttackPriorityBuildingsWithinRange() {
        BuildingBase highestAttackPriorityBuilding = null;
        foreach (var item in MapManager.Instance.GetCollectionOf<BuildingBase>()) {
            if (Vector3.Distance(item.transform.position, transform.position) > AttackRange) {
                continue;
            }

            if (highestAttackPriorityBuilding == null || highestAttackPriorityBuilding.AttackingPriority < item.AttackingPriority) {
                highestAttackPriorityBuilding = item;
            }
        }

        return highestAttackPriorityBuilding;
    }

    public void TakeDamage(float damage)
    {
        animator.SetBool(ANIMATION_IS_UNDER_STTACK, true);
        currentHitPoint -= damage;
        if (currentHitPoint <= 0.0f)
        {
            animator.SetBool(ANIMATION_IS_DEAD, true);
            MapManager.Instance.RemoveItemFromMap(this.gameObject);
            Destroy(this.gameObject, 2.0f);
        }
    }

    private void TryRecoverStateFromTakingDamage()
    {
        if (enemyState == EEnemyState.IDLE)
        {
            return;
        }

        if (Time.time - startTakingDamageTime >= AppConstant.Instance.buildingDamageMovingFreezeTime)
        {
            startTakingDamageTime = 0.0f;
            enemyState = EEnemyState.IDLE;
            SetIdleAnimationState();
        }
    }
}
