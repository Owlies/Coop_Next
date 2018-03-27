using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CancelResourceEvent : EventCommandBase {
    private Resource resource;

    public CancelResourceEvent(GameObject actor) : base(actor)
    {
    }

    public CancelResourceEvent(GameObject actor, GameObject receiver) : base(actor, receiver)
    {
    }

    public CancelResourceEvent(GameObject actor, Resource collectingReseource) : base(actor)
    {
        resource = collectingReseource;
    }

    public override void Execute()
    {
        PlayerController pc = actor.GetComponent<PlayerController>();
        if (pc == null)
        {
            return;
        }
        ResourceManager.Instance.cancelCollecting(pc, resource);
    }
}
