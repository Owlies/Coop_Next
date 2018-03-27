using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompleteResourceEvent : EventCommandBase {
    private Resource resource;
    public CompleteResourceEvent(GameObject actor) : base(actor)
    {
    }

    public CompleteResourceEvent(GameObject actor, GameObject receiver) : base(actor, receiver)
    {
    }

    public CompleteResourceEvent(GameObject actor, Resource collectingReseource) : base(actor) {
        resource = collectingReseource;
    }

    public override void Execute()
    {
        PlayerController pc = actor.GetComponent<PlayerController>();
        if (pc == null)
        {
            return;
        }
        ResourceManager.Instance.completeCollecting(pc, resource);
    }
}
