using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Object", menuName = "CoopNext/Object Config", order = 1)]
public class ObjectConfig : ScriptableObject
{
    [SerializeField]
    public ObjectData[] objects;
}

[System.Serializable]
public struct Receipt {
    public ResourceEnum[] resources;
}

[System.Serializable]
public struct ObjectData
{
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