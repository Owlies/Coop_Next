using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class PlayerMovementEvent : UnityEvent<float, float> {
}

[System.Serializable]
public class CancelPlayerMovementEvent : UnityEvent {
}

[System.Serializable]
public class PlayerActionEvent : UnityEvent<bool, bool> {
}

public enum EPlayerMovingState { IDLE, HORIZONTAL_MOVING, VERTICAL_MOVING };


public class InputController : OverridableMonoBehaviour
{
    public PlayerInputConfig inputConfig;
    public PlayerMovementEvent movementEvent = new PlayerMovementEvent();
    public CancelPlayerMovementEvent cancelMovementEvent = new CancelPlayerMovementEvent();
    public PlayerActionEvent actionEvent = new PlayerActionEvent();
    public EPlayerMovingState playerMovingState;

    public InputController() {
        playerMovingState = EPlayerMovingState.IDLE;
    }

    public void RegisterListeners(UnityAction<float, float> movementAction, UnityAction cancelMovementAction, UnityAction<bool, bool> playerAction)
    {
        movementEvent.AddListener(movementAction);
        cancelMovementEvent.AddListener(cancelMovementAction);
        actionEvent.AddListener(playerAction);
    }

    protected virtual void HandleMovement() {
    }

    protected virtual void HandleAction() {
    }

    public override void UpdateMe() {
        base.UpdateMe();
        HandleMovement();
        HandleAction();
    }
}

