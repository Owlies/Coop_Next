using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Object", menuName = "CoopNext/Object Config", order = 1)]
public class ObjectConfig : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField]
    private ObjectData[] objects;
    public Dictionary<string, ObjectData> objectsDictionary;
    
    public Dictionary<ObjectType, Dictionary<ObjectSubType, List<ObjectData>>> objectCollection;

    public Resource GetResourceByType(ResourceEnum e)
    {
        if (objectCollection.ContainsKey(ObjectType.Item) &&
            objectCollection[ObjectType.Item].ContainsKey(ObjectSubType.ResourceItem))
        {
            List<ObjectData> list = objectCollection[ObjectType.Item][ObjectSubType.ResourceItem];
            foreach(var objectData in list)
            {
                if (objectData.item is Resource)
                {
                    Resource resource = objectData.item as Resource;
                    if (resource.resourceEnum == e)
                        return resource;
                }
            }
        }

        return null;
    }

    public void OnAfterDeserialize()
    {
        // Convert ObjectData
        objectsDictionary = new Dictionary<string, ObjectData>();
        objectCollection = new Dictionary<ObjectType, Dictionary<ObjectSubType, List<ObjectData>>>();
        foreach (var obj in objects)
        {
            if (!objectCollection.ContainsKey(obj.type))
                objectCollection.Add(obj.type, new Dictionary<ObjectSubType, List<ObjectData>>());
            if (!objectCollection[obj.type].ContainsKey(obj.subType))
                objectCollection[obj.type].Add(obj.subType, new List<ObjectData>());
            objectCollection[obj.type][obj.subType].Add(obj);
            string name = obj.name;
            while (objectsDictionary.ContainsKey(name))
                name += "_d";
            objectsDictionary.Add(name,obj);
        }
        objects = null;
    }
    
    public void OnBeforeSerialize()
    {
        // Convert ObjectData
        if (objectsDictionary != null && objectsDictionary.Count > 0)
        {
            objects = new ObjectData[objectsDictionary.Count];
            int i = 0;
            foreach(var obj in objectsDictionary)
            {
                objects[i++] = obj.Value;
            }
        }
    }
}

[System.Serializable]
public struct Receipt {
    public ResourceEnum[] resources;
}

[System.Serializable]
public struct ResourceItemMapping {
    public ResourceEnum resourceEnum;
    public Resource resourceItem;
}

public enum ObjectType
{
    Building,
    Item,
}

public enum ObjectSubType
{
    AttackBuilding,
    DefendBuilding,
    SupportBuilding,
    FunctionalBuilding,
    EquipmentItem,
    ResourceItem,
}

[System.Serializable]
public class ObjectData
{
    public string name;
    public InteractiveItem item;
    public Receipt[] receipts;
    public ObjectType type;
    public ObjectSubType subType;

    public Vector2Int size
    {
        get
        {
            if (item == null)
                return new Vector2Int(1, 1);
            else
                return item.size;
        }
    }

    public GameObject gameObject
    {
        get
        {
            if (item == null)
                return null;
            else
                return item.gameObject;
        }
    }
}

[System.Serializable]
public class ItemData
{
    public string name;
    public string description;
    public string actionName;
}

[System.Serializable]
public class LootData
{

}