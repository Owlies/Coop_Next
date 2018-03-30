using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PCInputController : InputController
{
    private float lastPressTime;
    private float deadThreshold = 0.95f;
    private float startMoveThreshold = 0.25f;
    public PCInputController() : base()
    {
        lastPressTime = 0.0f;
    }

    protected override void handleMovement()
    {
        base.handleMovement();
        float x = Input.GetAxis(inputConfig.horizontalAxis);
        float z = Input.GetAxis(inputConfig.verticalAxis);

        if (x.Equals(0.0f) && z.Equals(0.0f))
        {
            return;
        }

        if (!(x < -deadThreshold || x > deadThreshold) || Input.GetButtonUp(inputConfig.horizontalAxis)) {
            cancelMovementEvent.Invoke(true);
        }

        if (!(z < -deadThreshold || z > deadThreshold) || Input.GetButtonUp(inputConfig.verticalAxis))
        {
            cancelMovementEvent.Invoke(false);
        }

        if (x > startMoveThreshold || x < -startMoveThreshold || z > startMoveThreshold || z < -startMoveThreshold) {
            movementEvent.Invoke(x, z);
        }

    }

    protected override void handleAction()
    {
        base.handleAction();
        
        if (Input.GetButtonDown(inputConfig.actionButton))
        {
            actionEvent.Invoke(false, true);
        }
        else if (Input.GetButtonUp(inputConfig.actionButton))
        {
            if (lastPressTime >= AppConstant.Instance.playerActionLongPressThreshold)
            {
                actionEvent.Invoke(true, false);
            }
            else
            {
                actionEvent.Invoke(false, false);
            }
            lastPressTime = 0.0f;
        }
        else if (Input.GetButton(inputConfig.actionButton))
        {
            lastPressTime += Time.deltaTime;
        }
    }
}


