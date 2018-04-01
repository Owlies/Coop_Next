using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CancelResourceEvent : EventCommandBase {
    private GameObject resource;

    public CancelResourceEvent(GameObject actor) : base(actor)
    {
    }

    public CancelResourceEvent(GameObject actor, GameObject receiver) : base(actor, receiver)
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
        ResourceManager.Instance.CancelCollecting(pc, resource);
    }
}
