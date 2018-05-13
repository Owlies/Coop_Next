using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : Singleton<CraftingManager> {
    public LevelConfig levelConfig;
    public GameObject[] fixedCrafts;
    public int availableSlotsCount = 2;
    public double slotRefreshSeconds = 10.0f;

    private List<InteractiveItem> unlockedCrafts;
    private List<InteractiveItem> currentAvailableCrafts;
    private Dictionary<int, double> slotIndexRefreshStartTimeMap;

    private HashSet<InteractiveItem> tmpEligibleCrafts;

    // Use this for initialization
    void Start() {
        tmpEligibleCrafts = new HashSet<InteractiveItem>();
        slotIndexRefreshStartTimeMap = new Dictionary<int, double>();
        unlockedCrafts = new List<InteractiveItem>();
        currentAvailableCrafts = new List<InteractiveItem>();

        for (int i = 0; i < fixedCrafts.Length; i++) {
            InteractiveItem item = fixedCrafts[i].GetComponent<InteractiveItem>();
            if (item == null) {
                Debug.LogError("Mising InteractiveItem Component");
                continue;
            }
            unlockedCrafts.Add(item);
            assignSlotWithCraft(i, fixedCrafts[i].GetComponent<InteractiveItem>());
        }

        tryRefreshAvailableCrafts();
    }

    // Update is called once per frame
    public override void UpdateMe() {
        base.UpdateMe();
        tryRefreshAvailableCrafts();
    }

    private void tryRefreshAvailableCrafts() {
        for (int i = fixedCrafts.Length; i < availableSlotsCount; i++) {
            refreshCraftSlot(i);
        }
    }

    private bool canRefreshCraftSlot(int slotIndex) {
        if (!slotIndexRefreshStartTimeMap.ContainsKey(slotIndex)) {
            return false;
        }

        double startTime = slotIndexRefreshStartTimeMap[slotIndex];
        if (Time.time - startTime < slotRefreshSeconds) {
            return false;
        }

        return true;
    }

    private void refreshCraftSlot(int slotIndex) {
        if (!canRefreshCraftSlot(slotIndex)) {
            return;
        }

        InteractiveItem selectedCraft = selectEligibleCraft();
        assignSlotWithCraft(slotIndex, selectedCraft);
    }

    private void assignSlotWithCraft(int slotIndex, InteractiveItem selectedCraft) {
        if (selectedCraft == null) {
            return;
        }

        slotIndexRefreshStartTimeMap[slotIndex] = Time.time;

        if (currentAvailableCrafts.Count <= slotIndex) {
            currentAvailableCrafts.Add(selectedCraft);
            return;
        }

        currentAvailableCrafts[slotIndex] = selectedCraft;
    }

    private InteractiveItem selectEligibleCraft() {
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

        int selectedIndex = Random.Range(0, tmpEligibleCrafts.Count);
        InteractiveItem[] tmpArray = new InteractiveItem[tmpEligibleCrafts.Count];
        tmpEligibleCrafts.CopyTo(tmpArray);
         
        return tmpArray[selectedIndex];
    }

}
