using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameThrower : AttackBuilding {

    protected override bool TryAttackEnemy()
    {
        if (!CanAttackEnemy())
        {
            attackingEnemy = null;
            return false;
        }

        foreach (EnemyBase enemy in EnemyManager.Instance.GetAllAliveEnemies())
        {
            if (Vector3.Distance(enemy.transform.position, this.transform.position) <= attackRange)
            {
                enemy.TakeDamage(attackDamage);
            }
        }

        attackState = EAttackBuildingState.COOLING_DOWN;
        attackCoolDownStartTime = Time.time;

        return true;
    }

    protected override bool CanAttackEnemy()
    {
        if (attackState == EAttackBuildingState.COOLING_DOWN)
        {
            return false;
        }

        return true;
    }
}
