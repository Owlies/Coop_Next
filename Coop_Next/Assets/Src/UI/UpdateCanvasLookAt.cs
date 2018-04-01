using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateCanvasLookAt : OverridableMonoBehaviour {
    private Canvas canvas;
    public bool canvasEnableOnStart;
    // Use this for initialization
    void Start () {
        canvas = GetComponentInChildren<Canvas>();
        canvas.worldCamera = Camera.main;
        canvas.enabled = canvasEnableOnStart;
    }

    // Update is called once per frame
    public override void LateUpdateMe()
    {
        //canvas.transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
        canvas.transform.rotation = Camera.main.transform.rotation;
    }
}
