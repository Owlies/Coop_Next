using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProgressBar;

public enum ResourceEnum {Stone, Wood, Ore, Coal}

public class Resource : OverridableMonoBehaviour {
    public ResourceEnum resourceEnum;
    private Canvas canvas;
    private void Start()
    {
        canvas = GetComponentInChildren<Canvas>();
        canvas.worldCamera = Camera.main;
        canvas.enabled = false;

        GetComponentInChildren<ProgressBarBehaviour>().enabled = false;
    }

    public override void UpdateMe() {
        //canvas.transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
    }
}
