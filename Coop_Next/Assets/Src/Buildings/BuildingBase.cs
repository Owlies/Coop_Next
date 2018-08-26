using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProgressBar;

public enum ERarity
{
    COMMON,
    UNCOMMON,
    RARE,
    EPIC,
    LEGENDARY
}

public class BuildingBase : InteractiveObject {
    public enum EBuildingState
    {
        IDLE,
        TAKING_DAMAGE
    }

    public float maxHitPoint = 100.0f;
    public float coolDownFactor = 1.0f;
    [HideInInspector]
    public float coolDownFactorModifier = 0.0f;

    public int underAttackingPriority = 1;

    private float currentHitPoint;
    protected EBuildingState buildingState;
    private float startTakingDamageTime;

    private HpBarBehaviour hpBarBehaviour;

    public override void Init() {
        base.Init();
        InitializeWithBuildingConfig();

        hpBarBehaviour = GetComponentInChildren<HpBarBehaviour>();
    }

    public float GetHitPointPercentage()
    {
        return maxHitPoint == 0 ? 0 : currentHitPoint / maxHitPoint;
    }

    protected virtual void InitializeWithBuildingConfig() {
        BuildingMetadata metadata = objectMetadata as BuildingMetadata;
        if (metadata == null) {
            return;
        }
        maxHitPoint = metadata.hp;
        underAttackingPriority = metadata.underAttackPriority;
        name = metadata.objectName;

        currentHitPoint = maxHitPoint;
        buildingState = EBuildingState.IDLE;
        startTakingDamageTime = 0.0f;
    }

    public override void ClearModifler()
    {
        base.ClearModifler();
        coolDownFactorModifier = 0;
    }

    public override void UpdateMe()
    {
        base.UpdateMe();
        TryRecoverStateFromTakingDamage();
    }

    #region LongPressAction
    private bool CanMoveBuilding(Player actor)
    {
        if (buildingState != EBuildingState.IDLE) {
            return false;
        }

        if (actor.GetPlayerActionState() != EPlayerActionState.IDLE)
        {
            return false;
        }

        if (this.tag != "Building") {
            return false;
        }

        return true;
    }

    private bool TryHandleMoveBuildingAction(Player actor)
    {
        if (!CanMoveBuilding(actor))
        {
            return false;
        }

        actor.SetCarryingItem(this);
        // Somehow changing parent will change hitObject.transform.gameObject to points to the parent
        return EventCenter.Instance.ExecuteEvent(new MoveBuildingEvent(this.gameObject, this.transform.gameObject));
    }

    public override bool LongPressAction(Player actor)
    {
        return TryHandleMoveBuildingAction(actor);
    }
    #endregion

    #region OtherFunctions
    public void TakeDamage(float damage) {
        currentHitPoint -= damage;
        startTakingDamageTime = Time.time;
        buildingState = EBuildingState.TAKING_DAMAGE;

        if (hpBarBehaviour != null) {
            hpBarBehaviour.UpdateHpBar(currentHitPoint, maxHitPoint);
        }
        
        if (currentHitPoint <= Constants.EPS) {
            MapManager.Instance.OnItemDestroyed(this.gameObject);
            Destroy(this.gameObject);
        }
    }

    public void AddHitPoint(float value)
    {
        currentHitPoint = Mathf.Min(maxHitPoint, currentHitPoint + value);
        if (hpBarBehaviour != null)
        {
            hpBarBehaviour.UpdateHpBar(currentHitPoint, maxHitPoint);
        }
    }

    private void TryRecoverStateFromTakingDamage() {
        if (buildingState == EBuildingState.IDLE) {
            return;
        }

        if (Time.time - startTakingDamageTime >= AppConstant.Instance.buildingDamageMovingFreezeTime) {
            startTakingDamageTime = 0.0f;
            buildingState = EBuildingState.IDLE;
        }
    }
    #endregion
}
