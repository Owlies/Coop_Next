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

    public override bool Execute()
    {
        Player player = actor.GetComponent<Player>();
        if (player == null)
        {
            return false;
        }
        return ResourceManager.Instance.CompleteCollecting(player, resource);
    }
}
