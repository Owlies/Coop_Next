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
        float x = Input.GetAxis(inputConfig.horizontalAxis) * AppConstant.Instance.playerHorizontalSpeed * Time.deltaTime;
        float z = Input.GetAxis(inputConfig.verticalAxis) * AppConstant.Instance.playerVerticalSpeed * Time.deltaTime;

        if (x.Equals(0.0f) && z.Equals(0.0f)) {
            return;
        }

        movementEvent.Invoke(x, z);
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


