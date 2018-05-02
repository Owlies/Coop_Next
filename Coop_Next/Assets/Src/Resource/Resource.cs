using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProgressBar;

public enum ResourceEnum {Stone, Wood, Ore, Coal}

public class Resource : InteractiveItem {
    public ResourceEnum resourceEnum;
    

    private void Start()
    {
        if (GetComponentInChildren<ProgressBarBehaviour>() != null) {
            GetComponentInChildren<ProgressBarBehaviour>().enabled = false;
        }
    }

    public bool isBasicResource() {
        return resourceEnum == ResourceEnum.Stone ||
            resourceEnum == ResourceEnum.Ore ||
            resourceEnum == ResourceEnum.Coal ||
            resourceEnum == ResourceEnum.Wood;
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

        MapManager mapManager = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        mapManager.RemoveItemFromMap(this.gameObject);

        return true;
    }

    public override bool ShortPressAction(Player actor)
    {
        if (actor.GetCarryingItem() != null) {
            return base.ShortPressAction(actor);
        }

        return TryCollectItemOnMap(actor);
    }
    #endregion
}
