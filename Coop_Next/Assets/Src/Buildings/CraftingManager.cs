using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : Singleton<CraftingManager> {
    public LevelConfig levelConfig;
    public GameObject[] fixedCrafts;
    public int availableSlotsCount = 2;
    public float slotRefreshSeconds = 2.0f;

    public List<InteractiveObject> currentAvailableCrafts;

    private List<InteractiveObject> unlockedCrafts;
    private Dictionary<int, double> slotIndexRefreshStartTimeMap;

    private HashSet<InteractiveObject> tmpEligibleCrafts;

    // Use this for initialization
    public void Initialize() {
        tmpEligibleCrafts = new HashSet<InteractiveObject>();
        slotIndexRefreshStartTimeMap = new Dictionary<int, double>();
        unlockedCrafts = new List<InteractiveObject>();
        currentAvailableCrafts = new List<InteractiveObject>();

        for (int i = 0; i < levelConfig.initialUnlockedBuildings.Length; i++) {
            unlockedCrafts.Add(levelConfig.initialUnlockedBuildings[i].GetComponent<InteractiveObject>());
        }

        for (int i = 0; i < fixedCrafts.Length; i++) {
            InteractiveObject item = fixedCrafts[i].GetComponent<InteractiveObject>();
            if (item == null) {
                Debug.LogError("Mising InteractiveItem Component");
                continue;
            }

            AssignSlotWithCraft(i, fixedCrafts[i].GetComponent<InteractiveObject>());
        }
    }

    // Update is called once per frame
    public override void UpdateMe() {
        TryRefreshAvailableCrafts();
    }

    private void TryRefreshAvailableCrafts() {
        for (int i = fixedCrafts.Length; i < availableSlotsCount; i++) {
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

        InteractiveObject selectedCraft = SelectEligibleCraft();
        AssignSlotWithCraft(slotIndex, selectedCraft);

        slotIndexRefreshStartTimeMap[slotIndex] = Time.time;
        CraftingUIManager.Instance.UpdateCraftIcon(slotIndex);
    }

    private void AssignSlotWithCraft(int slotIndex, InteractiveObject selectedCraft) {
        if (selectedCraft == null) {
            return;
        }

        if (currentAvailableCrafts.Count <= slotIndex) {
            currentAvailableCrafts.Add(selectedCraft);
            return;
        }

        currentAvailableCrafts[slotIndex] = selectedCraft;
    }

    private InteractiveObject SelectEligibleCraft() {
        tmpEligibleCrafts.Clear();
        foreach (InteractiveObject craft in unlockedCrafts) {
            int count = MapManager.Instance.GetNumberOfItemsOnMapWithName(craft.name);
            if (count >= craft.maxAllowed) {
                continue;
            }
            tmpEligibleCrafts.Add(craft);
        }


        foreach (GameObject craft in fixedCrafts) {
            InteractiveObject item = craft.GetComponent<InteractiveObject>();
            if (item == null) {
                Debug.LogError("Mising InteractiveItem Component");
                continue;
            }
            tmpEligibleCrafts.Remove(item);
        }

        if (tmpEligibleCrafts.Count == 0) {
            return null;
        }

        int selectedIndex = Random.Range(0, tmpEligibleCrafts.Count);
        InteractiveObject[] tmpArray = new InteractiveObject[tmpEligibleCrafts.Count];
        tmpEligibleCrafts.CopyTo(tmpArray);
         
        return tmpArray[selectedIndex];
    }

    public double GetSlotRefreshStartTime(int slotIndex) {
        if (!slotIndexRefreshStartTimeMap.ContainsKey(slotIndex)) {
            return 0.0f;
        }

        return slotIndexRefreshStartTimeMap[slotIndex];
    }

    public bool IsCraftAvailable(InteractiveObject craft) {
        foreach (InteractiveObject availableCraft in currentAvailableCrafts) {
            if (craft == availableCraft) {
                return true;
            }
        }

        return false;
    }

    public void UnlockCraft(InteractiveObject craft) {
        unlockedCrafts.Add(craft);
    }

    public void UpgradeCraft(InteractiveObject craft) {
        List < InteractiveObject > newList = new List<InteractiveObject>();
        foreach (InteractiveObject obj in unlockedCrafts) {
            if (obj.techTreeId == craft.techTreeId) {
                continue;
            }
            newList.Add(craft);
        }

        unlockedCrafts = newList;
    }

    public List<InteractiveObject> GetUnlockedCrafts() {
        return unlockedCrafts;
    }
}
