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
        if (Input.GetButtonDown(inputConfig.actionButton) || Input.GetButton(inputConfig.actionButton))
        {
            lastPressTime += Time.deltaTime;
        }
        else if (Input.GetButtonUp(inputConfig.actionButton)) {
            if (lastPressTime >= AppConstant.Instance.playerActionLongPressThreshold)
            {
                actionEvent.Invoke(true);
            }
            else {
                actionEvent.Invoke(false);
            }
            lastPressTime = 0.0f;
        }
    }

}


