using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using Mono.Data.Sqlite;

public class MetadataManager : Singleton<MetadataManager>
{
    public Dictionary<int, ObjectMetadata> objectsDictionary;
    public Dictionary<ObjectType, Dictionary<ObjectSubType, List<ObjectMetadata>>> objectCollection;
    
    public void Initialize()
    {
        objectCollection = new Dictionary<ObjectType, Dictionary<ObjectSubType, List<ObjectMetadata>>>();
        objectCollection.Add(ObjectType.Building, new Dictionary<ObjectSubType, List<ObjectMetadata>>());
        objectCollection[ObjectType.Building].Add(ObjectSubType.AttackBuilding, new List<ObjectMetadata>());
        objectCollection[ObjectType.Building].Add(ObjectSubType.DefendBuilding, new List<ObjectMetadata>());
        objectCollection[ObjectType.Building].Add(ObjectSubType.SupportBuilding, new List<ObjectMetadata>());
        objectCollection[ObjectType.Building].Add(ObjectSubType.FunctionalBuilding, new List<ObjectMetadata>());
        objectCollection.Add(ObjectType.Item, new Dictionary<ObjectSubType, List<ObjectMetadata>>());
        objectCollection[ObjectType.Item].Add(ObjectSubType.EquipmentItem, new List<ObjectMetadata>());
        objectCollection[ObjectType.Item].Add(ObjectSubType.ResourceItem, new List<ObjectMetadata>());

        objectsDictionary = new Dictionary<int, ObjectMetadata>();

        var buildingList = MetadataLoader.Instance.GetBuildingMetadata();
        for (int i = 0; i < buildingList.Count; i++)
        {
            BuildingMetadata building = buildingList[i];
            objectsDictionary.Add(building.objectId, building);
            if (objectCollection.ContainsKey(ObjectType.Building) == false ||
                objectCollection[ObjectType.Building].ContainsKey(building.subType) == false) {
                Debug.Log("Failed loading BuildingMetadata: " + building.objectName);
            }
            objectCollection[ObjectType.Building][building.subType].Add(building);
        }

        var itemList = MetadataLoader.Instance.GetItemMetadata();
        for (int i = 0; i < itemList.Count; i++)
        {
            ItemMetadata item = itemList[i];
            objectsDictionary.Add(item.objectId, item);
            objectCollection[ObjectType.Item][item.subType].Add(item);
        }
    }

    public Resource GetResourceByType(ResourceEnum e)
    {
        if (objectCollection.ContainsKey(ObjectType.Item) &&
            objectCollection[ObjectType.Item].ContainsKey(ObjectSubType.ResourceItem))
        {
            List<ObjectMetadata> list = objectCollection[ObjectType.Item][ObjectSubType.ResourceItem];
            foreach (var objectData in list)
            {
                if (objectData.gameObject != null)
                {
                    var item = objectData.gameObject.GetComponent<InteractiveObject>();
                    if (item is Resource)
                    {
                        Resource resource = item as Resource;
                        if (resource.resourceEnum == e)
                            return resource;
                    }
                }
            }
        }

        return null;
    }
    
    public BuildingMetadata GetBuildingMetadataWithTechTreeId(string techTreeId)
    {
        int curLevel = TechTreeManager.Instance.GetItemLevel(techTreeId);

        foreach (BuildingMetadata metadata in MetadataLoader.Instance.GetBuildingMetadata())
        {
            if (metadata.techTreeId.Equals(techTreeId) && metadata.level == curLevel)
            {
                return metadata;
            }
        }

        return null;
    }

    public int GetRandomLoot(int lootId)
    {
        if (lootId == 0)
            return 0;
        else
        {
            float currentValue = 0;
            float random = UnityEngine.Random.value;
            LootMetadata lootMetadata = MetadataLoader.Instance.GetLootMetadataById(lootId);
            if (lootMetadata == null || lootMetadata.lootRates.Count == 0)
            {
                Debug.LogError("LootId " + lootId + " is not in db or empty!!!");
                return 0;
            }
            for(int i = 0; i < lootMetadata.lootRates.Count; i++)
            {
                if (currentValue + lootMetadata.lootRates[i].rate > random)
                    return lootMetadata.lootRates[i].itemId;
                else
                {
                    currentValue += lootMetadata.lootRates[i].rate;
                    if (currentValue > 1)
                        Debug.LogError("The total rate of lootId " + lootId + " is over 1");
                }
            }
            return 0;
        }
    }
}

public enum ObjectType
{
    None = 0,
    Building,
    Item,
}

public enum ObjectSubType
{
    None = 0,
    AttackBuilding,
    DefendBuilding,
    SupportBuilding,
    FunctionalBuilding,
    EquipmentItem,
    ResourceItem,
}