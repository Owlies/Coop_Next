using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootObject : InteractiveObject
{
    [HideInInspector]
    public ObjectMetadata lootItemData;
    public string lootBehaviour;

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

        if (LootEnum.DROP_BEHAVIOUR.Equals(lootBehaviour)) {
            return HandleDropBehaviour(actor);
        }

        if (LootEnum.UNLOCK_BEHAVIOUR.Equals(lootBehaviour)) {
            return HandleUnlockBehaviour();
        }

        return false;
    }

    private bool HandleDropBehaviour(Player actor) {
        GameObject obj = GameObject.Instantiate(lootItemData.gameObject);

        if (obj != null) {
            actor.SetCarryingItem(obj.GetComponent<InteractiveObject>());
        }

        GameObject.Destroy(gameObject);

        return true;
    }

    private bool HandleUnlockBehaviour() {
        bool success = TechTreeManager.Instance.UnlockItem(lootItemData.gameObject.GetComponent<InteractiveObject>());
        GameObject.Destroy(gameObject);

        return success;
    }
}
