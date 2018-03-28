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
    private GameObject carryingBuilding;
    private GameObject collectingResource;
    private float startCollectingTime;

    #region initialize
    public void initialize(InputController iController, int pId) {
        base.Awake();

        inputController = iController;
        playerId = pId;
        carryingBuilding = null;

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
    #endregion

    #region action
    public void playerMove(float x, float z) {
        transform.Translate(x, 0, 0);
        transform.Translate(0, 0, z);
    }

    public void playerAction(bool isLongPress) {
        //TODO(Huayu):Call Event Center
        if (isLongPress) {
            if (tryHandleMoveBuildingAction()) {
                return;
            }
        }

        // Place building if carrying
        if (carryingBuilding != null) {
            EventCenter.Instance.executeEvent(new PlaceBuildingEvent(this.gameObject, carryingBuilding));
            carryingBuilding = null;
            return;
        }

        tryHandleResourceAction();
    }
    #endregion

    #region MoveBuilding
    private bool tryHandleMoveBuildingAction() {
        RaycastHit hitObject;
        if (Physics.Raycast(transform.position, transform.forward, out hitObject, AppConstant.Instance.playerActionRange, 1 << 8))
        {
            if (hitObject.transform.gameObject.tag != "Building")
            {
                return false;
            }

            EventCenter.Instance.executeEvent(new MoveBuildingEvent(this.gameObject, hitObject.transform.gameObject));
            carryingBuilding = hitObject.transform.gameObject;

            return true;
        }
        return false;
    }
    #endregion

    #region CollectResource
    private void tryHandleResourceAction() {
        if (collectingResource == null) {
            startCollectingResource();
            return;
        }

        if (Time.time - startCollectingTime >= AppConstant.Instance.resourceCollectingSeconds) {
            EventCenter.Instance.executeEvent(new CompleteResourceEvent(this.gameObject, collectingResource));
            return;
        }

        EventCenter.Instance.executeEvent(new CancelResourceEvent(this.gameObject, collectingResource));
    }

    private void startCollectingResource() {
        RaycastHit hitObject;
        if (Physics.Raycast(transform.position, transform.forward, out hitObject, AppConstant.Instance.playerActionRange, 1 << 8))
        {
            if (hitObject.transform.gameObject.tag != "Resource")
            {
                return;
            }

            EventCenter.Instance.executeEvent(new StartCollectResourceEvent(this.gameObject, hitObject.transform.gameObject));
            startCollectingTime = Time.time;
            collectingResource = hitObject.transform.gameObject;
        }
    }

    #endregion

    #region OtherFunctions
    public bool isFirstPlayer() {
        return playerId == 0;
    }
    #endregion
}
