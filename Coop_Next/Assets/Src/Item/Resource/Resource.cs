﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProgressBar;

public enum ResourceEnum {None, Rock, Wood, Ore, Coal, ArcaneOrb, NatureOrb, StrengthOrb}

public class Resource : Item {
    public ResourceEnum resourceEnum;

    private void Start()
    {
        if (GetComponentInChildren<ProgressBarBehaviour>() != null) {
            GetComponentInChildren<ProgressBarBehaviour>().enabled = false;
        }
    }

    public bool isBasicResource() {
        return resourceEnum == ResourceEnum.Rock ||
            resourceEnum == ResourceEnum.Ore ||
            resourceEnum == ResourceEnum.Coal ||
            resourceEnum == ResourceEnum.Wood;
    }

    public bool isRareResource()
    {
        return resourceEnum == ResourceEnum.ArcaneOrb ||
            resourceEnum == ResourceEnum.NatureOrb ||
            resourceEnum == ResourceEnum.StrengthOrb;
    }

    #region ShortPressAction
    private bool CanCollectItemOnMap(Player actor)
    {
        if (actor.GetPlayerActionState() != EPlayerActionState.IDLE)
        {
            return false;
        }

        if (this.tag != "Item") {
            return false;
        }

        return true;
    }

    private bool TryCollectItemOnMap(Player actor)
    {
        if (!CanCollectItemOnMap(actor))
        {
            return false;
        }

        actor.SetCarryingItem(this);

        MapManager.Instance.RemoveItemFromMap(this.gameObject);

        return true;
    }

    public override bool InteractAction(Player actor)
    {
        if (actor.GetCarryingItem() != null) {
            return base.InteractAction(actor);
        }

        return TryCollectItemOnMap(actor);
    }
    #endregion
}
