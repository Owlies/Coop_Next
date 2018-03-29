using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProgressBar;

public enum ResourceEnum {Rock, Wood, Ore, Coal}

public class Resource : OverridableMonoBehaviour {
    public ResourceEnum resourceEnum;

    private void Start()
    {
        Canvas canvas = GetComponentInChildren<Canvas>();
        canvas.worldCamera = Camera.main;
        canvas.enabled = false;

        ProgressBarBehaviour progressBar = GetComponentInChildren<ProgressBarBehaviour>();
        progressBar.enabled = false;
    }

    public override void UpdateMe() {
        //transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.back, Camera.main.transform.rotation * Vector3.down);
    }
}
