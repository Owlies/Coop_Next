using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBuildingEvent : EventCommandBase
{
    private float moveingBuildingAnchorMultplier = 2.0f;
    public MoveBuildingEvent(GameObject actor) : base(actor)
    {
    }

    public MoveBuildingEvent(GameObject actor, GameObject receiver) : base(actor, receiver)
    {
    }

    public override bool Execute()
    {
        //this.receiver.transform.parent = this.actor.transform;
        //this.receiver.transform.localPosition = Vector3.forward * moveingBuildingAnchorMultplier;


        MapManager.Instance.RemoveItemFromMap(this.receiver);

        return true;
    }
}
