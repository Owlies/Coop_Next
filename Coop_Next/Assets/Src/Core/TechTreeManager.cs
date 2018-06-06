using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechTreeManager : Singleton<TechTreeManager> {
    private Dictionary<string, int> techTreeLevelMap;

	// Use this for initialization
	void Start () {
        InitializeWithBuildings();
    }

    private void InitializeWithBuildings() {
        techTreeLevelMap = new Dictionary<string, int>();
        List<BuildingMetadataDBObject> buildingMetadataList = MetadataLoader.Instance.LoadBuildingMetadata();
        foreach (BuildingMetadataDBObject obj in buildingMetadataList) {
            if (techTreeLevelMap.ContainsKey(obj.techTreeId)) {
                continue;
            }

            techTreeLevelMap.Add(obj.techTreeId, 1);
        }
    }

    private void InitializeWithItems() {

    }

    public bool UpgreadeItem(InteractiveItem item) {
        if (!techTreeLevelMap.ContainsKey(item.techTreeId)) {
            return false;
        }

        techTreeLevelMap[item.techTreeId] = techTreeLevelMap[item.techTreeId] + 1;
        return true;
    }

    public bool UnlockItem(InteractiveItem item) {
        if (techTreeLevelMap.ContainsKey(item.techTreeId)) {
            return false;
        }

        techTreeLevelMap[item.techTreeId] = 1;
        return true;
    }

    public int GetItemLevel(InteractiveItem item) {
        if (!techTreeLevelMap.ContainsKey(item.techTreeId)) {
            return 0;
        }

        return techTreeLevelMap[item.techTreeId];
    }
}
