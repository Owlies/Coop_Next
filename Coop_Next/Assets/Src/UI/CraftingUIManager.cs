using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingUIManager : Singleton<CraftingUIManager> {
    public RectTransform craftingUIPanel;

    private List<RectTransform> iconPanels;
	// Use this for initialization
	void Start () {
        iconPanels = new List<RectTransform>();
        Transform[] childs = Util.Instance.GetFirstLayerChildComponents(craftingUIPanel);
        foreach (Transform obj in childs) {
            iconPanels.Add((RectTransform)(obj));
        }
        Debug.Log("Start");
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
