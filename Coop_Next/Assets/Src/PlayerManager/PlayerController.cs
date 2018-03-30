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
    private GameObject carryingResourceCube;

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
        float horizontalSpeed = 0.0f;
        float verticalSpeed = 0.0f;
        if (x < 0) {
            transform.rotation = Quaternion.Euler(0, 270, 0);
            horizontalSpeed = -AppConstant.Instance.playerHorizontalSpeed;
        } else if (x > 0)
        {
            transform.rotation = Quaternion.Euler(0, 90, 0);
            horizontalSpeed = AppConstant.Instance.playerHorizontalSpeed;
        }

        if (z < 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            verticalSpeed = -AppConstant.Instance.playerVerticalSpeed;
        } else if (z > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            verticalSpeed = AppConstant.Instance.playerVerticalSpeed;
        }

        GetComponent<Rigidbody>().velocity = new Vector3(horizontalSpeed, 0, verticalSpeed);

        cancelActions();
    }

    public void cancelPlayerMovement(bool isHorizontal) {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        if (isHorizontal) {
            rigidbody.velocity = new Vector3(0, 0, rigidbody.velocity.z);
            return;
        }
        
        rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0, 0);
    }

    public void playerAction(bool isLongPress, bool isButtonDown) {
        // Long press actions
        if (isLongPress) {
            if (tryHandleMoveBuildingAction()) {
                return;
            }
        }

        // First press actions
        if (isButtonDown) {
            // Place building if carrying
            if (carryingBuilding != null)
            {
                EventCenter.Instance.executeEvent(new PlaceBuildingEvent(this.gameObject, carryingBuilding));
                carryingBuilding = null;
                return;
            }

            if (tryStartCollectingResource()) {
                return;
            }

            return;
        }

        // Release actions
        if (tryCancelCollectingResource())
        {
            return;
        }

        if (tryCompleteCollectingResource()) {
            return;
        }
    }

    private void cancelActions() {
        tryCancelCollectingResource();
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
    private bool tryStartCollectingResource() {
        if (collectingResource == null)
        {
            startCollectingResource();
            return true;
        }

        return false;
    }

    private bool tryCancelCollectingResource() {
        if (Time.time - startCollectingTime < AppConstant.Instance.resourceCollectingSeconds)
        {
            EventCenter.Instance.executeEvent(new CancelResourceEvent(this.gameObject, collectingResource));
            collectingResource = null;
            startCollectingTime = 0;
            return true;
        }

        return false;
    }

    private bool tryCompleteCollectingResource()
    {
        if (Time.time - startCollectingTime >= AppConstant.Instance.resourceCollectingSeconds)
        {
            EventCenter.Instance.executeEvent(new CompleteResourceEvent(this.gameObject, collectingResource));
            return true;
        }

        return false;
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

    public void setCarryingResourceCube(GameObject cube) {
        carryingResourceCube = cube;
    }

    #endregion

    #region OtherFunctions

    public bool isFirstPlayer() {
        return playerId == 0;
    }

    #endregion
}
