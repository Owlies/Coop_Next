using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TechTreeManager : Singleton<TechTreeManager> {
    private Dictionary<string, int> techTreeLevelMap;

    public void Initialize() {
        techTreeLevelMap = new Dictionary<string, int>();

        InitializeWithBuildings();
    }

    //private void InitializeWithBuildings() {
    //    techTreeLevelMap = new Dictionary<string, int>();
    //    List<BuildingMetadata> buildingMetadataList = MetadataLoader.Instance.GetBuildingMetadata();
    //    foreach (BuildingMetadata obj in buildingMetadataList) {
    //        if (techTreeLevelMap.ContainsKey(obj.techTreeId)) {
    //            continue;
    //        }

    //        techTreeLevelMap.Add(obj.techTreeId, 1);
    //    }
    //}

    private void InitializeWithBuildings() {
        List<ObjectMetadata> unlockedCrafts = CraftingManager.Instance.GetUnlockedCrafts();
        foreach (ObjectMetadata craft in unlockedCrafts) {
            techTreeLevelMap.Add(craft.techTreeId, 1);
        }
    }

    private void InitializeWithItems() {

    }

    public bool UpgreadeItem(ObjectMetadata item) {
        if (!techTreeLevelMap.ContainsKey(item.techTreeId)) {
            return false;
        }

        techTreeLevelMap[item.techTreeId] = techTreeLevelMap[item.techTreeId] + 1;
        CraftingManager.Instance.UpgradeCraft(item);
        return true;
    }

    public bool UnlockItem(ObjectMetadata item) {
        if (techTreeLevelMap.ContainsKey(item.techTreeId)) {
            return false;
        }

        techTreeLevelMap[item.techTreeId] = 1;
        CraftingManager.Instance.UnlockCraft(item);
        return true;
    }

    public int GetItemLevel(InteractiveObject item) {
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
