using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCollectResourceEvent : EventCommandBase {
    private Resource resource;

    public StartCollectResourceEvent(GameObject actor) : base(actor)
    {
    }

    public StartCollectResourceEvent(GameObject actor, GameObject receiver) : base(actor, receiver)
    {
    }

    public StartCollectResourceEvent(GameObject actor, Resource collectingReseource) : base(actor)
    {
        resource = collectingReseource;
    }

    public override void Execute()
    {
        PlayerController pc = actor.GetComponent<PlayerController>();
        if (pc == null) {
            return;
        }
        ResourceManager.Instance.startCollecting(pc, resource);
    }
}
