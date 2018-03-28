using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompleteResourceEvent : EventCommandBase {
    private GameObject resource;
    public CompleteResourceEvent(GameObject actor) : base(actor)
    {
    }

    public CompleteResourceEvent(GameObject actor, GameObject receiver) : base(actor, receiver)
    {
        resource = receiver;
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
