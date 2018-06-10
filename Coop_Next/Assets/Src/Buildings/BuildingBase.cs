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

public class BuildingBase : InteractiveItem {
    public enum EBuildingState
    {
        IDLE,
        TAKING_DAMAGE
    }

    public float maxHitPoint = 100.0f;
    public float coolDownFactor = 1.0f;
    public int underAttackingPriority = 1;

    private float currentHitPoint;
    protected EBuildingState buildingState;
    private float startTakingDamageTime;

    private HpBarBehaviour hpBarBehaviour;

    public TimingCallbacks callbacks;

    public void Start() {
        InitializeWithBuildingConfig();

        currentHitPoint = maxHitPoint;
        buildingState = EBuildingState.IDLE;
        startTakingDamageTime = 0.0f;

        hpBarBehaviour = GetComponentInChildren<HpBarBehaviour>();
    }

    private void InitializeWithBuildingConfig() {
        BuildingMetadata metadata = MapManager.Instance.GetBuildingMetadataWithTechTreeId(techTreeId);
        if (metadata == null) {
            return;
        }
        maxHitPoint = metadata.hp;
        underAttackingPriority = metadata.underAttackPriority;
        name = metadata.objectName;
        itemId = metadata.objectId;
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
