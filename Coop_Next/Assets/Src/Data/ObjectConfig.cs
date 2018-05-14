using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Object", menuName = "CoopNext/Object Config", order = 1)]
public class ObjectConfig : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField]
    private ObjectData[] objects;
    public Dictionary<string, ObjectData> objectsDictionary;

    public OrbData[] orbData;

    public ResourceItemMapping[] resourceObject;
    public Dictionary<ResourceEnum, Resource> resourceEnumToItemMap;

    public void OnAfterDeserialize()
    {
        // Convert ObjectData
        objectsDictionary = new Dictionary<string, ObjectData>();
        foreach(var obj in objects)
        {
            string name = obj.name;
            while (objectsDictionary.ContainsKey(name))
                name += "_d";
            objectsDictionary.Add(name,obj);
        }
        objects = null;

        // Convert ResourceObjets
        resourceEnumToItemMap = new Dictionary<ResourceEnum, Resource>();
        foreach (ResourceItemMapping mapping in resourceObject) {
            resourceEnumToItemMap[mapping.resourceEnum] = mapping.resourceItem;
        }
        resourceObject = null;

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

        // Convert ResourceObjets
        if (resourceEnumToItemMap != null && resourceEnumToItemMap.Count > 0) {
            resourceObject = new ResourceItemMapping[resourceEnumToItemMap.Count];
            int index = 0;
            foreach (KeyValuePair<ResourceEnum, Resource> entry in resourceEnumToItemMap) {
                ResourceItemMapping tmpMapping = new ResourceItemMapping();
                tmpMapping.resourceEnum = entry.Key;
                tmpMapping.resourceItem = entry.Value;
                resourceObject[index++] = tmpMapping;
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

[System.Serializable]
public class ObjectData
{
    public string name;
    public InteractiveItem item;
    public Receipt[] receipts;

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