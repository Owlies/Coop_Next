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
        playerState = PlayerState.Idle;

        float x = 0.0f;
        float z = 0.0f;

        if (Input.GetButton(inputConfig.horizontalAxis) ||
            Input.GetButtonDown(inputConfig.horizontalAxis)) {
            playerState = PlayerState.HorizontalMoving;
            x = Input.GetAxis(inputConfig.horizontalAxis);
        }
        if(Input.GetButton(inputConfig.verticalAxis) || 
            Input.GetButtonDown(inputConfig.verticalAxis))
        {
            playerState = PlayerState.VerticalMoving;
            z = Input.GetAxis(inputConfig.verticalAxis);
        }

        if (playerState == PlayerState.Idle)
        {
            cancelMovementEvent.Invoke();
        }
        else {
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


