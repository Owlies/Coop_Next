using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveObject : OverridableMonoBehaviour {

    public Vector2Int posOnMap = new Vector2Int(-1, -1);

    public ObjectMetadata objectMetadata;
    public int itemId
    {
        get { return objectMetadata.objectId; }
    }
    public int maxAllowed = 99;
    public ERarity rarity = ERarity.COMMON;
    public string techTreeId
    {
        get { return objectMetadata.techTreeId; }
    }
    public BuffCollection buffs = new BuffCollection();

    public TimingCallbacks callbacks = new TimingCallbacks();

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

    public virtual void Init()
    {

    }

    public void AddBuff(Buff buff)
    {
        buffs.AddBuff(this, buff);
    }

    public void RemoveBuff(Buff buff)
    {
        buffs.RemoveBuff(this, buff);
    }

    public void UpdateBuff()
    {
        buffs.Tick(this);
    }

    public virtual void ClearModifler()
    {
    }

    public override void UpdateMe()
    {
        base.UpdateMe();
        ClearModifler();
        UpdateBuff();
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

        if (callbacks.OnPickedUpFromMap != null)
            callbacks.OnPickedUpFromMap();

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

        actor.UnsetCarryingItem();
        actor.SetPlayerActionState(EPlayerActionState.IDLE);

        MapManager.Instance.PlaceItemOnMap(this, itemPos, itemDirection);
        

        if (callbacks.OnPlacedOnMap != null)
            callbacks.OnPlacedOnMap();

        return true;
    }

    public virtual bool ShortPressAction(Player actor)
    {
        return TryPlaceItemOnMap(actor);
    } 

    public virtual bool InteractAction(Player actor)
    {
        return false;
    }
    #endregion
}
