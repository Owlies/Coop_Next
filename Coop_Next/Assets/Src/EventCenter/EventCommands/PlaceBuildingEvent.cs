using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceBuildingEvent : EventCommandBase {
    public PlaceBuildingEvent(GameObject actor) : base(actor)
    {
    }

    public PlaceBuildingEvent(GameObject actor, GameObject receiver) : base(actor, receiver)
    {
    }

    public override void Execute()
    {
        this.receiver.transform.parent = null;
    }
}
