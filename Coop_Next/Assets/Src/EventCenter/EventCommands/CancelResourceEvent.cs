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

    public override bool Execute()
    {
        Player player = actor.GetComponent<Player>();
        if (player == null)
        {
            return false;
        }
        return ResourceManager.Instance.CancelCollecting(player, resource);
    }
}
