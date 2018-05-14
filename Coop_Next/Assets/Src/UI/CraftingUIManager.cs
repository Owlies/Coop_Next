using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CraftingUIManager : Singleton<CraftingUIManager> {
    public RectTransform craftingUIPanel;
    public ObjectConfig objectConfig;

    private List<RectTransform> iconPanels;
    private List<string> displayingItemNames;

	// Use this for initialization
	void Start () {
        iconPanels = new List<RectTransform>();
        displayingItemNames = new List<string>();
        Transform[] childs = Util.Instance.GetFirstLayerChildComponents(craftingUIPanel);
        foreach (Transform obj in childs) {
            iconPanels.Add((RectTransform)(obj));
        }
    }

    public override void UpdateMe() {
        updateCraftDisplay();
    }

    private void updateCraftDisplay() {
        int i = 0;
        while (i < CraftingManager.Instance.currentAvailableCrafts.Count) {
            updateCraftIcon(i, iconPanels[i++]);
        }

        while (i < iconPanels.Count) {
            iconPanels[i].gameObject.SetActive(false);
            i++;
        }
    }

    private bool canUpdateCraftIcon(int slotIndex) {
        if (slotIndex >= CraftingManager.Instance.currentAvailableCrafts.Count) {
            return false;
        }

        return true;
    }

    private void updateCraftIcon(int slotIndex, RectTransform iconPanel) {
        if (!canUpdateCraftIcon(slotIndex)) {
            Debug.LogError("updateCraftIcon out of index");
            return;
        }

        InteractiveItem item = CraftingManager.Instance.currentAvailableCrafts[slotIndex];
        if (displayingItemNames.Count <= slotIndex) {
            displayingItemNames.Add(item.name);
        } else {
            displayingItemNames[slotIndex] = item.name;
        }

        // Second image is CraftIcon
        Image craftIcon = iconPanel.GetComponentsInChildren<Image>()[1];
        craftIcon.sprite = item.iconImage;

        Text craftName = iconPanel.GetComponentInChildren<Text>();
        craftName.text = item.name;

        // Forth element is the receipt icon panel
        updateCraftReceiptIcons(iconPanel.GetComponentsInChildren<RectTransform>()[4], item);
    }

    private void updateCraftReceiptIcons(RectTransform receiptPanel, InteractiveItem item) {
        ObjectData objectData = objectConfig.objectsDictionary[item.name];
        Receipt selectedReceipt = objectData.receipts[Random.Range(0, objectData.receipts.Length)];
        Image[] receiptIcons = receiptPanel.GetComponentsInChildren<Image>();

        for (int i = 0; i < receiptIcons.Length; i++) {
            receiptIcons[i].sprite = objectConfig.resourceEnumToItemMap[selectedReceipt.resources[i]].iconImage;
        }
    }
}
