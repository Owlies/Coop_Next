using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : Singleton<MapManager> {
    public ObjectConfig objectConfig;
    public LevelConfig levelConfig;
    public GameObject sceneRoot;
    private MapNode[,] mapNodes;
    private Vector2Int mapSize;
    private Vector3 mapOrigin;
    private float unitSize = 1.0f;

    static public float MAP_SIZE_UNIT = 2.0f;

    private void Start()
    {
        LoadLevel();
    }

    private void LoadLevel()
    {
        if (levelConfig != null)
        {
            mapSize = levelConfig.mapSize;
            mapOrigin = new Vector3(-mapSize.x / 2.0f * unitSize, 0, -mapSize.y / 2.0f * unitSize);
            mapNodes = new MapNode[levelConfig.mapSize.x, levelConfig.mapSize.y];
            for (int i = 0; i < levelConfig.objectInstances.Length; i++)
            {
                ObjectInstance instance = levelConfig.objectInstances[i];
                ObjectData objectData = objectConfig.objects[instance.objectID];
                GameObject obj = GameObject.Instantiate(objectData.prefab, sceneRoot.transform);
                obj.transform.localPosition = MapCoordToWorldPos(instance.position + new Vector2Int(objectData.size.x / 2, objectData.size.y / 2));
                for(int idxX = 0; idxX < objectData.size.x; idxX++)
                {
                    for (int idxY = 0; idxY < objectData.size.y; idxY++)
                    {
                        Vector2Int index = instance.position + new Vector2Int(idxX, idxY);
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
        return new Vector2Int((int)((worldPos.x - mapOrigin.x) / unitSize),
                                (int)((worldPos.z - mapOrigin.z) / unitSize));
    }

    public Vector3 MapCoordToWorldPos(Vector2Int mapCoord)
    {
        return new Vector3(mapCoord.x * unitSize + mapOrigin.x, 0, mapCoord.y * unitSize + mapOrigin.z);
    }

    public GameObject GetGameObject(Vector2Int mapCoord)
    {
        return mapNodes[mapCoord.x, mapCoord.y].gameObject;
    }

    public void Remove(Vector2Int mapCoord)
    {
        if (mapNodes[mapCoord.x, mapCoord.y].gameObject != null)
            Remove(mapNodes[mapCoord.x, mapCoord.y].gameObject);
    }

    public void Remove(GameObject obj)
    {
        if (obj == null)
            return;
        for (int i = 0; i < mapNodes.GetLength(0); i++)
        {
            for (int j = 0; j < mapNodes.GetLength(1); j++)
            {
                if (mapNodes[i, j].gameObject == obj)
                    mapNodes[i, j].Clear();
            }
        }
        GameObject.DestroyImmediate(obj);
    }

    public bool SettleDown(GameObject obj, Vector2Int size, Vector2Int mapCoord)
    {
        bool accessible = true;
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                Vector2Int coord = mapCoord + new Vector2Int(i, j);
                if (!mapNodes[coord.x, coord.y].IsEmpty())
                {
                    accessible = false;
                }
            }
        }
        if (accessible)
        {
            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    Vector2Int coord = mapCoord + new Vector2Int(i, j);
                    mapNodes[coord.x, coord.y].Place(obj);
                }
            }
            obj.transform.parent = sceneRoot.transform;
            obj.transform.localPosition = MapCoordToWorldPos(mapCoord + new Vector2Int(size.x / 2, size.y / 2)); ;
        }
        return accessible;
    }

    public bool SettleDown(ObjectData objData, Vector2Int mapCoord)
    {
        GameObject obj = GameObject.Instantiate(objData.prefab, sceneRoot.transform);
        return SettleDown(obj, objData.size, mapCoord);
    }

    public bool IsBlocked(Vector2Int mapCoord)
    {
        return mapNodes[mapCoord.x, mapCoord.y].isBlocked;
    }
}

public struct MapNode
{
    public GameObject gameObject;
    public bool isBlocked;
    public bool isMoveable;

    public void Clear()
    {
        gameObject = null;
        isBlocked = false;
        isMoveable = true;
    }

    public bool IsEmpty()
    {
        return (gameObject == null && !isBlocked);
    }

    public bool Place(GameObject obj)
    {
        if (IsEmpty())
        {
            gameObject = obj;
            isBlocked = true;
            return true;
        }
        else
            return false;
    }
}
