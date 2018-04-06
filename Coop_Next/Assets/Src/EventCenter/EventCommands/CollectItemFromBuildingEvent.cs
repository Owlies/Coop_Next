using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectItemFromBuildingEvent : EventCommandBase {
    public CollectItemFromBuildingEvent(GameObject actor) : base(actor)
    {
    }

    public CollectItemFromBuildingEvent(GameObject actor, GameObject receiver) : base(actor, receiver)
    {
    }

    public override void Execute()
    {
        if (receiver.GetComponent<CollectableBuilding>() != null) {
            receiver.GetComponent<CollectableBuilding>().CollectItem(actor);
        }
    }
}
