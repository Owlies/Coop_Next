using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProgressBar;



public class Forge : BuildingBase {
    private enum ForgeState { IDLE, FORGING }

    private Canvas receiptCanvas;
    private Canvas progressBarCanvas;
    private ProgressBarBehaviour forgingProgressBar;
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
    private GameObject forgingPlayer;

    private void Start()
    {
        receiptCanvas = GetComponentInChildren<Canvas>();
        progressBarCanvas = GetComponentsInChildren<Canvas>()[1];
        progressBarCanvas.worldCamera = Camera.main;
        forgingProgressBar = progressBarCanvas.GetComponentInChildren<ProgressBarBehaviour>();

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

    private bool CanStartForging() {
        if (forgeState != ForgeState.IDLE) {
            return false;
        }

        if (forgingPlayer != null) {
            return false;
        }

        if (FindMatchingReceiptObject() == null) {
            return false;
        }

        return true;
    }

    public void StartForging(GameObject player) {
        if (!CanStartForging()) {
            return;
        }

        receiptCanvas.enabled = false;
        progressBarCanvas.enabled = true;
        forgingProgressBar.enabled = true;
        forgeState = ForgeState.FORGING;
        curForgingProgress = 0.0f;
        forgingPlayer = player;
    }

    private void ForgingComplete() {
        GameObject forgedPrefab = FindMatchingReceiptObject();

        GameObject forgedBuilding = GameObject.Instantiate(forgedPrefab, forgingPlayer.transform);
        forgingPlayer.GetComponent<PlayerController>().SetCarryingItem(forgedBuilding);

        forgeState = ForgeState.IDLE;
        curForgingProgress = 0.0f;
        forgingProgressBar.enabled = false;
        forgingProgressBar.Value = 0.0f;
        forgingProgressBar.TransitoryValue = 0.0f;
        forgingPlayer = null;
        receiptCanvas.enabled = true;
        progressBarCanvas.enabled = false;

        resourceList.Clear();
        ResetResourceImages();
    }

    private void ResetResourceImages() {
        for(int i = 1; i <= 4; i++) {
            resourceImages[i].sprite = resourceEmptyImage;
        }
    }

    public void CancelForging() {
        forgeState = ForgeState.IDLE;
    }

    public void DestroyForging() {
        
    }
}
