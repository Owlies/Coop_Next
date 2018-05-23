using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceBuildingEvent : EventCommandBase {
    public PlaceBuildingEvent(GameObject actor) : base(actor)
    {
    }

    public PlaceBuildingEvent(GameObject actor, GameObject receiver) : base(actor, receiver)
    {
    }

    public override bool Execute()
    {
        float newScaleX = this.receiver.transform.localScale.x / AppConstant.Instance.moveBuildingScaleChange;
        float newScaleY = this.receiver.transform.localScale.y / AppConstant.Instance.moveBuildingScaleChange;
        float newScaleZ = this.receiver.transform.localScale.z / AppConstant.Instance.moveBuildingScaleChange;
        this.receiver.transform.localScale = new Vector3(newScaleX, newScaleY, newScaleZ);
        this.receiver.transform.parent = null;

        return true;
    }
}
