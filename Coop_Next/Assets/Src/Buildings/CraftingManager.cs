using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : Singleton<CraftingManager> {
    public LevelConfig levelConfig;
    public GameObject[] fixedCrafts;
    public int availableSlotsCount = 2;
    public double slotRefreshSeconds = 10.0f;

    private List<GameObject> unlockedCrafts;
    private List<GameObject> currentAvailableCrafts;
    private Dictionary<int, double> slotIndexRefreshStartTimeMap;

    private HashSet<GameObject> tmpEligibleCrafts;

    // Use this for initialization
    void Start() {
        tmpEligibleCrafts = new HashSet<GameObject>();
        slotIndexRefreshStartTimeMap = new Dictionary<int, double>();
        unlockedCrafts = new List<GameObject>();
        currentAvailableCrafts = new List<GameObject>();

        for (int i = 0; i < fixedCrafts.Length; i++) {
            unlockedCrafts.Add(fixedCrafts[i]);
            assignSlotWithCraft(i, fixedCrafts[i]);
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

        GameObject selectedCraft = selectEligibleCraft();
        assignSlotWithCraft(slotIndex, selectedCraft);
    }

    private void assignSlotWithCraft(int slotIndex, GameObject selectedCraft) {
        slotIndexRefreshStartTimeMap[slotIndex] = Time.time;

        if (currentAvailableCrafts.Count <= slotIndex) {
            currentAvailableCrafts.Add(selectedCraft);
            return;
        }

        currentAvailableCrafts[slotIndex] = selectedCraft;
    }

    private GameObject selectEligibleCraft() {
        tmpEligibleCrafts.Clear();
        foreach (GameObject craft in unlockedCrafts) {
            tmpEligibleCrafts.Add(craft);
        }

        foreach (GameObject craft in fixedCrafts) {
            tmpEligibleCrafts.Remove(craft);
        }

        int selectedIndex = Random.Range(0, tmpEligibleCrafts.Count);
        GameObject [] tmpArray = new GameObject[tmpEligibleCrafts.Count];
        tmpEligibleCrafts.CopyTo(tmpArray);
         
        return tmpArray[selectedIndex];
    }

}
