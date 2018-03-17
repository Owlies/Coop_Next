using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerInputConfig
{
    public string horizontalAxis;
    public string verticalAxis;
    public string actionButton;
};

public class PlayerController : OverridableMonoBehaviour {
    private int playerId;
    private InputController inputController;

    public PlayerController(InputController iController, int pId) {
        inputController = iController;
        playerId = pId;

        PlayerInputConfig inputConfig = new PlayerInputConfig();
        if (!AppConstant.Instance.isMultiPlayer)
        {
            inputConfig.horizontalAxis = InputAxisEnum.SinglePlayerHorizontal.Value;
            inputConfig.verticalAxis = InputAxisEnum.SinglePlayerVertical.Value;
            inputConfig.actionButton = InputAxisEnum.SinglePlayerAction.Value;
        }
        else {
            if (isFirstPlayer())
            {
                
                inputConfig.horizontalAxis = InputAxisEnum.Player1_Horizontal.Value;
                inputConfig.verticalAxis = InputAxisEnum.Player1_Vertical.Value;
                inputConfig.actionButton = InputAxisEnum.Player1_Action.Value;
            }
            else {
                inputConfig.horizontalAxis = InputAxisEnum.Player2_Horizontal.Value;
                inputConfig.verticalAxis = InputAxisEnum.Player2_Vertical.Value;
                inputConfig.actionButton = InputAxisEnum.Player2_Action.Value;
            }
        }

        inputController.inputConfig = inputConfig;
    }

    public void playerMove(float x, float z) {
        transform.Rotate(0, x, 0);
        transform.Rotate(0, 0, z);
    }

    public void playerAction(bool isLongPress) {

    }

    private bool isFirstPlayer() {
        return playerId == 0;
    }
}
