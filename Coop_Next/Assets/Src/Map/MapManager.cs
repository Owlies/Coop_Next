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

    // <GameObject, isOnMap>
    private Dictionary<GameObject, bool> gameObjectOnMapDictionary = new Dictionary<GameObject, bool>();

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
            gameObjectOnMapDictionary.Clear();
            for (int i = 0; i < levelConfig.objectInstances.Length; i++)
            {
                ObjectInstance instance = levelConfig.objectInstances[i];
                if (!objectConfig.objectsDictionary.ContainsKey(instance.objectKey))
                {
                    Debug.Log("No " + instance.objectKey + " in config!!!");
                    continue;
                }
                ObjectData objectData = objectConfig.objectsDictionary[instance.objectKey];
                if (objectData == null || objectData.gameObject == null)
                {
                    Debug.Log("No " + instance.objectKey + "'s gameobject in config!!!");
                    continue;
                }
                GameObject obj = GameObject.Instantiate(objectData.gameObject, sceneRoot.transform);

                //temp, just for test
                if (obj.GetComponent<Orb>())
                {
                    GameObject.DestroyImmediate(obj);
                    obj = OrbManager.CreateOrbGameObject(objectConfig.orbData[0]);
                }

                obj.transform.localPosition = MapIndexToWorldPos(instance.position + new Vector2(objectData.size.x / 2.0f, objectData.size.y / 2.0f));
                if (instance.dir == ObjectDir.Vertical)
                {
                    obj.transform.localEulerAngles = new Vector3(0, 90, 0);
                    obj.transform.localPosition = MapIndexToWorldPos(instance.position + new Vector2(objectData.size.y / 2.0f, objectData.size.x / 2.0f));
                }
                for(int idxX = 0; idxX < objectData.size.x; idxX++)
                {
                    for (int idxY = 0; idxY < objectData.size.y; idxY++)
                    {
                        Vector2Int index = instance.position + new Vector2Int(idxX, idxY);

                        if (instance.dir == ObjectDir.Vertical)
                            index = instance.position + new Vector2Int(idxY, idxX);
                            if (index.x >= 0 && index.y >= 0 &&
                            index.x < levelConfig.mapSize.x && index.y < levelConfig.mapSize.y)
                        {
                            mapNodes[index.x, index.y].isBlocked = true;
                            mapNodes[index.x, index.y].gameObject = obj;
                        }
                    }
                }
                gameObjectOnMapDictionary[obj] = true;
            }
        }
    }

    public IEnumerable<T> GetCollectionOfItemsOnMap<T>() where T : InteractiveItem
    {
        foreach(KeyValuePair <GameObject, bool> entry in gameObjectOnMapDictionary)
        {
            if (entry.Key == null) {
                Debug.LogError("[GetCollectionOfItemsOnMap] Empty objects found in gameObjectOnMapDictionary");
                continue;
            }
            T interactiveItem = entry.Key.GetComponent<T>();
            if (entry.Value && interactiveItem != null)
                yield return interactiveItem;
        }
    }

    public IEnumerable<T> GetCollectionOfItems<T>() where T : InteractiveItem {
        foreach (KeyValuePair<GameObject, bool> entry in gameObjectOnMapDictionary) {
            if (entry.Key == null) {
                Debug.LogError("[GetCollectionOfItems] Empty objects found in gameObjectOnMapDictionary");
                continue;
            }
            T interactiveItem = entry.Key.GetComponent<T>();
            if (interactiveItem != null)
                yield return interactiveItem;
        }
    }

    public int GetNumberOfItemsOnMapWithName(string itemName) {
        int count = 0;
        foreach (InteractiveItem item in MapManager.Instance.GetCollectionOfItems<InteractiveItem>()) {
            if (item.name.Equals(itemName)) {
                count++;
            }
        }

        return count;
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

    public void RemoveItemFromMap(Vector2Int mapIndex)
    {
        if (mapNodes[mapIndex.x, mapIndex.y].gameObject != null)
            RemoveItemFromMap(mapNodes[mapIndex.x, mapIndex.y].gameObject);
    }

    public void RemoveItemFromMap(GameObject obj)
    {
        if (obj == null)
            return;
        for (int i = 0; i < mapNodes.GetLength(0); i++)
        {
            for (int j = 0; j < mapNodes.GetLength(1); j++)
            {
                if (mapNodes[i, j].gameObject == obj)
                {
                    if (gameObjectOnMapDictionary.ContainsKey(obj)) {
                        gameObjectOnMapDictionary[obj] = false;
                    }
                    
                    mapNodes[i, j].Clear();
                }
            }
        }
    }

    public void OnItemDestroyed(GameObject item) {
        if (!gameObjectOnMapDictionary.ContainsKey(item)) {
            return;
        }
        
        RemoveItemFromMap(item);
        gameObjectOnMapDictionary.Remove(item);
    }

    public bool PlaceItemOnMap(InteractiveItem item, Vector2Int mapIndex, ObjectDir dir = ObjectDir.Horizontal)
    {
        GameObject obj = item.gameObject;
        Vector2Int size = item.size;
        bool accessible = true;
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                Vector2Int index = mapIndex + new Vector2Int(i, j);
                if (dir == ObjectDir.Vertical)
                    index = mapIndex + new Vector2Int(j, i);
                if (IsMapIndexOutOfBound(index) || !mapNodes[index.x, index.y].IsEmpty())
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
                    if (dir == ObjectDir.Vertical)
                        index = mapIndex + new Vector2Int(j, i);
                    mapNodes[index.x, index.y].AddItemToNode(obj);
                }
            }
            gameObjectOnMapDictionary[obj] = true;
            obj.transform.parent = sceneRoot.transform;
            obj.transform.localPosition = MapIndexToWorldPos(mapIndex + new Vector2(size.x / 2.0f, size.y / 2.0f));
            obj.transform.rotation = Quaternion.identity;
            if (dir == ObjectDir.Vertical)
                obj.transform.localPosition = MapIndexToWorldPos(mapIndex + new Vector2(size.y / 2.0f, size.x / 2.0f));
        }
        return accessible;
    }

    public void OnItemCreated(GameObject item) {
        gameObjectOnMapDictionary[item] = false;
    }

    public void OnItemCreated(GameObject item, bool isOnMap) {
        gameObjectOnMapDictionary[item] = isOnMap;
    }

    public bool CreateItemOnMap(ObjectData objData, Vector2Int mapIndex)
    {
        GameObject obj = GameObject.Instantiate(objData.gameObject, sceneRoot.transform);

        InteractiveItem item = obj.GetComponent<InteractiveItem>();
        if (item == null)
            return false;

        return PlaceItemOnMap(item, mapIndex);
    }

    public bool IsMapIndexOutOfBound(Vector2Int mapIndex)
    {
        return mapIndex.x < 0 || mapIndex.y < 0 || mapIndex.x >= mapSize.x || mapIndex.y >= mapSize.y;
    }

    public bool IsBlocked(Vector2Int mapIndex)
    {
        if (IsMapIndexOutOfBound(mapIndex))
            return true;
        else
            return mapNodes[mapIndex.x, mapIndex.y].isBlocked;
    }

    public bool CanPlaceItemOnMap(InteractiveItem item, Vector2Int pos, ObjectDir dir)
    {
        for(int i = 0; i < item.size.x; i++)
        {
            for(int j = 0; j < item.size.y; j++)
            {
                Vector2Int index = pos + new Vector2Int(i, j);
                if (dir == ObjectDir.Vertical)
                    index = pos + new Vector2Int(j, i);
                if (IsBlocked(index))
                    return false;
            }
        }
        return true;
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
