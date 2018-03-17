using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class PlayerMovementEvent : UnityEvent<float, float> {
}

[System.Serializable]
public class PlayerActionEvent : UnityEvent<bool> {
}

public class InputController : OverridableMonoBehaviour
{
    protected float horizontalSpeed;
    protected float verticalSpeed;
    public PlayerInputConfig inputConfig;
    public PlayerMovementEvent movementEvent = new PlayerMovementEvent();
    public PlayerActionEvent actionEvent = new PlayerActionEvent();

    public InputController() {
        horizontalSpeed = AppConstant.Instance.playerHorizontalSpeed;
        verticalSpeed = AppConstant.Instance.playerVerticalSpeed;
    }

    public void registerListeners(UnityAction<float, float> movementAction, UnityAction<bool> playerAction)
    {
        movementEvent.AddListener(movementAction);
        actionEvent.AddListener(playerAction);
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

