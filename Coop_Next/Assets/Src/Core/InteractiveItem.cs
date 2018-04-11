using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveItem : OverridableMonoBehaviour {
    public virtual bool LongPressAction(Player actor) { return false;}
    public virtual bool PressReleaseAction(Player actor) { return false; }

    #region ShortPressAction
    private bool CanPlaceItemOnMap(Player actor)
    {
        if (actor.GetCarryingItem() == null)
        {
            return false;
        }

        if (!(actor.GetPlayerActionState() != EPlayerActionState.CARRYING_BUILDING ||
            actor.GetPlayerActionState() != EPlayerActionState.CARRYING_RESOURCE))
        {
            return false;
        }

        return true;
    }

    private bool TryPlaceItemOnMap(Player actor)
    {
        if (!CanPlaceItemOnMap(actor))
        {
            return false;
        }

        if (actor.GetPlayerActionState() == EPlayerActionState.CARRYING_BUILDING)
        {
            if (!EventCenter.Instance.ExecuteEvent(new PlaceBuildingEvent(this.gameObject, this.gameObject)))
            {
                return false;
            }
        }

        actor.UnsetCarryingItem();

        actor.SetPlayerActionState(EPlayerActionState.IDLE);

        return true;
    }

    public virtual bool ShortPressAction(Player actor)
    {
        return TryPlaceItemOnMap(actor);
    }
    #endregion
}
