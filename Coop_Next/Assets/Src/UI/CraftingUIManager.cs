using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using ProgressBar;

public class CraftingUIManager : Singleton<CraftingUIManager> {
    public RectTransform craftingUIPanel;
    public MetadataManager metadataManager;

    private List<RectTransform> iconPanels;
    private List<string> displayingItemNames;
    private Dictionary<RectTransform, float> iconProgressMap;

    // Use this for initialization
    public void Initialize() {
        metadataManager = MetadataManager.Instance;
        iconPanels = new List<RectTransform>();
        displayingItemNames = new List<string>();
        iconProgressMap = new Dictionary<RectTransform, float>();
        Transform[] childs = Util.Instance.GetFirstLayerChildComponents(craftingUIPanel);
        foreach (Transform obj in childs) {
            iconPanels.Add((RectTransform)(obj));
            obj.gameObject.SetActive(false);
            obj.GetComponentInChildren<ProgressBarBehaviour>().ProgressSpeed = 1000;
            iconProgressMap[(RectTransform)(obj)] = 0.0f;
        }

        // Enable fixed crafting slots with disabled progress bar
        for (int i = 0; i < CraftingManager.Instance.fixedCrafts.Count; i++) {
            iconPanels[i].gameObject.SetActive(true);
            iconPanels[i].GetComponentInChildren<ProgressBarBehaviour>().gameObject.SetActive(false);
            UpdateCraftIcon(i);
        }
    }

    public override void UpdateMe() {
        UpdateCraftProgressBars();
    }

    private bool CanUpdateCraftIcon(int slotIndex) {
        if (slotIndex >= CraftingManager.Instance.currentAvailableCrafts.Count) {
            return false;
        }

        return true;
    }

    public void UpdateCraftIcon(int slotIndex) {
        if (!CanUpdateCraftIcon(slotIndex)) {
            Debug.LogError("updateCraftIcon out of index");
            return;
        }

        RectTransform iconPanel = iconPanels[slotIndex];
        ResetProgressBar(iconPanel);

        iconPanel.gameObject.SetActive(true);
        InteractiveObject item = CraftingManager.Instance.currentAvailableCrafts[slotIndex];
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

        // Forth element is the recipe icon panel
        UpdateCraftRecipeIcons(iconPanel.GetComponentsInChildren<RectTransform>()[4], item);
    }

    private void UpdateCraftRecipeIcons(RectTransform recipePanel, InteractiveObject item) {
        ObjectMetadata objectData = metadataManager.objectsDictionary[item.itemId];
        RecipeMetadata selectedRecipe = objectData.recipe;
        Image[] recipeIcons = recipePanel.GetComponentsInChildren<Image>();

        for (int i = 0; i < recipeIcons.Length; i++) {
            Resource resource = metadataManager.GetResourceByType(selectedRecipe.resources[i]);
            if (resource != null)
                recipeIcons[i].sprite = resource.iconImage;
        }
    }

    /*      Progress Bar Updates    */
    private void UpdateCraftProgressBars() {
        for (int i = CraftingManager.Instance.fixedCrafts.Count; i < CraftingManager.Instance.availableSlotsCount; i++) {
            UpdateCraftIconProgressBar(i, iconPanels[i]);
        }
    }

    private void UpdateCraftIconProgressBar(int slotIndex, RectTransform iconPanel) {
        ProgressBarBehaviour progressBar = iconPanel.GetComponentInChildren<ProgressBarBehaviour>();
        iconProgressMap[iconPanel] += Time.deltaTime * 100.0f / CraftingManager.Instance.slotRefreshSeconds;
        progressBar.Value = iconProgressMap[iconPanel];
    }

    private void ResetProgressBar(RectTransform iconPanel) {
        ProgressBarBehaviour progressBar = iconPanel.GetComponentInChildren<ProgressBarBehaviour>();
        if (progressBar == null || !progressBar.enabled) {
            return;
        }
        progressBar.Value = 0.0f;
        progressBar.TransitoryValue = 0.0f;
        iconProgressMap[iconPanel] = 0.0f;
    }
}
