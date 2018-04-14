using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBase : InteractiveItem {
    public float MaxHitPoint = 100.0f;
    public int AttackingPriority = 1;

    private float currentHitPoint;

    public void Start() {
        currentHitPoint = MaxHitPoint;
    }

    #region LongPressAction
    private bool CanMoveBuilding(Player actor)
    {
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
        if (currentHitPoint <= 0.0f) {
            MapManager.Instance.RemoveItemFromMap(this.gameObject);
            Destroy(this.gameObject);
        }
    }
    #endregion
}
