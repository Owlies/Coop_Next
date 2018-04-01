using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CancelForgingEvent : EventCommandBase {
    public CancelForgingEvent(GameObject actor) : base(actor)
    {
    }

    public CancelForgingEvent(GameObject actor, GameObject receiver) : base(actor, receiver)
    {
    }

    public override void Execute()
    {
        Forge forgeBuilding = receiver.GetComponent<Forge>();
        if (forgeBuilding != null)
        {
            forgeBuilding.CancelForging();
        }
    }

}
