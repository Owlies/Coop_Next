using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager> {
    public int coalQuantity;
    public int oreQuantity;
    public int woodQuantity;
    public int rockQuantity;

    private Dictionary<PlayerController, Resource> collectingMap;

    private bool canStartCollecting(PlayerController player, Resource resource) {
        if (collectingMap[player] != null) {
            return false;
        }

        foreach (KeyValuePair<PlayerController, Resource> entry in collectingMap) {
            if (entry.Value == resource) {
                return false;
            }
        }

        return true;
    }

    public bool startCollecting(PlayerController player, Resource resource) {
        if (!canStartCollecting(player, resource)) {
            return false;
        }

        collectingMap[player] = resource;

        return true;
    }

    private bool canCancelCollecting(PlayerController player, Resource resource) {
        if (collectingMap[player] == null) {
            return false;
        }

        return true;
    }

    public bool cancelCollecting(PlayerController player, Resource resource) {
        if (!canCancelCollecting(player, resource)) {
            return false;
        }

        collectingMap.Remove(player);

        return true;
    }

    private bool canCompleteCollecting(PlayerController player, Resource resource) {
        if (collectingMap[player] == null)
        {
            return false;
        }

        bool foundResource = false;
        foreach (KeyValuePair<PlayerController, Resource> entry in collectingMap)
        {
            if (entry.Value == resource)
            {
                foundResource = true;
                break;
            }
        }

        return foundResource;
    }

    public bool completeCollecting(PlayerController player, Resource resource) {
        if (!canCompleteCollecting(player, resource)) {
            return false;
        }

        collectingMap.Remove(player);
        switch (resource.resourceEnum) {
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
}
