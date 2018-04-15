using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBuilding : BuildingBase {
    public enum EAttackBuildingState
    {
        IDLE,
        COOLING_DOWN
    }

    public float AttackDamage = 2.0f;
    public float AttackRange = 5.0f;
    public float AttackCoolDownSeconds = 1.0f;

    private EnemyBase attackingEnemy;
    private float attackCoolDownStartTime;
    private EAttackBuildingState attackState;

    public override void UpdateMe()
    {
        base.UpdateMe();

        if (TryAttackEnemy()) {
            return;
        }

        UpdateAttackCoolDown();
        TryFindEnemyToAttack();
    }

    private void TryFindEnemyToAttack()
    {
        EnemyBase enemy = GetEnemyWithinRange();
        if (enemy == null)
        {
            return;
        }

        attackingEnemy = enemy;
    }

    private EnemyBase GetEnemyWithinRange() {
        foreach (EnemyBase enemy in EnemyManager.Instance.GetAllAliveEnemies()) {
            if (Vector3.Distance(enemy.transform.position, this.transform.position) <= AttackRange) {
                return enemy;
            }
        }

        return null;
    }

    private bool CanAttackEnemy() {
        if (attackingEnemy == null)
        {
            return false;
        }

        if (attackState == EAttackBuildingState.COOLING_DOWN) {
            return false;
        }

        if (Vector3.Distance(attackingEnemy.transform.position, transform.position) > AttackRange)
        {
            return false;
        }

        return true;
    }

    private bool TryAttackEnemy() {
        if (CanAttackEnemy()) {
            attackingEnemy = null;
            return false;
        }

        attackingEnemy.TakeDamage(AttackDamage);
        attackState = EAttackBuildingState.COOLING_DOWN;
        attackCoolDownStartTime = Time.time;

        return true;
    }

    private void UpdateAttackCoolDown() {
        if (attackState != EAttackBuildingState.COOLING_DOWN) {
            return;
        }

        if (Time.time - attackCoolDownStartTime >= AttackCoolDownSeconds) {
            attackState = EAttackBuildingState.IDLE;
        }
    }

}
