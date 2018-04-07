using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProgressBar;

public enum ResourceEnum {Stone, Wood, Ore, Coal}

public class Resource : InteractionReceiver {
    public ResourceEnum resourceEnum;

    private void Start()
    {
        if (GetComponentInChildren<ProgressBarBehaviour>() != null) {
            GetComponentInChildren<ProgressBarBehaviour>().enabled = false;
        }
    }

    public bool isBasicResource() {
        return resourceEnum == ResourceEnum.Stone ||
            resourceEnum == ResourceEnum.Ore ||
            resourceEnum == ResourceEnum.Coal ||
            resourceEnum == ResourceEnum.Wood;
    }

}
