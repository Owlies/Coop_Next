using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBuildingEvent : EventCommandBase
{
    public MoveBuildingEvent(GameObject actor) : base(actor)
    {
    }

    public MoveBuildingEvent(GameObject actor, GameObject receiver) : base(actor, receiver)
    {
    }

    public override void Execute()
    {
        this.receiver.transform.parent = this.actor.transform;
    }
}
