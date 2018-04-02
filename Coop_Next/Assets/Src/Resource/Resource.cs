﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProgressBar;

public enum ResourceEnum {Stone, Wood, Ore, Coal}

public class Resource : OverridableMonoBehaviour {
    public ResourceEnum resourceEnum;

    private void Start()
    {
        if (GetComponentInChildren<ProgressBarBehaviour>() != null) {
            GetComponentInChildren<ProgressBarBehaviour>().enabled = false;
        }
    }

    //public override void UpdateMe()
    //{
    //    canvas.transform.rotation = Camera.main.transform.rotation;
    //}

}
