using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyForgingEvent : EventCommandBase {
    public DestroyForgingEvent(GameObject actor) : base(actor)
    {
    }

    public DestroyForgingEvent(GameObject actor, GameObject receiver) : base(actor, receiver)
    {
    }

    public override void Execute()
    {
        Forge forgeBuilding = receiver.GetComponent<Forge>();
        if (forgeBuilding != null)
        {
            forgeBuilding.DestroyForging();
        }
    }
}
