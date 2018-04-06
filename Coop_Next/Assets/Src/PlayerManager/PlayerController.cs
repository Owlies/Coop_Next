using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerInputConfig
{
    public string horizontalAxis;
    public string verticalAxis;
    public string actionButton;
};

public enum EPlayerActionState {
    IDLE,
    CARRYING_RESOURCE,
    CARRYING_BUILDING,
    FORGING,
    COLLECTING_RESOURCE,
}

public class PlayerController : OverridableMonoBehaviour {
    private int playerId;
    private InputController inputController;
    private GameObject carryingItem;
    private GameObject collectingResource;
    private float startCollectingTime;
    private EPlayerActionState playerActionState;

    #region initialize
    public void Initialize(InputController iController, int pId) {
        base.Awake();

        inputController = iController;
        playerId = pId;
        carryingItem = null;
        playerActionState = EPlayerActionState.IDLE;

        PlayerInputConfig inputConfig = new PlayerInputConfig();
        if (!AppConstant.Instance.isMultiPlayer)
        {
            inputConfig.horizontalAxis = InputAxisEnum.SinglePlayerHorizontal.Value;
            inputConfig.verticalAxis = InputAxisEnum.SinglePlayerVertical.Value;
            inputConfig.actionButton = InputAxisEnum.SinglePlayerAction.Value;
        }
        else {
            if (IsFirstPlayer())
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

    #region update
    public override void FixedUpdateMe()
    {
        base.FixedUpdateMe();

        Rigidbody rigidbody = GetComponent<Rigidbody>();
        if (rigidbody.angularVelocity != Vector3.zero) {
            rigidbody.angularVelocity = Vector3.zero;
        }
    }
    #endregion

    #region action
    public void PlayerMove(float x, float z) {
        float horizontalSpeed = 0.0f;
        float verticalSpeed = 0.0f;
        float speed = AppConstant.Instance.playerMovingSpeed;

        if (!x.Equals(0.0f) && !z.Equals(0.0f)) {
            speed = speed / Mathf.Sqrt(2.0f);
        }

        if (x < 0) {
            transform.rotation = Quaternion.Euler(0, 270, 0);
            horizontalSpeed = -speed * Time.deltaTime;
        } else if (x > 0)
        {
            transform.rotation = Quaternion.Euler(0, 90, 0);
            horizontalSpeed = speed * Time.deltaTime;
        }

        if (z < 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            verticalSpeed = -speed * Time.deltaTime;
        } else if (z > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            verticalSpeed = speed * Time.deltaTime;
        }

        GetComponent<Rigidbody>().velocity = new Vector3(horizontalSpeed, 0, verticalSpeed);
        if (!transform.position.y.Equals(0.0f)) {
            transform.position = new Vector3(transform.position.x, 0.0f, transform.position.z);
        }

        CancelActions();
    }

    public void CancelPlayerMovement() {
        GetComponent<Rigidbody>().velocity = new Vector3(0.0f, 0.0f, 0.0f);
    }

    public void PlayerAction(bool isLongPress, bool isButtonDown) {
        RaycastHit hitObject;
        bool isHit = Physics.Raycast(new Vector3(transform.position.x, 0.01f, transform.position.z), transform.forward, out hitObject, AppConstant.Instance.playerActionRange, 1 << 8);

        #region ButtonLongPressActions
        /*  Button Long Press Actions   */
        if (isLongPress) {
            if (TryHandleMoveBuildingAction(isHit, hitObject)) {
                return;
            }
        }
        #endregion

        #region ButtonShortPressActions
        /*  Button Short Press Actions  */
        if (isButtonDown) {
            if (TryCollectItemOnMap(isHit, hitObject)) {
                return;
            }

            if (TryStartCollectingResource(isHit, hitObject)) {
                return;
            }

            if (TryAddResourceToForge(isHit, hitObject)) {
                return;
            }

            if (TryCollectItemFromBuilding(isHit, hitObject)) {
                return;
            }

            if (TryPlaceItemOnMap())
            {
                return;
            }
            return;
        }
        #endregion

        #region ButtonReleaseActions
        /*  Button Release Actions  */
        if (TryCancelCollectingResource())
        {
            return;
        }

        if (TryCompleteCollectingResource()) {
            return;
        }

        #endregion
    }

    private void CancelActions() {
        TryCancelCollectingResource();
    }
    #endregion

    #region InteractiveItemActions
    private bool CanMoveBuilding(bool isHist, RaycastHit hitObject) {
        if (!isHist) {
            return false;
        }

        if (playerActionState != EPlayerActionState.IDLE) {
            return false;
        }

        if (hitObject.transform.gameObject.tag != "Building")
        {
            return false;
        }

        return true;
    }

    private bool TryHandleMoveBuildingAction(bool isHit, RaycastHit hitObject) {
        if (!CanMoveBuilding(isHit, hitObject)) {
            return false;
        }

        SetCarryingItem(hitObject.transform.gameObject);
        // Somehow changing parent will change hitObject.transform.gameObject to points to the parent
        return EventCenter.Instance.ExecuteEvent(new MoveBuildingEvent(this.gameObject, hitObject.transform.gameObject));
    }

    private bool CanCollectItemOnMap(bool isHit, RaycastHit hitObject) {
        if (!isHit) {
            return false;
        }

        if (playerActionState != EPlayerActionState.IDLE)
        {
            return false;
        }

        if (hitObject.transform.gameObject.tag != "Item")
        {
            return false;
        }

        return true;
    }

    private bool TryCollectItemOnMap(bool isHit, RaycastHit hitObject) {
        if (!CanCollectItemOnMap(isHit, hitObject)) {
            return false;
        }

        SetCarryingItem(hitObject.transform.gameObject);

        return true;
    }

    private bool CanPlaceItemOnMap() {
        if (carryingItem == null) {
            return false;
        }

        if (!(playerActionState != EPlayerActionState.CARRYING_BUILDING ||
            playerActionState != EPlayerActionState.CARRYING_RESOURCE)) {
            return false;
        }

        return true;
    }

    private bool TryPlaceItemOnMap() {
        if (!CanPlaceItemOnMap()) {
            return false;
        }

        if (playerActionState == EPlayerActionState.CARRYING_BUILDING) {
            EventCenter.Instance.ExecuteEvent(new PlaceBuildingEvent(this.gameObject, carryingItem));
        }
        
        UnsetCarryingItem();

        playerActionState = EPlayerActionState.IDLE;

        return true;
    }

    private bool CanCollectItemFromBuilding(bool isHit, RaycastHit hitObject) {
        if (!isHit) {
            return false;
        }

        if (playerActionState != EPlayerActionState.IDLE) {
            return false;
        }

        if (hitObject.transform.GetComponent<BuildingBase>() == null) {
            return false;
        }

        return true;
    }

    private bool TryCollectItemFromBuilding(bool isHit, RaycastHit hitObject) {
        if (!CanCollectItemFromBuilding(isHit, hitObject)) {
            return false;
        }

        return EventCenter.Instance.ExecuteEvent(new CollectItemFromBuildingEvent(this.gameObject, hitObject.transform.gameObject));
    }

    #endregion

    #region Forging
    private bool CanAddResourceToForge(bool isHit, RaycastHit hitObject) {
        if (!isHit)
        {
            return false;
        }

        if (playerActionState != EPlayerActionState.CARRYING_RESOURCE) {
            return false;
        }

        if (carryingItem == null)
        {
            return false;
        }

        if (hitObject.transform.gameObject.tag != "Forge")
        {
            return false;
        }
        
        return true;
    }

    private bool TryAddResourceToForge(bool isHit, RaycastHit hitObject) {
        if (!CanAddResourceToForge(isHit, hitObject)) {
            return false;
        }

        return EventCenter.Instance.ExecuteEvent(new AddResourceToForgeEvent(this.gameObject, carryingItem, hitObject.transform.gameObject));
    }

    public void OnAddResourceToForgeComplete() {
        GameObject.Destroy(carryingItem);
        UnsetCarryingItem();
    }

    #endregion

    #region CollectResource
    private bool CanCancelCollectingResource() {
        if (playerActionState != EPlayerActionState.COLLECTING_RESOURCE) {
            return false;
        }

        if (startCollectingTime <= 0.0f) {
            return false;
        }

        if (Time.time - startCollectingTime >= AppConstant.Instance.resourceCollectingSeconds) {
            return false;
        }

        return true;
    }

    private bool TryCancelCollectingResource() {
        if (!CanCancelCollectingResource())
        {
            return false;
        }

        if (!EventCenter.Instance.ExecuteEvent(new CancelResourceEvent(this.gameObject, collectingResource))) {
            return false;
        }

        collectingResource = null;
        startCollectingTime = 0;
        playerActionState = EPlayerActionState.IDLE;

        return true;
    }

    private bool CanCompleteCollectingResource() {
        if (startCollectingTime <= 0.0f) {
            return false;
        }

        if (Time.time - startCollectingTime < AppConstant.Instance.resourceCollectingSeconds) {
            return false;
        }

        if (playerActionState != EPlayerActionState.COLLECTING_RESOURCE) {
            return false;
        }

        return true;
    }

    private bool TryCompleteCollectingResource()
    {
        if (!CanCompleteCollectingResource())
        {
            return false;
        }

        if (!EventCenter.Instance.ExecuteEvent(new CompleteResourceEvent(this.gameObject, collectingResource))) {
            return false;
        }

        collectingResource = null;
        startCollectingTime = 0;

        playerActionState = EPlayerActionState.CARRYING_RESOURCE;
        return true;
    }

    private bool CanStartCollectionResource(bool isHit, RaycastHit hitObject)
    {
        if (!isHit)
        {
            return false;
        }

        if (playerActionState != EPlayerActionState.IDLE)
        {
            return false;
        }

        if (collectingResource != null)
        {
            return false;
        }

        if (carryingItem != null)
        {
            return false;
        }

        if (hitObject.transform.tag != "Resource")
        {
            return false;
        }

        return true;
    }

    private bool TryStartCollectingResource(bool isHit, RaycastHit hitObject)
    {
        if (!CanStartCollectionResource(isHit, hitObject))
        {
            return false;
        }

        if (!EventCenter.Instance.ExecuteEvent(new StartCollectResourceEvent(this.gameObject, hitObject.transform.gameObject)))
        {
            return false;
        }
        startCollectingTime = Time.time;
        collectingResource = hitObject.transform.gameObject;
        playerActionState = EPlayerActionState.COLLECTING_RESOURCE;

        return true;
    }

    #endregion

    #region OtherFunctions

    public bool IsFirstPlayer() {
        return playerId == 0;
    }

    public void SetCarryingItem(GameObject item) {
        carryingItem = item;
        if (carryingItem.GetComponent<BoxCollider>() != null) {
            carryingItem.GetComponent<BoxCollider>().enabled = false;
            carryingItem.transform.SetPositionAndRotation(carryingItem.transform.position + this.transform.forward * 1.0f, carryingItem.transform.rotation);
            if (carryingItem.tag == "Building") {
                playerActionState = EPlayerActionState.CARRYING_BUILDING;
            }
            else if(carryingItem.tag == "Item") {
                playerActionState = EPlayerActionState.CARRYING_RESOURCE;
            }
        }

        if (carryingItem.GetComponent<Rigidbody>() != null) {
            carryingItem.GetComponent<Rigidbody>().detectCollisions = false;
        }

        carryingItem.transform.parent = this.transform;
    }

    public void UnsetCarryingItem() {
        if (carryingItem.GetComponent<BoxCollider>() != null)
        {
            carryingItem.GetComponent<BoxCollider>().enabled = true;
        }

        if (carryingItem.GetComponent<Rigidbody>() != null)
        {
            carryingItem.GetComponent<Rigidbody>().detectCollisions = true;
        }

        carryingItem.transform.parent = null;
        carryingItem = null;
        playerActionState = EPlayerActionState.IDLE;
    }

    #endregion
}
