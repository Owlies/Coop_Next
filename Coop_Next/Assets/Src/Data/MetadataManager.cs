﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using Mono.Data.Sqlite;

public class MetadataManager : Singleton<MetadataManager>
{
    public Dictionary<string, ObjectMetadata> objectsDictionary;
    public Dictionary<ObjectType, Dictionary<ObjectSubType, List<ObjectMetadata>>> objectCollection;

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
                    var item = objectData.gameObject.GetComponent<InteractiveItem>();
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

        objectsDictionary = new Dictionary<string, ObjectMetadata>();

        var buildingList = MetadataLoader.Instance.GetBuildingMetadata();
        for (int i = 0; i < buildingList.Count; i++)
        {
            BuildingMetadata building = buildingList[i];
            objectsDictionary.Add(building.name, building);
            objectCollection[ObjectType.Building][building.subType].Add(building);
        }

        var itemList = MetadataLoader.Instance.GetItemMetadata();
        for (int i = 0; i < itemList.Count; i++)
        {
            ItemMetadata item = itemList[i];
            objectsDictionary.Add(item.name, item);
            objectCollection[ObjectType.Item][item.subType].Add(item);
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