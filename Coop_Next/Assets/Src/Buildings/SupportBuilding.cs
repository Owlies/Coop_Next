using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupportBuilding : BuildingBase
{
    public enum ESupportBuildingState
    {
        IDLE,
        COOLING_DOWN
    }

    protected float range = 5.0f;
    protected float rangeModifier = 0.0f;
    public float GetRange()
    {
        UpdateBuff();
        return range + rangeModifier;
    }

    protected float strength = 5.0f;
    protected float strengthModifier = 0.0f;
    public float GetStrength()
    {
        UpdateBuff();
        return strength + strengthModifier;
    }

    protected float actionCoolDownSeconds = 1.0f;
    protected float actionCoolDownSecondsModifier = 0.0f;
    public float GetActionCoolDownSeconds()
    {
        UpdateBuff();
        return actionCoolDownSeconds + actionCoolDownSecondsModifier;
    }

    protected ESupportBuildingState actionState;
    protected float actionCoolDownStartTime;

    protected override void InitializeWithBuildingConfig()
    {
        base.InitializeWithBuildingConfig();
        BuildingMetadata metadata = objectMetadata as BuildingMetadata;
        if (metadata == null)
            return;
        range = metadata.attackRange;
        strength = metadata.attack;
        actionCoolDownSeconds = 1.0f / metadata.attackFrequency;
    }

    public override void ClearModifler()
    {
        base.ClearModifler();
        rangeModifier = 0;
        strengthModifier = 0;
    }

    public override void UpdateMe()
    {
        base.UpdateMe();
        DoAction();
        UpdateActionCoolDown();
    }

    public virtual bool DoAction()
    {
        foreach (var building in MapManager.Instance.GetCollectionOfItems<BuildingBase>())
        {
            if (Util.Get2DDistanceSquared(building.gameObject, this.gameObject) <= range)
            {
                building.AddBuff(new AtackDamageBuff());
            }
        }

        return true;
    }

    private void UpdateActionCoolDown()
    {
        if (actionState != ESupportBuildingState.COOLING_DOWN)
        {
            return;
        }

        if (Time.time - actionCoolDownStartTime >= actionCoolDownSeconds)
        {
            actionState = ESupportBuildingState.IDLE;
        }
    }
}
