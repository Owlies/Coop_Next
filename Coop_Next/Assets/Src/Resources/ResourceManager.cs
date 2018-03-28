using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProgressBar;

public class ResourceManager : Singleton<ResourceManager> {
    public int coalQuantity;
    public int oreQuantity;
    public int woodQuantity;
    public int rockQuantity;
    public GameObject coalCubePrefab;
    public GameObject oreCubePrefab;
    public GameObject woodCubePrefab;
    public GameObject rockCubePrefab;

    private Dictionary<PlayerController, GameObject> collectingMap = new Dictionary<PlayerController, GameObject>();

    private bool canStartCollecting(PlayerController player, GameObject resource) {
        if (collectingMap.ContainsKey(player)) {
            return false;
        }

        foreach (KeyValuePair<PlayerController, GameObject> entry in collectingMap) {
            if (entry.Value == resource) {
                return false;
            }
        }

        return true;
    }

    public bool startCollecting(PlayerController player, GameObject resource) {
        if (!canStartCollecting(player, resource)) {
            return false;
        }

        collectingMap[player] = resource;

        ProgressBarBehaviour progressBar = resource.GetComponentInChildren<ProgressBarBehaviour>();
        progressBar.enabled = true;

        return true;
    }

    private bool canCancelCollecting(PlayerController player, GameObject resource) {
        if (!collectingMap.ContainsKey(player)) {
            return false;
        }

        return true;
    }

    public bool cancelCollecting(PlayerController player, GameObject resource) {
        if (!canCancelCollecting(player, resource)) {
            return false;
        }

        collectingMap.Remove(player);

        return true;
    }

    private bool canCompleteCollecting(PlayerController player, GameObject resource) {
        if (!collectingMap.ContainsKey(player))
        {
            return false;
        }

        bool foundResource = false;
        foreach (KeyValuePair<PlayerController, GameObject> entry in collectingMap)
        {
            if (entry.Value == resource)
            {
                foundResource = true;
                break;
            }
        }

        return foundResource;
    }

    public bool completeCollecting(PlayerController player, GameObject resource) {
        if (!canCompleteCollecting(player, resource)) {
            return false;
        }

        Resource resourceType = resource.GetComponent<Resource>();
        collectingMap.Remove(player);
        switch (resourceType.resourceEnum) {
            case ResourceEnum.Coal:
                coalQuantity++;
                break;
            case ResourceEnum.Ore:
                oreQuantity++;
                break;
            case ResourceEnum.Rock:
                rockQuantity++;
                break;
            case ResourceEnum.Wood:
                woodQuantity++;
                break;
        }

        return true;
    }

    public override void UpdateMe() {

        foreach (KeyValuePair<PlayerController, GameObject> entry in collectingMap)
        {
            ProgressBarBehaviour progressBar = entry.Value.GetComponentInChildren<ProgressBarBehaviour>();
            progressBar.Value += Time.deltaTime;
        }
        
    }
}
