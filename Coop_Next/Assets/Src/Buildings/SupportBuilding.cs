using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupportBuilding : BuildingBase
{
    public float attackRange = 5.0f;
    public float attackRangeModifier = 0.0f;
    public float GetAttackRange()
    {
        UpdateBuff();
        return attackRange + attackRangeModifier;
    }

    public new void Awake()
    {
        base.Awake();
        InitializeWithBuildingConfig();
    }

    private void InitializeWithBuildingConfig()
    {
        BuildingMetadata metadata = MetadataManager.Instance.GetBuildingMetadataWithTechTreeId(techTreeId);
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
