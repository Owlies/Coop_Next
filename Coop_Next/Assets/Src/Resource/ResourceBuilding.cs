using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceBuilding : BuildingBase {
    public ResourceEnum resourceEnum;

    private bool isBeenCollecting;
    private float startCollectingTime;

    public new void Start()
    {
        base.Start();
        isBeenCollecting = false;
        startCollectingTime = 0.0f;
    }

    private bool CanStartCollectionResource(Player actor)
    {
        if (actor.GetPlayerActionState() != EPlayerActionState.IDLE)
        {
            return false;
        }

        if (isBeenCollecting)
        {
            return false;
        }

        if (actor.GetCarryingItem() != null)
        {
            return false;
        }

        if (this.transform.tag != "Resource")
        {
            return false;
        }

        return true;
    }

    private bool TryStartCollectingResource(Player actor)
    {
        if (!CanStartCollectionResource(actor))
        {
            return false;
        }

        if (!EventCenter.Instance.ExecuteEvent(new StartCollectResourceEvent(actor.gameObject, this.gameObject)))
        {
            return false;
        }

        startCollectingTime = Time.time;
        actor.SetPlayerActionState(EPlayerActionState.COLLECTING_RESOURCE);

        return true;
    }

    private bool CanCancelCollectingResource(Player actor)
    {
        if (actor.GetPlayerActionState() != EPlayerActionState.COLLECTING_RESOURCE)
        {
            return false;
        }

        if (startCollectingTime <= 0.0f)
        {
            return false;
        }

        if (Time.time - startCollectingTime >= AppConstant.Instance.resourceCollectingSeconds)
        {
            return false;
        }

        return true;
    }

    private bool TryCancelCollectingResource(Player actor)
    {
        if (!CanCancelCollectingResource(actor))
        {
            return false;
        }

        if (!EventCenter.Instance.ExecuteEvent(new CancelResourceEvent(actor.gameObject, this.gameObject)))
        {
            return false;
        }

        startCollectingTime = 0.0f;
        actor.SetPlayerActionState(EPlayerActionState.IDLE);

        return true;
    }

    private bool CanCompleteCollectingResource(Player actor)
    {
        if (startCollectingTime <= 0.0f)
        {
            return false;
        }

        if (Time.time - startCollectingTime < AppConstant.Instance.resourceCollectingSeconds)
        {
            return false;
        }

        if (actor.GetPlayerActionState() != EPlayerActionState.COLLECTING_RESOURCE)
        {
            return false;
        }

        return true;
    }

    private bool TryCompleteCollectingResource(Player actor)
    {
        if (!CanCompleteCollectingResource(actor))
        {
            return false;
        }

        if (!EventCenter.Instance.ExecuteEvent(new CompleteResourceEvent(actor.gameObject, this.gameObject)))
        {
            return false;
        }

        startCollectingTime = 0.0f;
        actor.SetPlayerActionState(EPlayerActionState.CARRYING_RESOURCE);

        return true;
    }

    public override bool PressReleaseAction(Player actor)
    {
        if (TryCompleteCollectingResource(actor)) {
            return true;
        }

        return TryCancelCollectingResource(actor);
    }

    public override bool ShortPressAction(Player actor)
    {
        return TryStartCollectingResource(actor);
    }
}
