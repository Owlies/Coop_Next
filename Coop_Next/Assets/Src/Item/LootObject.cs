using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootObject : InteractiveObject
{
    [HideInInspector]
    public ItemMetadata lootItemData;

    private new void Awake()
    {
        base.Awake();
    }
    public override bool ShortPressAction(Player actor)
    {
        if (actor.GetCarryingItem() != null)
        {
            return base.ShortPressAction(actor);
        }

        GameObject obj = GameObject.Instantiate(lootItemData.gameObject);

        if (obj != null)
            actor.SetCarryingItem(obj.GetComponent<InteractiveObject>());

        GameObject.Destroy(gameObject);

        return true;
    }
}
