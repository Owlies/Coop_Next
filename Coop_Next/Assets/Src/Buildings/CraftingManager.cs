using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : Singleton<CraftingManager> {
    public LevelConfig levelConfig;
    public GameObject[] fixedCrafts;
    public int availableSlotsCount = 2;
    public float slotRefreshSeconds = 2.0f;

    public List<InteractiveItem> currentAvailableCrafts;

    private List<InteractiveItem> unlockedCrafts;
    private Dictionary<int, double> slotIndexRefreshStartTimeMap;

    private HashSet<InteractiveItem> tmpEligibleCrafts;

    // Use this for initialization
    void Start() {
        tmpEligibleCrafts = new HashSet<InteractiveItem>();
        slotIndexRefreshStartTimeMap = new Dictionary<int, double>();
        unlockedCrafts = new List<InteractiveItem>();
        currentAvailableCrafts = new List<InteractiveItem>();

        for (int i = 0; i < levelConfig.initialUnlockedBuildings.Length; i++) {
            unlockedCrafts.Add(levelConfig.initialUnlockedBuildings[i].GetComponent<InteractiveItem>());
        }

        for (int i = 0; i < fixedCrafts.Length; i++) {
            InteractiveItem item = fixedCrafts[i].GetComponent<InteractiveItem>();
            if (item == null) {
                Debug.LogError("Mising InteractiveItem Component");
                continue;
            }

            AssignSlotWithCraft(i, fixedCrafts[i].GetComponent<InteractiveItem>());
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

        InteractiveItem selectedCraft = SelectEligibleCraft();
        AssignSlotWithCraft(slotIndex, selectedCraft);

        slotIndexRefreshStartTimeMap[slotIndex] = Time.time;
        CraftingUIManager.Instance.UpdateCraftIcon(slotIndex);
    }

    private void AssignSlotWithCraft(int slotIndex, InteractiveItem selectedCraft) {
        if (selectedCraft == null) {
            return;
        }

        if (currentAvailableCrafts.Count <= slotIndex) {
            currentAvailableCrafts.Add(selectedCraft);
            return;
        }

        currentAvailableCrafts[slotIndex] = selectedCraft;
    }

    private InteractiveItem SelectEligibleCraft() {
        tmpEligibleCrafts.Clear();
        foreach (InteractiveItem craft in unlockedCrafts) {
            tmpEligibleCrafts.Add(craft);
        }


        foreach (GameObject craft in fixedCrafts) {
            InteractiveItem item = craft.GetComponent<InteractiveItem>();
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
        InteractiveItem[] tmpArray = new InteractiveItem[tmpEligibleCrafts.Count];
        tmpEligibleCrafts.CopyTo(tmpArray);
         
        return tmpArray[selectedIndex];
    }

    public double GetSlotRefreshStartTime(int slotIndex) {
        if (!slotIndexRefreshStartTimeMap.ContainsKey(slotIndex)) {
            return 0.0f;
        }

        return slotIndexRefreshStartTimeMap[slotIndex];
    }

    public bool IsCraftAvailable(InteractiveItem craft) {
        foreach (InteractiveItem availableCraft in currentAvailableCrafts) {
            if (craft == availableCraft) {
                return true;
            }
        }

        return false;
    }
}
