using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : Singleton<MapManager> {
    public MetadataManager metadataManager;
    public LevelConfig levelConfig;
    public GameObject sceneRoot;
    private MapNode[,] mapNodes;
    private Vector2Int mapSize;
    private Vector3 mapOrigin;
    public bool showDebug = true;
    public int currentHitPoint = 10;

    public void ReduceCurrentHP(int value)
    {
        currentHitPoint -= value;
        Debug.LogErrorFormat("只有{0}血啦!!!!!!!",currentHitPoint);
        if (currentHitPoint <= 0)
            Debug.LogError("DIEDIEDIEDIEDIEDIE");
    }

    // <GameObject, isOnMap>
    private Dictionary<GameObject, bool> gameObjectOnMapDictionary = new Dictionary<GameObject, bool>();

    static public float MAP_SIZE_UNIT = 2.0f;

    public void Initialize()
    {
        metadataManager = MetadataManager.Instance;
        LoadLevel();
        //gridRender = new PlaneRenderer(MAP_SIZE_UNIT, "Materials/GridRenderer");
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
                if (!metadataManager.objectsDictionary.ContainsKey(instance.objectId))
                {
                    Debug.Log("No " + instance.objectId + " in config!!!");
                    continue;
                }
                ObjectMetadata objectData = metadataManager.objectsDictionary[instance.objectId];
                if (objectData == null)
                {
                    Debug.Log("No " + instance.objectId + "'s gameobject in config!!!");
                    continue;
                }
                GameObject obj = objectData.GetGameObjectFromPool(sceneRoot.transform);

                Item item = obj.GetComponent<Item>();
                if (item != null)
                {
                    ItemManager.InitItem(obj, (ItemMetadata)objectData);
                }

                obj.transform.localPosition = MapIndexToWorldPos(instance.position + new Vector2(1 / 2.0f, 1 / 2.0f));
                if (instance.dir == ObjectDir.Vertical)
                {
                    obj.transform.localEulerAngles = new Vector3(0, 90, 0);
                    obj.transform.localPosition = MapIndexToWorldPos(instance.position + new Vector2(1 / 2.0f, 1 / 2.0f));
                }

                Vector2Int index = instance.position;

                if (instance.dir == ObjectDir.Vertical)
                    index = instance.position;
                if (index.x >= 0 && index.y >= 0 &&
                index.x < levelConfig.mapSize.x && index.y < levelConfig.mapSize.y)
                {
                    mapNodes[index.x, index.y].isBlocked = true;
                    mapNodes[index.x, index.y].gameObject = obj;
                }

                gameObjectOnMapDictionary[obj] = true;
            }
        }
    }

    public IEnumerable<T> GetCollectionOfItemsWithinRange<T>(float range, Vector2Int mapIdx) where T : InteractiveObject
    {
        List<T> tmpObjs = new List<T>();
        int offset = (int)(range / MAP_SIZE_UNIT);
        Vector2Int min = mapIdx - new Vector2Int(offset, offset);
        if (min.x < 0)
            min.x = 0;
        if (min.y < 0)
            min.y = 0;
        Vector2Int max = mapIdx + new Vector2Int(offset, offset);
        if (max.x > mapSize.x)
            max.x = mapSize.x;
        if (max.y > mapSize.y)
            max.y = mapSize.y;
        for (int i = min.x; i < max.x; ++i)
        {
            for(int j = min.y; j < max.y; ++j)
            {
                GameObject obj = mapNodes[i, j].gameObject;
                if (obj != null)
                {
                    var interactiveObj = obj.GetComponent<T>();
                    if (interactiveObj != null && !tmpObjs.Contains(interactiveObj))
                    {
                        tmpObjs.Add(interactiveObj);
                        yield return interactiveObj;
                    }
                }

            }
        }

    }

    public IEnumerable<T> GetCollectionOfItemsOnMap<T>() where T : InteractiveObject
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

    public IEnumerable<T> GetCollectionOfItems<T>() where T : InteractiveObject {
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
        foreach (InteractiveObject item in MapManager.Instance.GetCollectionOfItems<InteractiveObject>()) {
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
                    var interactiveObj = obj.GetComponent<InteractiveObject>();
                    if (interactiveObj != null)
                        interactiveObj.posOnMap = new Vector2Int(-1,-1);
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

    public bool PlaceItemOnMap(InteractiveObject item, Vector2Int mapIndex, ObjectDir dir = ObjectDir.Horizontal)
    {
        GameObject obj = item.gameObject;
        bool accessible = true;

        if (IsMapIndexOutOfBound(mapIndex) || !mapNodes[mapIndex.x, mapIndex.y].IsEmpty())
            accessible = false;

        if (accessible)
        {
            mapNodes[mapIndex.x, mapIndex.y].AddItemToNode(obj);
            gameObjectOnMapDictionary[obj] = true;
            obj.transform.parent = sceneRoot.transform;
            obj.transform.localPosition = MapIndexToWorldPos(mapIndex + new Vector2(1 / 2.0f, 1 / 2.0f));

            if (dir == ObjectDir.Vertical && !item.objectMetadata.fixDir)
                obj.transform.localPosition = MapIndexToWorldPos(mapIndex + new Vector2(1 / 2.0f, 1 / 2.0f));

            if (item.objectMetadata.fixDir)
                obj.transform.localRotation = Quaternion.identity;

            item.posOnMap = mapIndex;
        }
        return accessible;
    }

    public void OnItemCreated(GameObject item) {
        gameObjectOnMapDictionary[item] = false;
    }

    public void OnItemCreated(GameObject item, bool isOnMap) {
        gameObjectOnMapDictionary[item] = isOnMap;
    }

    public bool CreateItemOnMap(ObjectMetadata objData, Vector2Int mapIndex)
    {
        GameObject obj = objData.GetGameObjectFromPool(sceneRoot.transform);

        InteractiveObject item = obj.GetComponent<InteractiveObject>();
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

    public bool CanPlaceItemOnMap(InteractiveObject item)
    {
        ObjectDir dir = item.GetItemDirection();
        Vector2Int pos = GeItemMapPosition(item);

        if (IsBlocked(pos))
            return false;

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
                PlaneRenderer.Draw(MapIndexToWorldPos(index + new Vector2(1 / 2.0f, 1 / 2.0f)), MAP_SIZE_UNIT, color,Resources.Load("Materials/GridRenderer") as Material);
            }
        }
    }

    public Vector2Int GeItemMapPosition(InteractiveObject item) {
        if (item == null) {
            return new Vector2Int(int.MinValue, int.MinValue);
        }
            
        ObjectDir itemDirection = item.GetItemDirection();
        Vector2Int index = MapManager.Instance.WorldPosToMapIndex(item.transform.position);
        index -= new Vector2Int(1 / 2, 1 / 2);
            
        return index;
    }

    private Dictionary<GameObject,GameObject> originToNew = new Dictionary<GameObject, GameObject>(32);
    public void UpgradeBuildings(ObjectSubType subType, int level)
    {
        var iter = gameObjectOnMapDictionary.GetEnumerator();
        while(iter.MoveNext())
        {
            InteractiveObject interactiveObject = iter.Current.Key.GetComponent<InteractiveObject>();
            if (interactiveObject.objectMetadata.subType == subType && interactiveObject.objectMetadata.level < level)
            {
                var objMetaData = metadataManager.GetObjectMetadataWithObjectId(interactiveObject.objectMetadata.objectId + level - interactiveObject.objectMetadata.level);
                if (objMetaData != null)
                {
                    GameObject originObj = iter.Current.Key;
                    GameObject obj = objMetaData.GetGameObjectFromPool(sceneRoot.transform);
                    obj.transform.SetPositionAndRotation(originObj.transform.position, originObj.transform.rotation);

                    originToNew.Add(originObj, obj);
                }
            }
        }

        for (int i = 0; i < mapNodes.GetLength(0); i++)
        {
            for (int j = 0; j < mapNodes.GetLength(1); j++)
            {
                if (mapNodes[i, j].gameObject != null && 
                    originToNew.ContainsKey(mapNodes[i, j].gameObject))
                {
                    GameObject origin = mapNodes[i, j].gameObject;
                    mapNodes[i, j].gameObject = originToNew[origin];
                    gameObjectOnMapDictionary.Add(mapNodes[i, j].gameObject, true);
                    gameObjectOnMapDictionary.Remove(origin);
                    GameObject.Destroy(origin);
                }
            }
        }

        originToNew.Clear();
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
