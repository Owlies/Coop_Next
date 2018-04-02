using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartForgeEvent : EventCommandBase {
    public StartForgeEvent(GameObject actor) : base(actor)
    {
    }

    public StartForgeEvent(GameObject actor, GameObject receiver) : base(actor, receiver)
    {
    }

    public override void Execute()
    {
        Forge forgeBuilding = receiver.GetComponent<Forge>();
        if (forgeBuilding != null)
        {
            forgeBuilding.StartForging();
        }
    }
}
