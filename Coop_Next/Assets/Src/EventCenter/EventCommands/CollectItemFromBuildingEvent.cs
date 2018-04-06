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

    public override bool Execute()
    {
        CollectableBuilding collectableBuilding = receiver.GetComponent<CollectableBuilding>();
        if (collectableBuilding == null) {
            return false;
        }

        return collectableBuilding.CollectItem(actor);
    }
}
