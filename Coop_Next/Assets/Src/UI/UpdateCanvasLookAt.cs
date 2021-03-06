﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateCanvasLookAt : OverridableMonoBehaviour {
    public Canvas canvas;
    public bool canvasEnableOnStart;
    // Use this for initialization
    void Start () {
        if (canvas == null)
            canvas = GetComponentInChildren<Canvas>();
        if(canvas == null) {
            return;
        }
        canvas.worldCamera = Camera.main;
        canvas.enabled = canvasEnableOnStart;
    }

    // Update is called once per frame
    public override void LateUpdateMe()
    {
        if (canvas == null) {
            return;
        }
        // canvas.transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
        canvas.transform.rotation = Camera.main.transform.rotation;
    }
}
