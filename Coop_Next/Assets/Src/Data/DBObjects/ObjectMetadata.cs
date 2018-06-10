using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMetadata {
    public int id;
    public string name;
    public string description;
    public string prefabPath;
    public int recipeID;
    public string techTreeId;
    public Vector2Int size;
    public ObjectSubType subType;
    public int level;

    public GameObject gameObject;

    public InteractiveItem item
    {
        get
        {
            return gameObject.GetComponent<InteractiveItem>();
        }
    }

    public RecipeMetadata recipe
    {
        get
        {
            return MetadataLoader.Instance.GetRecipeMetadataByID(recipeID);
        }
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

        return ObjectSubType.None;
    }
}
