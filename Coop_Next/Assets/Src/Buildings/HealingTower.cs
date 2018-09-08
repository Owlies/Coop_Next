using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingTower : SupportBuilding
{
    private bool CanHealOther()
    {
        if (actionState == ESupportBuildingState.COOLING_DOWN)
        {
            return false;
        }

        return true;
    }

    public override bool DoAction()
    {
        if (!CanHealOther())
            return false;

        float hpRate = 1.1f;
        BuildingBase lowestHealthBuilding = null;
        foreach (var building in MapManager.Instance.GetCollectionOfItems<BuildingBase>())
        {
            if (Util.Get2DDistanceSquared(building.gameObject, this.gameObject) <= range)
            {
                if (hpRate > building.GetHitPointPercentage())
                {
                    hpRate = building.GetHitPointPercentage();
                    lowestHealthBuilding = building;
                }
            }
        }
        if (lowestHealthBuilding != null)
        {
            lowestHealthBuilding.AddHitPoint(strength);
        }

        return true;
    }
}
