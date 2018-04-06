using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProgressBar;



public class Forge : CollectableBuilding {
    private enum ForgeState { IDLE, FORGING }

    private Canvas receiptCanvas;
    private Canvas progressBarCanvas;
    private Canvas destroyProgressBarCanvas;
    private ProgressBarBehaviour forgingProgressBar;
    private ProgressBarBehaviour destroyProgressBar;
    private List<ResourceEnum> resourceList;
    private Image[] resourceImages;

    public Sprite resourceEmptyImage;
    public Sprite resourceStoneImage;
    public Sprite resourceCoalImage;
    public Sprite resourceOreImage;
    public Sprite resourceWoodImage;
    public ObjectConfig objectConfig;

    private ForgeState forgeState;
    private float curForgingProgress = 0.0f;
    private GameObject forgedPrefab;

    private void Start()
    {
        receiptCanvas = GetComponentsInChildren<Canvas>()[0];
        progressBarCanvas = GetComponentsInChildren<Canvas>()[1];
        destroyProgressBarCanvas = GetComponentsInChildren<Canvas>()[2];

        progressBarCanvas.worldCamera = Camera.main;
        destroyProgressBarCanvas.worldCamera = Camera.main;
        forgingProgressBar = progressBarCanvas.GetComponentInChildren<ProgressBarBehaviour>();
        destroyProgressBar = destroyProgressBarCanvas.GetComponentInChildren<ProgressBarBehaviour>();

        resourceList = new List<ResourceEnum>();

        // [0] - Background image
        // [1 - 4] Resource images
        // [5 - ] Progress bar images
        resourceImages = GetComponentsInChildren<Image>();

        if (forgingProgressBar != null)
        {
            forgingProgressBar.enabled = false;
            progressBarCanvas.enabled = false;
        }

        if (destroyProgressBar != null)
        {
            destroyProgressBar.enabled = false;
            destroyProgressBarCanvas.enabled = false;
        }
    }

    public override void UpdateMe()
    {
        base.UpdateMe();
        if (forgeState == ForgeState.FORGING) {
            curForgingProgress += Time.deltaTime;
            forgingProgressBar.Value = curForgingProgress * 100.0f / AppConstant.Instance.forgingTime;
            if (curForgingProgress >= AppConstant.Instance.forgingTime) {
                ForgingComplete();
            }
        }
    }
    public override void LateUpdateMe()
    {
        base.LateUpdateMe();
        progressBarCanvas.transform.rotation = Camera.main.transform.rotation;
    }

    private bool CanAddResourceToForge(Resource resource) {
        if (resource == null) {
            return false;
        }

        if (resourceList.Count < 3 && !resource.isBasicResource()) {
            return false;
        }

        if (resourceList.Count == 3 && resource.isBasicResource()) {
            return false;
        }

        if (resourceList.Count >= 4) {
            return false;
        }

        return true;
    }

    public void AddResourceToForge(GameObject player, GameObject resourceCube) {
        Resource resource = resourceCube.GetComponent<Resource>();
        Debug.Log("AddResourceToForge");
        if (!CanAddResourceToForge(resource)) {
            return;
        }

        switch (resource.resourceEnum) {
            case ResourceEnum.Coal:
                resourceList.Add(ResourceEnum.Coal);
                resourceImages[resourceList.Count].sprite = resourceCoalImage;
                break;
            case ResourceEnum.Ore:
                resourceList.Add(ResourceEnum.Ore);
                resourceImages[resourceList.Count].sprite = resourceOreImage;
                break;
            case ResourceEnum.Stone:
                resourceList.Add(ResourceEnum.Stone);
                resourceImages[resourceList.Count].sprite = resourceStoneImage;
                break;
            case ResourceEnum.Wood:
                resourceList.Add(ResourceEnum.Wood);
                resourceImages[resourceList.Count].sprite = resourceWoodImage;
                break;
        }

        player.GetComponent<PlayerController>().OnAddResourceToForgeComplete();

        StartForgeOrDestroy();

        return;
    }

    private GameObject FindMatchingReceiptObject() {
        foreach (ObjectData data in objectConfig.objects) {
            if (data.receipts.Length > 0) {
                foreach (Receipt receipt in data.receipts) {
                    bool doesMatch = true;
                    for (int i = 0; i < receipt.resources.Length; i++) {
                        if (i == resourceList.Count) {
                            doesMatch = false;
                            break;
                        }

                        if (resourceList[i] != receipt.resources[i]) {
                            doesMatch = false;
                            break;
                        }
                    }

                    if (doesMatch) {
                        return data.prefab;
                    }
                }
            }
        }

        return null;
    }

    private void StartForgeOrDestroy() {
        if (FindMatchingReceiptObject() != null)
        {
            StartForging();
            return;
        }

        DestroyForging();
    }

    public void StartForging() {
        receiptCanvas.enabled = false;
        progressBarCanvas.enabled = true;
        forgingProgressBar.enabled = true;
        forgeState = ForgeState.FORGING;
        curForgingProgress = 0.0f;
    }

    private void ForgingComplete() {
        forgedPrefab = FindMatchingReceiptObject();
        forgeState = ForgeState.IDLE;
        ResetForgingProgressBar();
        resourceList.Clear();
        ResetResourceImages();

        //TODO(Huayu):Ready to collect UI
    }

    private void ResetResourceImages() {
        for(int i = 1; i <= 4; i++) {
            resourceImages[i].sprite = resourceEmptyImage;
        }
    }

    private void ResetForgingProgressBar()
    {
        curForgingProgress = 0.0f;
        forgingProgressBar.enabled = false;
        forgingProgressBar.Value = 0.0f;
        forgingProgressBar.TransitoryValue = 0.0f;
        progressBarCanvas.enabled = false;
    }

    public void DestroyForging() {
        
    }

    private bool CanCollectItem() {
        if (forgedPrefab == null) {
            return false;
        }

        return true;
    }

    public override bool CollectItem(GameObject player)
    {
        if (!CanCollectItem()) {
            return false;
        }

        GameObject forgedBuilding = GameObject.Instantiate(forgedPrefab, player.transform);
        player.GetComponent<PlayerController>().SetCarryingItem(forgedBuilding);
        forgedPrefab = null;
        receiptCanvas.enabled = true;

        return true;
    }
}
