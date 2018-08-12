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
    private InteractiveObject carryingItem;
    private InteractiveObject interactingItem;
    private int playerId;
    private InputController inputController;
    private EPlayerActionState playerActionState;
    private Transform carryingPivot = null;
    private HashSet<GameObject> nearbyInteractiveGameObjects;
    private GameObject nearestInteractiveGameObject = null;

    #region initialize
    public void Initialize(InputController iController, int pId, Transform carryingPivot) {
        base.Awake();

        inputController = iController;
        playerId = pId;
        carryingItem = null;
        playerActionState = EPlayerActionState.IDLE;
        this.carryingPivot = carryingPivot;

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
        
        nearbyInteractiveGameObjects = new HashSet<GameObject>();
    }
    #endregion

    #region update
    public override void UpdateMe()
    {
        base.UpdateMe();

        UpdateNearestInteractiveObject();
        ShowBuildingPlacementIndicator();
        SetCarryingItemDirection();
    }

    public override void FixedUpdateMe()
    {
        base.FixedUpdateMe();

        Rigidbody rigidbody = GetComponent<Rigidbody>();
        if (rigidbody.angularVelocity != Vector3.zero) {
            rigidbody.angularVelocity = Vector3.zero;
        }
    }
    
    public void UpdateNearestInteractiveObject()
    {
        var currentNearestObj = GetNearestInteractiveGameObject();
        if (nearestInteractiveGameObject != currentNearestObj)
        {
            if (nearestInteractiveGameObject != null)
            {
                InteractiveObject nearestInteractiveObj = nearestInteractiveGameObject.GetComponent<InteractiveObject>();
                if (nearestInteractiveObj != null && nearestInteractiveObj.callbacks.OnNotBeingNearestToPlayer != null)
                    nearestInteractiveObj.callbacks.OnNotBeingNearestToPlayer();
            }
            if (currentNearestObj != null)
            {
                InteractiveObject currentInteractiveObj = currentNearestObj.GetComponent<InteractiveObject>();
                if (currentInteractiveObj != null && currentInteractiveObj.callbacks.OnBeingNearestToPlayer != null)
                    currentInteractiveObj.callbacks.OnBeingNearestToPlayer();
            }
            nearestInteractiveGameObject = currentNearestObj;
        }
    }

    public void ShowBuildingPlacementIndicator()
    {
        if (carryingItem != null)
        {
            Vector2Int index = MapManager.Instance.WorldPosToMapIndex(carryingItem.transform.position);

            if (carryingItem.GetItemDirection() == ObjectDir.Horizontal)
            {
                index -= new Vector2Int(carryingItem.size.x / 2, carryingItem.size.y / 2);
                MapManager.Instance.RenderGrid(index, carryingItem.size);
            }
            else
            {
                index -= new Vector2Int(carryingItem.size.y / 2, carryingItem.size.x / 2);
                MapManager.Instance.RenderGrid(index, new Vector2Int(carryingItem.size.y, carryingItem.size.x));
            }
        }
    }

    private void SetCarryingItemDirection() {
        if(carryingItem == null) {
            return;
        }

        carryingItem.transform.rotation = gameObject.transform.rotation;
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
        GameObject interactiveObject = GetNearestInteractiveGameObject();

        #region ButtonLongPressActions
        /*  Button Long Press Actions   */
        if (isLongPress) {
            if (TryHandleLongPressAction(interactiveObject)) {
                return;
            }
        }
        #endregion

        #region ButtonShortPressActions
        /*  Button Short Press Actions  */
        if (isButtonDown) {
            if (TryHandleShortPressAction(interactiveObject)) {
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

    private bool CanHandleLongPressAction(GameObject inteactiveObject)
    {
        if (inteactiveObject == null)
        {
            return false;
        }

        if (inteactiveObject.GetComponent<InteractiveObject>() == null)
        {
            return false;
        }

        return true;
    }

    private bool TryHandleLongPressAction(GameObject inteactiveObject)
    {
        if (!CanHandleLongPressAction(inteactiveObject))
        {
            return false;
        }

        InteractiveObject item = inteactiveObject.GetComponent<InteractiveObject>();
        if (item.LongPressAction(this)) {
            interactingItem = item;
            return true;
        }

        return false;
    }

    private bool CanHandleShortPressAction(GameObject inteactiveObject) {
        if (inteactiveObject == null && carryingItem == null) {
            return false;
        }

        if (inteactiveObject != null && inteactiveObject.GetComponent<InteractiveObject>() == null) {
            return false;
        }

        return true;
    }

    private bool TryHandleShortPressAction(GameObject inteactiveObject)
    {
        if (!CanHandleShortPressAction(inteactiveObject))
        {
            return false;
        }

        if ((carryingItem == null) || (inteactiveObject != null && inteactiveObject.GetComponent<BuildingBase>() != null)) {
            InteractiveObject item = inteactiveObject.GetComponent<InteractiveObject>();
            interactingItem = item;
            return item.InteractAction(this);
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
    public bool IsFirstPlayer()
    {
        return playerId == 0;
    }

    public InteractiveObject GetCarryingItem()
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

    public void SetCarryingItem(InteractiveObject item)
    {
        carryingItem = item;
        if (carryingItem.GetComponentInChildren<BoxCollider>() != null)
        {
            carryingItem.GetComponentInChildren<BoxCollider>().enabled = false;
        }
        RemoveNearbyInteractiveGameObject(carryingItem.gameObject);

        //carryingItem.transform.SetPositionAndRotation(carryingItem.transform.position + this.transform.forward * 1.0f, carryingItem.transform.rotation);
        if (carryingItem is BuildingBase)
        {
            playerActionState = EPlayerActionState.CARRYING_BUILDING;
            carryingItem.readyToUpdate = false;
        }
        else if (carryingItem is Item)
        {
            playerActionState = EPlayerActionState.CARRYING_RESOURCE;
            carryingItem.readyToUpdate = true;
        }

        if (carryingItem.GetComponentInChildren<Rigidbody>() != null)
        {
            carryingItem.GetComponentInChildren<Rigidbody>().detectCollisions = false;
        }
        float newScaleX = carryingItem.transform.localScale.x * AppConstant.Instance.moveBuildingScaleChange;
        float newScaleY = carryingItem.transform.localScale.y * AppConstant.Instance.moveBuildingScaleChange;
        float newScaleZ = carryingItem.transform.localScale.z * AppConstant.Instance.moveBuildingScaleChange;
        carryingItem.transform.parent = this.carryingPivot;
        carryingItem.transform.localPosition = Vector3.zero;
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

        if (carryingItem is BuildingBase)
            carryingItem.readyToUpdate = true;
        else if (carryingItem is Item)
            carryingItem.readyToUpdate = false;

        float newScaleX = carryingItem.transform.localScale.x / AppConstant.Instance.moveBuildingScaleChange;
        float newScaleY = carryingItem.transform.localScale.y / AppConstant.Instance.moveBuildingScaleChange;
        float newScaleZ = carryingItem.transform.localScale.z / AppConstant.Instance.moveBuildingScaleChange;
        carryingItem.transform.localScale = new Vector3(newScaleX, newScaleY, newScaleZ);
        carryingItem.transform.rotation = transform.rotation;
        
        carryingItem.transform.parent = null;
        carryingItem = null;
        playerActionState = EPlayerActionState.IDLE;
    }

    public void AddNearbyInteractiveGameObject(GameObject gameObjectToAdd) {
        if (nearbyInteractiveGameObjects.Contains(gameObjectToAdd)) {
            return;
        }
        nearbyInteractiveGameObjects.Add(gameObjectToAdd);
    }

    public void RemoveNearbyInteractiveGameObject(GameObject gameObjectToRemove) {
        nearbyInteractiveGameObjects.Remove(gameObjectToRemove);
    }

    private void RemoveDestroyedNearbyObjects() {
        HashSet<GameObject> newSet = new HashSet<GameObject>(nearbyInteractiveGameObjects);
        foreach (GameObject obj in nearbyInteractiveGameObjects) {
            if (obj == null || obj.activeSelf == false) {
                newSet.Remove(obj);
            }
        }

        nearbyInteractiveGameObjects = newSet;
    }

    private GameObject GetNearestInteractiveGameObject() {
        // Obj might be destroyed
        RemoveDestroyedNearbyObjects();

        GameObject result = null;
        float minDis = int.MaxValue;
        foreach (GameObject obj in nearbyInteractiveGameObjects) {
            if (obj == null) {
                continue;
            }
            float dis = Vector3.Distance(transform.position, obj.transform.position);
            if (minDis > dis) {
                minDis = dis;
                result = obj;
            }
        }

        return result;
    }

    #endregion
}
