using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TechTreeManager : Singleton<TechTreeManager> {
    private Dictionary<string, int> techTreeLevelMap;

    public void Initialize() {
        InitializeWithBuildings();
    }

    private void InitializeWithBuildings() {
        techTreeLevelMap = new Dictionary<string, int>();
        List<BuildingMetadataDBObject> buildingMetadataList = MetadataLoader.Instance.GetBuildingMetadata();
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

    public int GetItemLevel(string techTreeId) {
        if (!techTreeLevelMap.ContainsKey(techTreeId)) {
            return 0;
        }

        return techTreeLevelMap[techTreeId];
    }
}
