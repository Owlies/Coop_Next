using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TechTreeManager : Singleton<TechTreeManager> {
    private Dictionary<string, int> techTreeLevelMap;
    private Dictionary<int, int> subTypeLevelMap;

    public void Initialize() {
        techTreeLevelMap = new Dictionary<string, int>();
        subTypeLevelMap = new Dictionary<int, int>();

        InitializeWithBuildings();
    }

    public Dictionary<string, int> GetTechTreeLevelMap()
    {
        return techTreeLevelMap;
    }

    public Dictionary<int, int> GetSubTypeLevelMap()
    {
        return subTypeLevelMap;
    }

    private void InitializeWithBuildings() {
        List<ObjectMetadata> unlockedCrafts = CraftingManager.Instance.GetUnlockedCrafts();
        foreach (ObjectMetadata craft in unlockedCrafts) {
            techTreeLevelMap.Add(craft.techTreeId, 1);
        }

        foreach (ObjectSubType subType in Enum.GetValues(typeof(ObjectSubType))) {
            if (subType == ObjectSubType.FunctionalBuilding
                || subType == ObjectSubType.EquipmentItem
                || subType == ObjectSubType.ResourceItem
                || subType == ObjectSubType.None)
                continue;
            subTypeLevelMap.Add((int)subType, 1);
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

    public void UpgradeBuildingWithSubType(ObjectSubType subTypeToUpgrade) {
        subTypeLevelMap[(int)subTypeToUpgrade]++;
        int newLevel = subTypeLevelMap[(int)subTypeToUpgrade];
        foreach (ObjectMetadata metadata in CraftingManager.Instance.GetUnlockedCrafts()) {
            if (metadata.subType == subTypeToUpgrade) {
                if (techTreeLevelMap[metadata.techTreeId] < newLevel) {
                    techTreeLevelMap[metadata.techTreeId] = subTypeLevelMap[(int)subTypeToUpgrade];
                }
            }
        }
    }

    public bool UnlockItem(ObjectMetadata item) {
        if (subTypeLevelMap.ContainsKey((int)item.subType)) {
            return false;
        }

        techTreeLevelMap[item.techTreeId] = subTypeLevelMap[(int)item.subType];
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
