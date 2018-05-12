using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Object", menuName = "CoopNext/Object Config", order = 1)]
public class ObjectConfig : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField]
    private ObjectData[] objects;

    public Dictionary<string, ObjectData> objectsDictionary;

    public void OnAfterDeserialize()
    {
        objectsDictionary = new Dictionary<string, ObjectData>();
        foreach(var obj in objects)
        {
            string name = obj.name;
            while (objectsDictionary.ContainsKey(name))
                name += "_d";
            objectsDictionary.Add(name,obj);
        }
        objects = null;
    }
    
    public void OnBeforeSerialize()
    {
        if (objectsDictionary != null && objectsDictionary.Count > 0)
        {
            objects = new ObjectData[objectsDictionary.Count];
            int i = 0;
            foreach(var obj in objectsDictionary)
            {
                objects[i] = obj.Value;
                i++;
            }
        }
    }
}

[System.Serializable]
public struct Receipt {
    public ResourceEnum[] resources;
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