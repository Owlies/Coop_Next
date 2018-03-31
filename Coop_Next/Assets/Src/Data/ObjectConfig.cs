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
    public GameObject prefab;
    public Vector2Int size;
    public Receipt[] receipts;
}