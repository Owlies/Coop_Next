using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : Singleton<MapManager> {
    public ObjectConfig objectConfig;
    public LevelConfig levelConfig;
    public GameObject sceneRoot;
    private MapNode[,] mapNodes;
    private Vector2Int mapSize;

    static public float MAP_SIZE_UNIT = 2.0f;

    public Vector2Int WorldToMapIndex(Vector2Int idx)
    {
        Vector2Int result = idx;// - mapSize / new Vector2Int(2,2);
        result.x += mapSize.x / 2;
        result.y += mapSize.y / 2;
        return result;
    }

    private void Start()
    {
        LoadLevel();
    }

    private void LoadLevel()
    {
        if (levelConfig != null)
        {
            mapSize = levelConfig.mapSize;
            mapNodes = new MapNode[levelConfig.mapSize.x, levelConfig.mapSize.y];
            for (int i = 0; i < levelConfig.objectInstances.Length; i++)
            {
                ObjectInstance instance = levelConfig.objectInstances[i];
                ObjectData objectData = objectConfig.objects[instance.objectID];
                GameObject obj = GameObject.Instantiate(objectData.prefab, sceneRoot.transform);
                obj.transform.localPosition = new Vector3(instance.position.x + objectData.size.x / 2, 0, instance.position.y + objectData.size.y / 2);
                for(int idxX = 0; idxX < objectData.size.x; idxX++)
                {
                    for (int idxY = 0; idxY < objectData.size.y; idxY++)
                    {
                        Vector2Int index = WorldToMapIndex(instance.position + new Vector2Int(idxX, idxY));
                        if (index.x > 0 && index.y > 0 &&
                            index.x < levelConfig.mapSize.x && index.y < levelConfig.mapSize.y)
                        {
                            mapNodes[index.x, index.y].isBlocked = true;
                            mapNodes[index.x, index.y].gameObject = obj;
                        }
                    }
                }
            }
        }
    }

    public Vector2Int WorldPosToMapCoord(Vector3 worldPos)
    {
        return Vector2Int.zero;
    }

    public Vector3 MapCoordToWorldPos(Vector2Int mapCoord)
    {
        return Vector3.zero;
    }

    public bool PickUp(Vector2Int mapCoord)
    {
        return true;
    }

    public bool PickUp(GameObject obj)
    {
        return true;
    }

    public bool SettleDown(GameObject obj, Vector2Int size, Vector2Int mapCoord)
    {
        return true;
    }

}

public struct MapNode
{
    public GameObject gameObject;
    public bool isBlocked;
    public bool isMoveable;
}
