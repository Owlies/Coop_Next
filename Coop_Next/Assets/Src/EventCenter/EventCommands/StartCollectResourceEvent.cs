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
        Player player = actor.GetComponent<Player>();
        if (player == null) {
            return false;
        }
        return ResourceManager.Instance.StartCollecting(player, resource);
    }
}
