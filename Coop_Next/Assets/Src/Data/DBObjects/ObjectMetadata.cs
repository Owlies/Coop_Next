using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMetadata {
    public int objectId;
    public string objectName;
    public string description;
    public string prefabPath;
    public int recipeId;
    public string techTreeId;
    public Vector2Int size;
    public ObjectSubType subType;
    public int level;
    public int maxAllowed = 99;
    public Sprite icon;

    protected GameObject gameObjectPrefab;

    public GameObject GetGameObjectFromPool(Transform parent = null)//pool later
    {
        return InitializeNewGameObject(parent);
    }

    private GameObject InitializeNewGameObject(Transform parent = null) {
        GameObject obj = GameObject.Instantiate<GameObject>(gameObjectPrefab, parent) as GameObject;
        InteractiveObject interactiveObj = obj.GetComponent<InteractiveObject>();
        if (interactiveObj != null) {
            interactiveObj.objectMetadata = this;
            interactiveObj.Init();
        }

        return obj;
    }

    public ObjectSubType GetSubType(string subTypeName)
    {
        if (subTypeName.Equals("AttackBuilding"))
            return ObjectSubType.AttackBuilding;
        if (subTypeName.Equals("DefendBuilding"))
            return ObjectSubType.DefendBuilding;
        if (subTypeName.Equals("SupportBuilding"))
            return ObjectSubType.SupportBuilding;
        if (subTypeName.Equals("FunctionalBuilding"))
            return ObjectSubType.FunctionalBuilding;
        if (subTypeName.Equals("EquipmentItem"))
            return ObjectSubType.EquipmentItem;
        if (subTypeName.Equals("ResourceItem"))
            return ObjectSubType.ResourceItem;
        if (subTypeName.Equals("Trap"))
            return ObjectSubType.Trap;

        return ObjectSubType.None;
    }
}
