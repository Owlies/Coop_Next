using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddResourceToForgeEvent : EventCommandBase {
    public GameObject resourceCube;
    public AddResourceToForgeEvent(GameObject actor) : base(actor)
    {
    }

    public AddResourceToForgeEvent(GameObject actor, GameObject receiver) : base(actor, receiver)
    {
    }

    public AddResourceToForgeEvent(GameObject actor, GameObject resourceCube, GameObject receiver) : base(actor, receiver)
    {
        this.resourceCube = resourceCube;
    }

    public override bool Execute()
    {
        Forge forgeBuilding = receiver.GetComponent<Forge>();
        if (forgeBuilding == null) {
            return false;
        }

        return forgeBuilding.AddResourceToForge(this.actor, resourceCube);
    }
}
