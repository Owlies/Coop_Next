using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupportBuilding : BuildingBase
{
    private float attackRange = 5.0f;
    private float attackRangeModifier = 0.0f;
    public float GetAttackRange()
    {
        UpdateBuff();
        return attackRange + attackRangeModifier;
    }

    private new void InitializeWithBuildingConfig()
    {
        base.InitializeWithBuildingConfig();
        BuildingMetadata metadata = objectMetadata as BuildingMetadata;
        if (metadata == null)
            return;
        attackRange = metadata.attackRange;
    }

    public override void ClearModifler()
    {
        base.ClearModifler();
        attackRangeModifier = 0;
    }

    public override void UpdateMe()
    {
        base.UpdateMe();
        foreach(var building in MapManager.Instance.GetCollectionOfItems<AttackBuilding>())
        {
            if (Util.Get2DDistanceSquared(building.gameObject, this.gameObject) <= attackRange)
            {
                building.AddBuff(new AtackDamageBuff());
            }
        }
    }
}
