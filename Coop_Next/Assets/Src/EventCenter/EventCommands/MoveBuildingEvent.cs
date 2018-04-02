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

    public override void Execute()
    {
        this.receiver.GetComponent<BoxCollider>().enabled = false;
        float newScaleX = this.receiver.transform.localScale.x * AppConstant.Instance.moveBuildingScaleChange;
        float newScaleY = this.receiver.transform.localScale.y * AppConstant.Instance.moveBuildingScaleChange;
        float newScaleZ = this.receiver.transform.localScale.z * AppConstant.Instance.moveBuildingScaleChange;
        this.receiver.transform.localScale = new Vector3(newScaleX, newScaleY, newScaleZ);
        
        this.receiver.transform.parent = this.actor.transform;
        this.receiver.transform.localPosition = Vector3.forward * moveingBuildingAnchorMultplier;

        MapManager mapManager = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        Vector2Int index = mapManager.WorldPosToMapIndex(this.receiver.transform.position);
        mapManager.RenderGrid(index, new Vector2Int(1, 1));
    }
}
