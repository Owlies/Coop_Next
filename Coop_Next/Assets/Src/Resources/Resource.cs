using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResourceEnum {Rock, Wood, Ore, Coal}

public class Resource : MonoBehaviour {
    public ResourceEnum resourceEnum;

    private void Start()
    {
        Canvas canvas = GetComponentInChildren<Canvas>();
        canvas.worldCamera = Camera.main;
    }
}
