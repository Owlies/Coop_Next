using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : Singleton<CraftingManager> {
    public LevelConfig levelConfig;

    public string[] fixedCraftTechTreeId;
    [HideInInspector]
    public List<ObjectMetadata> fixedCrafts;
    public int availableSlotsCount = 2;
    public float slotRefreshSeconds = 2.0f;

    public List<ObjectMetadata> currentAvailableCrafts;

    private List<ObjectMetadata> unlockedCrafts;
    private Dictionary<int, double> slotIndexRefreshStartTimeMap;

    private HashSet<ObjectMetadata> tmpEligibleCrafts;

    // Use this for initialization
    public void Initialize() {
        tmpEligibleCrafts = new HashSet<ObjectMetadata>();
        slotIndexRefreshStartTimeMap = new Dictionary<int, double>();
        unlockedCrafts = new List<ObjectMetadata>();
        currentAvailableCrafts = new List<ObjectMetadata>();

        for (int i = 0; i < levelConfig.initialUnlockedBuildings.Count; i++) {
            unlockedCrafts.Add(levelConfig.initialUnlockedBuildings[i]);
        }

        TechTreeManager.Instance.Initialize();

        fixedCrafts = new List<ObjectMetadata>();
        for (int i = 0; i < fixedCraftTechTreeId.Length; ++i)
        {
            var buildingMetadata = MetadataManager.Instance.GetBuildingMetadataWithTechTreeId(fixedCraftTechTreeId[i]);
            fixedCrafts.Add(buildingMetadata);
        }

        for (int i = 0; i < fixedCrafts.Count; i++)
        {
            AssignSlotWithCraft(i, fixedCrafts[i]);
        }
    }

    // Update is called once per frame
    public override void UpdateMe() {
        TryRefreshAvailableCrafts();
    }

    private void TryRefreshAvailableCrafts() {
        for (int i = fixedCrafts.Count; i < availableSlotsCount; i++) {
            RefreshCraftSlot(i);
        }
    }

    private bool CanRefreshCraftSlot(int slotIndex) {
        if (slotIndexRefreshStartTimeMap.ContainsKey(slotIndex)) {
            double startTime = slotIndexRefreshStartTimeMap[slotIndex];
            if (Time.time - startTime < slotRefreshSeconds) {
                return false;
            }
        }

        return true;
    }

    private void RefreshCraftSlot(int slotIndex) {
        if (!CanRefreshCraftSlot(slotIndex)) {
            return;
        }

        ObjectMetadata selectedCraft = SelectEligibleCraft();
        AssignSlotWithCraft(slotIndex, selectedCraft);

        slotIndexRefreshStartTimeMap[slotIndex] = Time.time;
        CraftingUIManager.Instance.UpdateCraftIcon(slotIndex);
    }

    private void AssignSlotWithCraft(int slotIndex, ObjectMetadata selectedCraft) {
        if (selectedCraft == null) {
            return;
        }

        if (currentAvailableCrafts.Count <= slotIndex) {
            currentAvailableCrafts.Add(selectedCraft);
            return;
        }

        currentAvailableCrafts[slotIndex] = selectedCraft;
    }

    private ObjectMetadata SelectEligibleCraft() {
        tmpEligibleCrafts.Clear();
        foreach (ObjectMetadata craft in unlockedCrafts) {
            int count = MapManager.Instance.GetNumberOfItemsOnMapWithName(craft.objectName);
            if (count >= craft.maxAllowed) {
                continue;
            }
            tmpEligibleCrafts.Add(craft);
        }


        foreach (ObjectMetadata craft in fixedCrafts) {
            tmpEligibleCrafts.Remove(craft);
        }

        if (tmpEligibleCrafts.Count == 0) {
            return null;
        }

        int selectedIndex = Random.Range(0, tmpEligibleCrafts.Count);
        ObjectMetadata[] tmpArray = new ObjectMetadata[tmpEligibleCrafts.Count];
        tmpEligibleCrafts.CopyTo(tmpArray);
         
        return tmpArray[selectedIndex];
    }

    public double GetSlotRefreshStartTime(int slotIndex) {
        if (!slotIndexRefreshStartTimeMap.ContainsKey(slotIndex)) {
            return 0.0f;
        }

        return slotIndexRefreshStartTimeMap[slotIndex];
    }

    public bool IsCraftAvailable(ObjectMetadata craft) {
        foreach (ObjectMetadata availableCraft in currentAvailableCrafts) {
            if (craft == availableCraft) {
                return true;
            }
        }

        return false;
    }

    public void UnlockCraft(ObjectMetadata craft) {
        unlockedCrafts.Add(craft);
    }

    public void UpgradeCraft(ObjectMetadata craft) {
        List <ObjectMetadata> newList = new List<ObjectMetadata>();
        foreach (ObjectMetadata obj in unlockedCrafts) {
            if (obj.techTreeId == craft.techTreeId) {
                continue;
            }
            newList.Add(craft);
        }

        unlockedCrafts = newList;
    }

    public List<ObjectMetadata> GetUnlockedCrafts() {
        return unlockedCrafts;
    }
}
