using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProgressBar;

public class Forge : BuildingBase {
    private enum ForgeState { IDLE, FORGING, DESTROYING, READY_TO_COLLECT }

    private Canvas recipeCanvas;
    private Canvas progressBarCanvas;
    private Canvas destroyProgressBarCanvas;
    private ProgressBarBehaviour forgingProgressBar;
    private ProgressBarBehaviour destroyProgressBar;
    private List<Resource> resourceList;
    private Image[] resourceImages;

    public Sprite resourceEmptyImage;
    public MetadataManager metadataManager;

    private ForgeState forgeState;
    private float curProgress = 0.0f;
    private GameObject forgedGameObject;

    private void Start() {
        recipeCanvas = GetComponentsInChildren<Canvas>()[0];
        progressBarCanvas = GetComponentsInChildren<Canvas>()[1];
        destroyProgressBarCanvas = GetComponentsInChildren<Canvas>()[2];

        progressBarCanvas.worldCamera = Camera.main;
        destroyProgressBarCanvas.worldCamera = Camera.main;
        forgingProgressBar = progressBarCanvas.GetComponentInChildren<ProgressBarBehaviour>();
        destroyProgressBar = destroyProgressBarCanvas.GetComponentInChildren<ProgressBarBehaviour>();

        resourceList = new List<Resource>();

        // [0] - Background image
        // [1 - 4] Resource images
        // [5 - ] Progress bar images
        resourceImages = GetComponentsInChildren<Image>();

        if (forgingProgressBar != null) {
            forgingProgressBar.enabled = false;
            progressBarCanvas.enabled = false;
        }

        if (destroyProgressBar != null) {
            destroyProgressBar.enabled = false;
            destroyProgressBarCanvas.enabled = false;
        }

        metadataManager = MetadataManager.Instance;
    }

    public new void Awake()
    {
        base.Awake();

    }

    public override void UpdateMe()
    {
        base.UpdateMe();
        if (forgeState == ForgeState.FORGING)
        {
            curProgress += Time.deltaTime;
            forgingProgressBar.Value = curProgress * 100.0f / AppConstant.Instance.forgingTime;
            if (curProgress >= AppConstant.Instance.forgingTime)
            {
                ForgingComplete();
            }
        }
        else if (forgeState == ForgeState.DESTROYING) {
            curProgress += Time.deltaTime;
            destroyProgressBar.Value = curProgress * 100.0f / AppConstant.Instance.forgingTime;
            if (curProgress >= AppConstant.Instance.forgingTime)
            {
                DestroyForging();
            }
        }
    }
    public override void LateUpdateMe()
    {
        base.LateUpdateMe();
        progressBarCanvas.transform.rotation = Camera.main.transform.rotation;
        destroyProgressBarCanvas.transform.rotation = Camera.main.transform.rotation;
    }

    private bool CanAddResourceToForge(Player actor)
    {
        if (actor.GetPlayerActionState() != EPlayerActionState.CARRYING_RESOURCE)
        {
            return false;
        }

        if (actor.GetCarryingItem() == null) {
            return false;
        }

        Resource resource = actor.GetCarryingItem().GetComponent<Resource>();
        if (resource == null)
        {
            return false;
        }

        if (this.transform.gameObject.tag != "Forge")
        {
            return false;
        }

        if (forgeState == ForgeState.READY_TO_COLLECT)
        {
            return false;
        }

        if (resourceList.Count < 3 && !resource.isBasicResource())
        {
            return false;
        }

        if (resourceList.Count == 3 && resource.isBasicResource())
        {
            return false;
        }

        if (resourceList.Count >= 4)
        {
            return false;
        }

        return true;
    }

    private bool AddResourceToForge(Player actor) {
        if (!CanAddResourceToForge(actor))
        {
            return false;
        }

        Resource resource = actor.GetCarryingItem().GetComponent<Resource>();
        resourceList.Add(resource);
        Debug.Log("AddResourceToForge");

        resourceImages[resourceList.Count].sprite = resource.iconImage;

        OnAddResourceToForgeComplete(actor);

        StartForgeOrDestroy();

        return true;
    }

    public void OnAddResourceToForgeComplete(Player actor)
    {
        GameObject resource = actor.GetCarryingItem().gameObject;
        resource.SetActive(false);
        actor.UnsetCarryingItem();
        resource.transform.parent = transform;
    }

    private void StartForgeOrDestroy() {
        if (FindMatchingRecipeObject() != null)
        {
            StartForging();
            return;
        }

        StartDestroyForgingItems();
    }

