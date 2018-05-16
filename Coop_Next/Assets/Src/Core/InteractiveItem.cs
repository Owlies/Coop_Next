using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveItem : OverridableMonoBehaviour {
    public int maxAllowed = 99;
    public ERarity Rarity = ERarity.COMMON;

    public Vector2Int size = new Vector2Int(1, 1);

    public virtual bool LongPressAction(Player actor) { return false;}
    public virtual bool PressReleaseAction(Player actor) { return false; }

    public Sprite iconImage;

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

        MapManager mapManager = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        if (!mapManager.CanPlaceItemOnMap(actor.GetCarryingItem(), actor.GetCarryingItemPosition(), actor.carryingItemDir))
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

        //if (actor.GetPlayerActionState() == EPlayerActionState.CARRYING_BUILDING)
        //{
        //    if (!EventCenter.Instance.ExecuteEvent(new PlaceBuildingEvent(this.gameObject, this.gameObject)))
        //    {
        //        return false;
        //    }
        //}

        Vector2Int carryingItemPos = actor.GetCarryingItemPosition();
        ObjectDir carryingItemDir = actor.carryingItemDir;

        actor.UnsetCarryingItem();
        actor.SetPlayerActionState(EPlayerActionState.IDLE);

        MapManager mapManager = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        mapManager.PlaceItemOnMap(this, carryingItemPos, carryingItemDir);

        return true;
    }

    public virtual bool ShortPressAction(Player actor)
    {
        return TryPlaceItemOnMap(actor);
    }
    #endregion
}
