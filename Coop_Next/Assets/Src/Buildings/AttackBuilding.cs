using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBuilding : BuildingBase {
    public enum EAttackBuildingState
    {
        IDLE,
        COOLING_DOWN
    }

    public float attackDamage = 20.0f;
    public float attackDamageModifier = 0.0f;
    public float GetAttackDamage()
    {
        UpdateBuff();
        return attackDamage + attackDamageModifier;
    }

    public float attackRange = 5.0f;
    public float attackRangeModifier = 0.0f;
    public float GetAttackRange()
    {
        UpdateBuff();
        return attackRange + attackRangeModifier;
    }

    public float attackCoolDownSeconds = 1.0f;
    public float attackCoolDownSecondsModifier = 0.0f;
    public float GetAttackCoolDownSeconds()
    {
        UpdateBuff();
        return attackCoolDownSeconds + attackCoolDownSecondsModifier;
    }

    public GameObject bulletPrefab;
    // units / seconds, attackRange/bulletSpeed > attackCoolDownSeconds
    public float bulletSpeed = 10.0f;

    private EnemyBase attackingEnemy;
    private float attackCoolDownStartTime;
    private EAttackBuildingState attackState;
    private Vector3 firingPosition;

    protected override void InitializeWithBuildingConfig() {
        base.InitializeWithBuildingConfig();
        BuildingMetadata metadata = objectMetadata as BuildingMetadata;
        if (metadata == null)
            return;
        attackDamage = metadata.attack;
        attackCoolDownSeconds = 1.0f / metadata.attackFrequency;
        attackRange = metadata.attackRange;
    }

    public override void ClearModifler()
    {
        base.ClearModifler();
        attackDamageModifier = 0;
        attackRangeModifier = 0;
        attackCoolDownSecondsModifier = 0;
    }

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
        if (attackingEnemy != null) {
            return;
        }
        
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

        float currentHP = attackingEnemy.GetComponent<EnemyBase>().GetCurrentHitPoint();

        if (currentHP <= Constants.EPS) {
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
        firingPosition = new Vector3(transform.position.x, transform.position.y + (float)(GetComponent<BoxCollider>().size.y * 0.8), transform.position.z);
        GameObject bullet = GameObject.Instantiate(bulletPrefab, firingPosition, Quaternion.LookRotation(attackingEnemy.gameObject.transform.position));
        bullet.GetComponent<Bullet>().Initialize(attackingEnemy.gameObject, bulletSpeed, GetAttackDamage());
        Destroy(bullet, 30.0f);
        
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
