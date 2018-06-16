using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveObject : OverridableMonoBehaviour {
    public int itemId;
    public int maxAllowed = 99;
    public ERarity rarity = ERarity.COMMON;
    public string techTreeId = "";
    public BuffCollection buffs = new BuffCollection();

    public Vector2Int size = new Vector2Int(1, 1);

    public virtual bool LongPressAction(Player actor) { return false;}
    public virtual bool PressReleaseAction(Player actor) { return false; }

    public Sprite iconImage;

    public ObjectDir GetItemDirection() {
        if (transform.rotation.eulerAngles.y.Equals(0.0f) || transform.rotation.eulerAngles.y.Equals(180.0f)) {
            return ObjectDir.Horizontal;
        } else {
            return ObjectDir.Vertical;
        }
    }

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

        if (!MapManager.Instance.CanPlaceItemOnMap(actor.GetCarryingItem()))
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

        Vector2Int itemPos = MapManager.Instance.GeItemMapPosition(this);
        ObjectDir itemDirection = GetItemDirection();

        MapManager.Instance.PlaceItemOnMap(this, itemPos, itemDirection);
        
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
