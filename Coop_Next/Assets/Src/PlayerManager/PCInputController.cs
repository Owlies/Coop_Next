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

    protected override void HandleMovement()
    {
        base.HandleMovement();
        playerMovingState = EPlayerMovingState.IDLE;

        float x = 0.0f;
        float z = 0.0f;

        if (Input.GetButton(inputConfig.horizontalAxis) ||
            Input.GetButtonDown(inputConfig.horizontalAxis)) {
            playerMovingState = EPlayerMovingState.HORIZONTAL_MOVING;
            x = Input.GetAxis(inputConfig.horizontalAxis);
        }
        if(Input.GetButton(inputConfig.verticalAxis) || 
            Input.GetButtonDown(inputConfig.verticalAxis))
        {
            playerMovingState = EPlayerMovingState.VERTICAL_MOVING;
            z = Input.GetAxis(inputConfig.verticalAxis);
        }

        if (playerMovingState == EPlayerMovingState.IDLE)
        {
            cancelMovementEvent.Invoke();
        }
        else {
            movementEvent.Invoke(x, z);
        }
    }

    protected override void HandleAction()
    {
        base.HandleAction();
        
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


