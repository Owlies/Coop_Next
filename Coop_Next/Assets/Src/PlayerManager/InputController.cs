using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class PlayerMovementEvent : UnityEvent<float, float> {
}

[System.Serializable]
public class CancelPlayerMovementEvent : UnityEvent<bool> {
}

[System.Serializable]
public class PlayerActionEvent : UnityEvent<bool, bool> {
}

public class InputController : OverridableMonoBehaviour
{
    public PlayerInputConfig inputConfig;
    public PlayerMovementEvent movementEvent = new PlayerMovementEvent();
    public CancelPlayerMovementEvent cancelMovementEvent = new CancelPlayerMovementEvent();
    public PlayerActionEvent actionEvent = new PlayerActionEvent();

    public InputController() {
        
    }

    public void registerListeners(UnityAction<float, float> movementAction, UnityAction<bool> cancelMovementAction, UnityAction<bool, bool> playerAction)
    {
        movementEvent.AddListener(movementAction);
        cancelMovementEvent.AddListener(cancelMovementAction);
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

