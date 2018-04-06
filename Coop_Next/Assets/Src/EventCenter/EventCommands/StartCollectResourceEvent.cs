using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCollectResourceEvent : EventCommandBase {
    private GameObject resource;

    public StartCollectResourceEvent(GameObject actor) : base(actor)
    {
    }

    public StartCollectResourceEvent(GameObject actor, GameObject receiver) : base(actor, receiver)
    {
        resource = receiver;
    }

    public override bool Execute()
    {
        PlayerController pc = actor.GetComponent<PlayerController>();
        if (pc == null) {
            return false;
        }
        return ResourceManager.Instance.StartCollecting(pc, resource);
    }
}
