using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProgressBar;

public class ResourceManager : Singleton<ResourceManager> {
    public GameObject coalCubePrefab;
    public GameObject oreCubePrefab;
    public GameObject woodCubePrefab;
    public GameObject stoneCubePrefab;

    private Dictionary<PlayerController, GameObject> collectingMap = new Dictionary<PlayerController, GameObject>();
    private Dictionary<GameObject, float> collectingProgressMap = new Dictionary<GameObject, float>();

    private bool canStartCollecting(PlayerController player, GameObject resource) {
        if (collectingMap.ContainsKey(player)) {
            return false;
        }

        if (collectingProgressMap.ContainsKey(resource)) {
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
        collectingProgressMap[resource] = 0;

        Debug.Log("startCollecting");

        enableProgressBar(resource, true);

        return true;
    }

    private bool canCancelCollecting(PlayerController player, GameObject resource) {
        if (!collectingMap.ContainsKey(player)) {
            return false;
        }

        Debug.Log("canCancelCollecting");

        return true;
    }

    public bool cancelCollecting(PlayerController player, GameObject resource) {
        if (!canCancelCollecting(player, resource)) {
            return false;
        }

        cleanMap(player, resource);

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
        Debug.Log("completeCollecting");
        Resource resourceType = resource.GetComponent<Resource>();
        
        GameObject cube = null;
        switch (resourceType.resourceEnum) {
            case ResourceEnum.Coal:
                cube = GameObject.Instantiate(coalCubePrefab, player.transform);
                break;
            case ResourceEnum.Ore:
                cube = GameObject.Instantiate(oreCubePrefab, player.transform);
                break;
            case ResourceEnum.Stone:
                cube = GameObject.Instantiate(stoneCubePrefab, player.transform);
                break;
            case ResourceEnum.Wood:
                cube = GameObject.Instantiate(woodCubePrefab, player.transform);
                break;
        }

        cube.transform.SetPositionAndRotation(cube.transform.position + Vector3.forward * 2.0f, cube.transform.rotation);
        player.GetComponent<PlayerController>().SetCarryingResourceCube(cube);

        cleanMap(player, resource);

        return true;
    }

    private void cleanMap(PlayerController player, GameObject resource) {
        collectingMap.Remove(player);
        collectingProgressMap.Remove(resource);

        enableProgressBar(resource, false);
    }

    private void enableProgressBar(GameObject resource, bool enable) {
        resource.GetComponentInChildren<ProgressBarBehaviour>().enabled = enable;
        resource.GetComponentInChildren<ProgressBarBehaviour>().Value = 0.0f;
        resource.GetComponentInChildren<ProgressBarBehaviour>().TransitoryValue = 0.0f;
        resource.GetComponentInChildren<Canvas>().enabled = enable;
    }

    public override void UpdateMe() {

        foreach (KeyValuePair<PlayerController, GameObject> entry in collectingMap)
        {
            ProgressBarBehaviour progressBar = entry.Value.GetComponentInChildren<ProgressBarBehaviour>();
            collectingProgressMap[entry.Value] += Time.deltaTime * 100.0f / AppConstant.Instance.resourceCollectingSeconds;
            progressBar.Value = collectingProgressMap[entry.Value];
        }
        
    }
}
