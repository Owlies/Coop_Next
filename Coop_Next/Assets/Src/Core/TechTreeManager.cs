using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TechTreeManager : Singleton<TechTreeManager> {
    private Dictionary<string, int> techTreeLevelMap;
    private Dictionary<string, int> subTypeLevelMap;

    public void Initialize() {
        techTreeLevelMap = new Dictionary<string, int>();
        subTypeLevelMap = new Dictionary<string, int>();

        InitializeWithBuildings();
    }

    public Dictionary<string, int> GetTechTreeLevelMap()
    {
        return techTreeLevelMap;
    }

    public Dictionary<string, int> GetSubTypeLevelMap()
    {
        return subTypeLevelMap;
    }

    private void InitializeWithBuildings() {
        List<ObjectMetadata> unlockedCrafts = CraftingManager.Instance.GetUnlockedCrafts();
        foreach (ObjectMetadata craft in unlockedCrafts) {
            techTreeLevelMap.Add(craft.techTreeId, 1);
        }

        foreach (string subType in Enum.GetNames(typeof(ObjectSubType))) {
            if (subType.Equals("FunctionalBuilding")
                || subType.Equals("EquipmentItem")
                || subType.Equals("ResourceItem")
                || subType.Equals("None"))
                continue;
            subTypeLevelMap.Add(subType, 1);
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
        subTypeLevelMap[subTypeToUpgrade.ToString()]++;
        int newLevel = subTypeLevelMap[subTypeToUpgrade.ToString()];
        foreach (ObjectMetadata metadata in CraftingManager.Instance.GetUnlockedCrafts()) {
            if (metadata.subType == subTypeToUpgrade) {
                if (techTreeLevelMap[metadata.techTreeId] < newLevel) {
                    techTreeLevelMap[metadata.techTreeId] = subTypeLevelMap[subTypeToUpgrade.ToString()];
                }
            }
        }
    }

    public bool UnlockItem(ObjectMetadata item) {
        if (techTreeLevelMap.ContainsKey(item.techTreeId)) {
            return false;
        }

        techTreeLevelMap[item.techTreeId] = subTypeLevelMap[item.subType.ToString()];
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
