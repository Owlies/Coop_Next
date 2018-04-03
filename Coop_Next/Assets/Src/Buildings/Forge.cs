using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Forge : BuildingBase {
    private Canvas canvas;
    private List<ResourceEnum> resourceList;
    private Image[] resourceImages;
    public Sprite resourceEmptyImage;
    public Sprite resourceStoneImage;
    public Sprite resourceCoalImage;
    public Sprite resourceOreImage;
    public Sprite resourceWoodImage;

    private void Start()
    {
        canvas = GetComponentInChildren<Canvas>();
        resourceList = new List<ResourceEnum>();

        // [0] - Background image
        // [1 - 4] Resource images
        resourceImages = GetComponentsInChildren<Image>();
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

    public void StartForging() {

    }

    public void CancelForging() {

    }
}