    private void StartForging() {
        //recipeCanvas.enabled = false;
        ResetDestroyForgingProgressBar();
        ResetForgingProgressBar();
        progressBarCanvas.enabled = true;
        forgingProgressBar.enabled = true;
        forgeState = ForgeState.FORGING;
        curProgress = 0.0f;
    }

    private void ForgingComplete() {
        recipeCanvas.enabled = false;
        ObjectMetadata forgedPrefabMetadata = FindMatchingRecipeObject();
        forgedGameObject = forgedPrefabMetadata.GetGameObjectFromPool(transform);
        MapManager.Instance.OnItemCreated(forgedGameObject);
        forgedGameObject.SetActive(false);

        InteractiveObject item = forgedGameObject.GetComponent<InteractiveObject>();
        if (resourceList.Count == 4 && resourceList[3].isRareResource())
        {
            (resourceList[3] as Orb).applyOrbEffect(item);
        }

        forgeState = ForgeState.READY_TO_COLLECT;
        ResetForgingProgressBar();

        //TODO(Huayu):Ready to collect UI
    }

    private void StartDestroyForgingItems() {
        //recipeCanvas.enabled = false;
        ResetDestroyForgingProgressBar();
        ResetForgingProgressBar();
        destroyProgressBarCanvas.enabled = true;
        destroyProgressBar.enabled = true;

        forgeState = ForgeState.DESTROYING;
        curProgress = 0.0f;
    }

    public void DestroyForging() {
        forgeState = ForgeState.IDLE;
        ResetDestroyForgingProgressBar();
        ResetAndEnableRecipeCanvas();
    }

    private bool CanCollectItem(Player player) {
        if (forgedGameObject == null) {
            return false;
        }

        if (player.GetPlayerActionState() != EPlayerActionState.IDLE)
        {
            return false;
        }

        return true;
    }

    private bool CollectItem(Player player)
    {
        if (!CanCollectItem(player)) {
            return false;
        }

        forgedGameObject.SetActive(true);
        InteractiveObject item = forgedGameObject.GetComponent<InteractiveObject>();
        player.SetCarryingItem(item);
        forgedGameObject = null;

        forgeState = ForgeState.IDLE;
        ResetAndEnableRecipeCanvas();

        return true;
    }


    #region HelperFunctions
    private ObjectMetadata FindMatchingRecipeObject()
    {
        foreach (var obj in metadataManager.objectsDictionary)
        {
            ObjectMetadata data = obj.Value;
            if (!CraftingManager.Instance.IsCraftAvailable(data)) {
                continue;
            }

            RecipeMetadata recipe = MetadataLoader.Instance.GetRecipeMetadataById(data.recipeId);

            if (recipe!= null)
            {
                bool doesMatch = true;
                for (int i = 0; i < recipe.resources.Count; i++)
                {
                    if (i == resourceList.Count)
                    {
                        doesMatch = false;
                        break;
                    }

                    if (resourceList[i].resourceEnum != recipe.resources[i])
                    {
                        doesMatch = false;
                        break;
                    }
                }

                if (doesMatch)
                {
                    return data;
                }
            }
        }

        return null;
    }

    private void ResetResourceImages()
    {
        for (int i = 1; i <= 4; i++)
        {
            resourceImages[i].sprite = resourceEmptyImage;
        }
    }

    protected override void OnDestroy()
    { 
        for (int i = 0; i < resourceList.Count; i++)
            GameObject.DestroyImmediate(resourceList[i].gameObject);
        resourceList.Clear();
        base.OnDestroy();
    }

    private void ResetForgingProgressBar()
    {
        curProgress = 0.0f;
        forgingProgressBar.enabled = false;
        forgingProgressBar.Value = 0.0f;
        forgingProgressBar.TransitoryValue = 0.0f;
        progressBarCanvas.enabled = false;
    }

    private void ResetDestroyForgingProgressBar()
    {
        curProgress = 0.0f;
        destroyProgressBar.enabled = false;
        destroyProgressBar.Value = 0.0f;
        destroyProgressBar.TransitoryValue = 0.0f;
        destroyProgressBarCanvas.enabled = false;
    }

    private void ResetAndEnableRecipeCanvas()
    {
        for (int i = 0; i < resourceList.Count; i++) {
            MapManager.Instance.OnItemDestroyed(resourceList[i].gameObject);
            GameObject.Destroy(resourceList[i].gameObject);
        }
            
        resourceList.Clear();
        ResetResourceImages();
        recipeCanvas.enabled = true;
    }

    #endregion

    public override bool InteractAction(Player actor)
    {
        if (AddResourceToForge(actor)) {
            return true;
        }

        if (CollectItem(actor)) {
            return true;
        }

        return false;
    }
}
