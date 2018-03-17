using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : OverridableMonoBehaviour
{
    protected int playerId;
    protected float horizontalSpeed;
    protected float verticalSpeed;

    public InputController(int pId) {
        playerId = pId;
        horizontalSpeed = AppConstant.Instance.playerHorizontalSpeed;
        verticalSpeed = AppConstant.Instance.playerVerticalSpeed;
    }

    protected virtual void handleMovement() {
    }

    protected virtual void handleAction() {
    }

    public override void UpdateMe() {
        base.UpdateMe();
        handleMovement();
        handleAction();
    }
}

