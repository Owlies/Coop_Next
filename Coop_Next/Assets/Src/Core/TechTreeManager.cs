using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechTreeManager : Singleton<TechTreeManager> {
    private Dictionary<string, int> itemNameToLevelMap;

	// Use this for initialization
	void Start () {
        InitializeWithBuildings();
    }

    private void InitializeWithBuildings() {
        List<BuildingMetadataDBObject> buildingMetadataList = MetadataLoader.Instance.LoadBuildingMetadata();
        foreach (BuildingMetadataDBObject obj in buildingMetadataList) {
            if (itemNameToLevelMap.ContainsKey(obj.techTreeId)) {
                continue;
            }

            itemNameToLevelMap.Add(obj.techTreeId, 1);
        }
    }

    private void InitializeWithItems() {

    }
}
