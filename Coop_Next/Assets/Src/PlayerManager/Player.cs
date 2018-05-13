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

public class Player:OverridableMonoBehaviour
{
    private InteractiveItem carryingItem;
    private InteractiveItem interactingItem;
    private int playerId;
    private InputController inputController;
    private EPlayerActionState playerActionState;
    public ObjectDir carryingItemDir = ObjectDir.Horizontal;
    

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
    public override void UpdateMe()
    {
        base.UpdateMe();

        ShowBuildingPlacementIndicator();
    }

    public override void FixedUpdateMe()
    {
        base.FixedUpdateMe();

        Rigidbody rigidbody = GetComponent<Rigidbody>();
        if (rigidbody.angularVelocity != Vector3.zero) {
            rigidbody.angularVelocity = Vector3.zero;
        }
    }
    
    public void ShowBuildingPlacementIndicator()
    {
        if (carryingItem != null)
        {
            MapManager mapManager = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
            Vector2Int index = mapManager.WorldPosToMapIndex(carryingItem.transform.position);
            if (carryingItemDir == ObjectDir.Horizontal)
            {
                index -= new Vector2Int(carryingItem.size.x / 2, carryingItem.size.y / 2);
                mapManager.RenderGrid(index, carryingItem.size);
            }
            else
            {
                index -= new Vector2Int(carryingItem.size.y / 2, carryingItem.size.x / 2);
                mapManager.RenderGrid(index, new Vector2Int(carryingItem.size.y, carryingItem.size.x));
            }
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
            carryingItemDir = ObjectDir.Vertical;
        } else if (x > 0)
        {
            transform.rotation = Quaternion.Euler(0, 90, 0);
            horizontalSpeed = speed * Time.deltaTime;
            carryingItemDir = ObjectDir.Vertical;
        }

        if (z < 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            verticalSpeed = -speed * Time.deltaTime;
            carryingItemDir = ObjectDir.Horizontal;
        } else if (z > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            verticalSpeed = speed * Time.deltaTime;
            carryingItemDir = ObjectDir.Horizontal;
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
            if (TryHandleLongPressAction(isHit, hitObject)) {
                return;
            }
        }
        #endregion

        #region ButtonShortPressActions
        /*  Button Short Press Actions  */
        if (isButtonDown) {
            if (TryHandleShortPressAction(isHit, hitObject)) {
                return;
            }

            return;
        }
        #endregion

        #region ButtonReleaseActions
        /*  Button Release Actions  */
        if (TryHandlePressReleaseAction()) {
            return;
        }

        #endregion
    }

    private void CancelActions() {
        if (interactingItem != null) {
            interactingItem.PressReleaseAction(this);
        }
    }
    #endregion

    #region InputHandling

    private bool CanHandleLongPressAction(bool isHit, RaycastHit hitObject)
    {
        if (!isHit)
        {
            return false;
        }

        if (hitObject.transform.GetComponent<InteractiveItem>() == null)
        {
            return false;
        }

        return true;
    }

    private bool TryHandleLongPressAction(bool isHit, RaycastHit hitObject)
    {
        if (!CanHandleLongPressAction(isHit, hitObject))
        {
            return false;
        }

        InteractiveItem item = hitObject.transform.GetComponent<InteractiveItem>();
        if (item.LongPressAction(this)) {
            interactingItem = item;
            return true;
        }

        return false;
    }

    private bool CanHandleShortPressAction(bool isHit, RaycastHit hitObject) {
        if (!isHit && carryingItem == null) {
            return false;
        }

        if (isHit && hitObject.transform.GetComponent<InteractiveItem>() == null) {
            return false;
        }

        return true;
    }

    private bool TryHandleShortPressAction(bool isHit, RaycastHit hitObject)
    {
        if (!CanHandleShortPressAction(isHit, hitObject))
        {
            return false;
        }

        if (isHit) {
            InteractiveItem item = hitObject.transform.GetComponent<InteractiveItem>();
            interactingItem = item;
            return item.ShortPressAction(this);
        }
        
        return carryingItem.ShortPressAction(this);
    }

    private bool CanHandlePressReleaseAction() {
        return interactingItem != null;
    }

    private bool TryHandlePressReleaseAction()
    {
        if (!CanHandlePressReleaseAction()) {
            return false;
        }

        bool result = interactingItem.PressReleaseAction(this);
        interactingItem = null;
        return result;
    }

    #endregion

    #region OtherFunctions
    
    public Vector2Int GetCarryingItemPosition()
    {
        if (carryingItem == null)
            return new Vector2Int(int.MinValue, int.MinValue);
        MapManager mapManager = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        Vector2Int index = mapManager.WorldPosToMapIndex(carryingItem.transform.position);
        if (carryingItemDir == ObjectDir.Horizontal)
            index -= new Vector2Int(carryingItem.size.x / 2, carryingItem.size.y / 2);
        else
            index -= new Vector2Int(carryingItem.size.y / 2, carryingItem.size.x / 2);

        return  index;
    }

    public bool IsFirstPlayer()
    {
        return playerId == 0;
    }

    public InteractiveItem GetCarryingItem()
    {
        return carryingItem;
    }

    public void SetPlayerActionState(EPlayerActionState state)
    {
        playerActionState = state;
    }

    public EPlayerActionState GetPlayerActionState() {
        return playerActionState;
    }

    public void SetCarryingItem(InteractiveItem item)
    {
        carryingItem = item;
        if (carryingItem.GetComponentInChildren<BoxCollider>() != null)
        {
            carryingItem.GetComponentInChildren<BoxCollider>().enabled = false;
            carryingItem.transform.SetPositionAndRotation(carryingItem.transform.position + this.transform.forward * 1.0f, carryingItem.transform.rotation);
            if (carryingItem.tag == "Building")
            {
                playerActionState = EPlayerActionState.CARRYING_BUILDING;
            }
            else if (carryingItem.tag == "Item")
            {
                playerActionState = EPlayerActionState.CARRYING_RESOURCE;
            }
        }

        if (carryingItem.GetComponentInChildren<Rigidbody>() != null)
        {
            carryingItem.GetComponentInChildren<Rigidbody>().detectCollisions = false;
        }
        float newScaleX = carryingItem.transform.localScale.x * AppConstant.Instance.moveBuildingScaleChange;
        float newScaleY = carryingItem.transform.localScale.y * AppConstant.Instance.moveBuildingScaleChange;
        float newScaleZ = carryingItem.transform.localScale.z * AppConstant.Instance.moveBuildingScaleChange;
        carryingItem.transform.parent = this.transform;
        carryingItem.transform.localScale = new Vector3(newScaleX, newScaleY, newScaleZ);
    }

    public void UnsetCarryingItem()
    {
        if (carryingItem.GetComponentInChildren<BoxCollider>() != null)
        {
            carryingItem.GetComponentInChildren<BoxCollider>().enabled = true;
        }

        if (carryingItem.GetComponentInChildren<Rigidbody>() != null)
        {
            carryingItem.GetComponentInChildren<Rigidbody>().detectCollisions = true;
        }

        float newScaleX = carryingItem.transform.localScale.x / AppConstant.Instance.moveBuildingScaleChange;
        float newScaleY = carryingItem.transform.localScale.y / AppConstant.Instance.moveBuildingScaleChange;
        float newScaleZ = carryingItem.transform.localScale.z / AppConstant.Instance.moveBuildingScaleChange;
        carryingItem.transform.localScale = new Vector3(newScaleX, newScaleY, newScaleZ);

        carryingItem.transform.parent = null;
        carryingItem = null;
        playerActionState = EPlayerActionState.IDLE;
    }

    #endregion
}
