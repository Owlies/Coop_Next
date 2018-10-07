using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameThrower : AttackBuilding {

    public BoxCollider flameCollider;
    private float flameWidth;

    protected override void InitializeWithBuildingConfig()
    {
        base.InitializeWithBuildingConfig();
        BuildingMetadata metadata = objectMetadata as BuildingMetadata;
        flameWidth = metadata.GetFloatCustomValue("flameWidth");
        flameCollider.center = new Vector3(0, 3, attackRange / 2.0f);
        flameCollider.size = new Vector3(flameWidth / 2.0f, 8, attackRange);
    }

    public List<EnemyBase> attackingEnemys = new List<EnemyBase>();
    protected override bool TryAttackEnemy()
    {
        if (!CanAttackEnemy())
        {
            return false;
        }

        for(int i =0; i < attackingEnemys.Count; ++i)
            attackingEnemys[i].TakeDamage(attackDamage);

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

        if (attackingEnemys.Count == 0)
            return false;

        return true;
    }
}
