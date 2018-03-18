using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : Singleton<MapManager> {
    public ObjectConfig objectConfig;
    public LevelConfig levelConfig;
    public GameObject sceneRoot;
    private MapNode[,] mapNodes;

    static public float MAP_SIZE_UNIT = 2.0f;

    private void Start()
    {
        LoadLevel();
    }

    private void LoadLevel()
    {
        if (levelConfig != null)
        {
            mapNodes = new MapNode[levelConfig.mapSize.x, levelConfig.mapSize.y];
            for(int i =0; i < levelConfig.objectInstances.Count; i++)
            {
                ObjectInstance instance = levelConfig.objectInstances[i];
                GameObject obj = GameObject.Instantiate(objectConfig.objects[instance.objectID].prefab, sceneRoot.transform);
                obj.transform.localPosition = new Vector3(0,0,0);
            }
        }
    }
}

public struct MapNode
{
    bool isBlocked;
    bool isMoveable;
}
