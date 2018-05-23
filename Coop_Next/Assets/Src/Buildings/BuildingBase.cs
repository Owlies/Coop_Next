using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public float MaxHitPoint = 100.0f;
    public float coolDownFactor = 1.0f;
    public int AttackingPriority = 1;

    private float currentHitPoint;
    protected EBuildingState buildingState;
    private float startTakingDamageTime;

    public TimingCallbacks callbacks;

    public void Start() {
        currentHitPoint = MaxHitPoint;
        buildingState = EBuildingState.IDLE;
        startTakingDamageTime = 0.0f;
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
        if (currentHitPoint <= Constants.EPS) {
            MapManager.Instance.OnItemDestroyed(this.gameObject);
            Destroy(this.gameObject);
            Debug.Log("Building Destroyed");
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
