using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : Singleton<MapManager> {
    private MapGridRender gridRender;
    public ObjectConfig objectConfig;
    public LevelConfig levelConfig;
    public GameObject sceneRoot;
    private MapNode[,] mapNodes;
    private Vector2Int mapSize;
    private Vector3 mapOrigin;
    public bool showDebug = true;
    private List<GameObject> gameObjects = new List<GameObject>();

    static public float MAP_SIZE_UNIT = 2.0f;

    public void Start()
    {
        LoadLevel();
        gridRender = new MapGridRender(MAP_SIZE_UNIT);
    }

    private void LoadLevel()
    {
        if (levelConfig != null)
        {
            mapSize = levelConfig.mapSize;
            mapOrigin = new Vector3(-mapSize.x / 2.0f * MAP_SIZE_UNIT, 0, -mapSize.y / 2.0f * MAP_SIZE_UNIT);
            mapNodes = new MapNode[levelConfig.mapSize.x, levelConfig.mapSize.y];
            gameObjects.Clear();
            for (int i = 0; i < levelConfig.objectInstances.Length; i++)
            {
                ObjectInstance instance = levelConfig.objectInstances[i];
                ObjectData objectData = objectConfig.objects[instance.objectID];
                if (objectData.prefab == null)
                    continue;
                GameObject obj = GameObject.Instantiate(objectData.prefab, sceneRoot.transform);
                obj.transform.localPosition = MapIndexToWorldPos(instance.position + new Vector2(objectData.size.x / 2.0f, objectData.size.y / 2.0f));
                for(int idxX = 0; idxX < objectData.size.x; idxX++)
                {
                    for (int idxY = 0; idxY < objectData.size.y; idxY++)
                    {
                        Vector2Int index = instance.position + new Vector2Int(idxX, idxY);
                        if (index.x >= 0 && index.y >= 0 &&
                            index.x < levelConfig.mapSize.x && index.y < levelConfig.mapSize.y)
                        {
                            mapNodes[index.x, index.y].isBlocked = true;
                            mapNodes[index.x, index.y].gameObject = obj;
                            gameObjects.Add(obj);
                        }
                    }
                }
            }
        }
    }

    public IEnumerable<T> GetCollectionOf<T>() where T : InteractiveItem
    {
        foreach(var gameObject in gameObjects)
        {
            T interactiveItem = gameObject.GetComponent<T>();
            if (interactiveItem != null)
                yield return interactiveItem;
        }
    }

    public Vector2Int WorldPosToMapIndex(Vector3 worldPos)
    {
        return new Vector2Int((int)((worldPos.x - mapOrigin.x) / MAP_SIZE_UNIT),
                                (int)((worldPos.z - mapOrigin.z) / MAP_SIZE_UNIT));
    }

    public Vector3 MapIndexToWorldPos(Vector2 mapIndex)
    {
        return new Vector3(mapIndex.x * MAP_SIZE_UNIT + mapOrigin.x, 0, mapIndex.y * MAP_SIZE_UNIT + mapOrigin.z);
    }

    public GameObject GetGameObject(Vector2Int mapIndex)
    {
        return mapNodes[mapIndex.x, mapIndex.y].gameObject;
    }

    public void PickUpItemFromMap(Vector2Int mapIndex)
    {
        if (mapNodes[mapIndex.x, mapIndex.y].gameObject != null)
            PickUpItemFromMap(mapNodes[mapIndex.x, mapIndex.y].gameObject);
    }

    public void PickUpItemFromMap(GameObject obj)
    {
        if (obj == null)
            return;
        for (int i = 0; i < mapNodes.GetLength(0); i++)
        {
            for (int j = 0; j < mapNodes.GetLength(1); j++)
            {
                if (mapNodes[i, j].gameObject == obj)
                {
                    mapNodes[i, j].Clear();
                    gameObjects.Remove(mapNodes[i, j].gameObject);
                }
            }
        }
        //GameObject.DestroyImmediate(obj);
    }

    public bool PlaceItemOnMap(GameObject obj, Vector2Int size, Vector2Int mapIndex)
    {
        bool accessible = true;
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                Vector2Int index = mapIndex + new Vector2Int(i, j);
                if (!mapNodes[index.x, index.y].IsEmpty())
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
                    Vector2Int index = mapIndex + new Vector2Int(i, j);
                    mapNodes[index.x, index.y].AddItemToNode(obj);
                }
            }
            obj.transform.parent = sceneRoot.transform;
            obj.transform.localPosition = MapIndexToWorldPos(mapIndex + new Vector2Int(size.x / 2, size.y / 2)); ;
        }
        return accessible;
    }

    public bool CreateItemOnMap(ObjectData objData, Vector2Int mapIndex)
    {
        GameObject obj = GameObject.Instantiate(objData.prefab, sceneRoot.transform);
        return PlaceItemOnMap(obj, objData.size, mapIndex);
    }

    public bool IsBlocked(Vector2Int mapIndex)
    {
        return mapNodes[mapIndex.x, mapIndex.y].isBlocked;
    }

    public void RenderGrid(Vector2Int mapIndex, Vector2Int size)
    {
        for(int i = 0; i < size.x; i++)
        {
            for(int j = 0; j < size.y;j++)
            {
                Vector2Int index = mapIndex + new Vector2Int(i, j);
                Color color = Color.green;
                if (IsBlocked(index))
                    color = Color.red;
                gridRender.Draw(MapIndexToWorldPos(index), color);
            }
        }
    }

    public override void UpdateMe()
    {
        if (showDebug)
            RenderGrid(new Vector2Int(0,0), mapSize);
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

    public bool AddItemToNode(GameObject obj)
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
