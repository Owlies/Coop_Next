﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProgressBar;

public class ResourceManager : Singleton<ResourceManager> {
    public GameObject coalCubePrefab;
    public GameObject oreCubePrefab;
    public GameObject woodCubePrefab;
    public GameObject rockCubePrefab;

    private Dictionary<Player, GameObject> collectingMap = new Dictionary<Player, GameObject>();
    private Dictionary<GameObject, float> collectingProgressMap = new Dictionary<GameObject, float>();

    private bool CanStartCollecting(Player player, GameObject resource) {
        if (collectingMap.ContainsKey(player)) {
            return false;
        }

        if (collectingProgressMap.ContainsKey(resource)) {
            return false;
        }

        foreach (KeyValuePair<Player, GameObject> entry in collectingMap) {
            if (entry.Value == resource) {
                return false;
            }
        }

        return true;
    }

    public bool StartCollecting(Player player, GameObject resource) {
        if (!CanStartCollecting(player, resource)) {
            return false;
        }

        collectingMap[player] = resource;
        collectingProgressMap[resource] = 0;

        Debug.Log("startCollecting");

        EnableProgressBar(resource, true);

        return true;
    }

    private bool CanCancelCollecting(Player player, GameObject resource) {
        if (!collectingMap.ContainsKey(player)) {
            return false;
        }

        Debug.Log("canCancelCollecting");

        return true;
    }

    public bool CancelCollecting(Player player, GameObject resource) {
        if (!CanCancelCollecting(player, resource)) {
            return false;
        }

        CleanMap(player, resource);

        return true;
    }

    private bool CanCompleteCollecting(Player player, GameObject resource) {
        if (!collectingMap.ContainsKey(player))
        {
            return false;
        }

        bool foundResource = false;
        foreach (KeyValuePair<Player, GameObject> entry in collectingMap)
        {
            if (entry.Value == resource)
            {
                foundResource = true;
                break;
            }
        }

        return foundResource;
    }

    public bool CompleteCollecting(Player player, GameObject resource) {
        if (!CanCompleteCollecting(player, resource)) {
            return false;
        }
        Debug.Log("completeCollecting");
        ResourceBuilding resourceType = resource.GetComponent<ResourceBuilding>();
        
        GameObject cube = null;
        switch (resourceType.resourceEnum) {
            case ResourceEnum.Coal:
                cube = GameObject.Instantiate(coalCubePrefab, player.transform);
                break;
            case ResourceEnum.Ore:
                cube = GameObject.Instantiate(oreCubePrefab, player.transform);
                break;
            case ResourceEnum.Rock:
                cube = GameObject.Instantiate(rockCubePrefab, player.transform);
                break;
            case ResourceEnum.Wood:
                cube = GameObject.Instantiate(woodCubePrefab, player.transform);
                break;
        }

        cube.transform.SetPositionAndRotation(cube.transform.position + player.transform.forward * 2.0f, cube.transform.rotation);

        InteractiveObject item = cube.GetComponent<InteractiveObject>();
        player.GetComponent<Player>().SetCarryingItem(item);

        CleanMap(player, resource);

        return true;
    }

    private void CleanMap(Player player, GameObject resource) {
        collectingMap.Remove(player);
        collectingProgressMap.Remove(resource);

        EnableProgressBar(resource, false);
    }

    private void EnableProgressBar(GameObject resource, bool enable) {
        ProgressBarBehaviour progressBar = resource.GetComponentInChildren<ProgressBarBehaviour>();
        progressBar.enabled = enable;
        progressBar.Value = 0.0f;
        progressBar.ProgressSpeed = 1000;
        progressBar.TransitoryValue = 0.0f;
        resource.GetComponentInChildren<Canvas>().enabled = enable;
    }

    public override void UpdateMe() {

        foreach (KeyValuePair<Player, GameObject> entry in collectingMap)
        {
            ProgressBarBehaviour progressBar = entry.Value.GetComponentInChildren<ProgressBarBehaviour>();
            collectingProgressMap[entry.Value] += Time.deltaTime * 100.0f / AppConstant.Instance.resourceCollectingSeconds;
            progressBar.Value = collectingProgressMap[entry.Value];
        }
        
    }
}
