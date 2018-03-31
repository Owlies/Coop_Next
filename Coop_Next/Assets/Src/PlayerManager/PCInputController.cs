using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PCInputController : InputController
{
    private float lastPressTime;
    public PCInputController() : base()
    {
        lastPressTime = 0.0f;
    }

    protected override void handleMovement()
    {
        base.handleMovement();
        PlayerState curState = playerState;

        if (Input.GetButton(inputConfig.horizontalAxis) ||
            Input.GetButtonDown(inputConfig.horizontalAxis) ||
            Input.GetButton(inputConfig.verticalAxis) || 
            Input.GetButtonDown(inputConfig.verticalAxis))
        {
            playerState = PlayerState.Moveing;
            float x = Input.GetAxis(inputConfig.horizontalAxis);
            float z = Input.GetAxis(inputConfig.verticalAxis);
            movementEvent.Invoke(x, z);
        }
        else {
            playerState = PlayerState.Idle;
        }

        if (playerState == PlayerState.Idle)
        {
            cancelMovementEvent.Invoke(true);
            cancelMovementEvent.Invoke(false);
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


