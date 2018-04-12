using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "Level", menuName = "CoopNext/Level Config", order = 1)]
public class LevelConfig : ScriptableObject {
    public Vector2Int mapSize = new Vector2Int(0,0);

    [SerializeField]
    public ObjectInstance[] objectInstances;

    [SerializeField]
    public EnemyPrefab[] enemyPrefabs;
}

[System.Serializable]
public struct ObjectInstance
{
    public int objectID;
    public short angle;//0-3
    public Vector2Int position;
}

[System.Serializable]
public struct EnemyPrefab {
    public GameObject enemyPrefab;
}