using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBuilding : BuildingBase {
    public enum EAttackBuildingState
    {
        IDLE,
        COOLING_DOWN
    }

    public float attackDamage = 2.0f;
    public float attackRange = 5.0f;
    public float attackCoolDownSeconds = 1.0f;

    public GameObject bulletPrefab;

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
            if (Vector3.Distance(enemy.transform.position, this.transform.position) <= attackRange) {
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

        if (Vector3.Distance(attackingEnemy.transform.position, transform.position) > attackRange)
        {
            return false;
        }

        return true;
    }

    private bool TryAttackEnemy() {
        if (!CanAttackEnemy()) {
            attackingEnemy = null;
            return false;
        }

        GameObject bullet = GameObject.Instantiate(bulletPrefab);
        bullet.GetComponent<Bullet>().Initialize(attackingEnemy.gameObject, attackDamage);
        
        attackState = EAttackBuildingState.COOLING_DOWN;
        attackCoolDownStartTime = Time.time;

        return true;
    }

    private void UpdateAttackCoolDown() {
        if (attackState != EAttackBuildingState.COOLING_DOWN) {
            return;
        }

        if (Time.time - attackCoolDownStartTime >= attackCoolDownSeconds) {
            attackState = EAttackBuildingState.IDLE;
        }
    }

}
