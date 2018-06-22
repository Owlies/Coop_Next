using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnMapUIBillboard : OverridableMonoBehaviour {
    Text title;
    Text content;
    Canvas canvas;

	// Use this for initialization
	void Start () {
        canvas = GetComponentInChildren<Canvas>();
        title = GetComponentsInChildren<Text>()[0];
        content = GetComponentsInChildren<Text>()[1];
    }

    public void Initialize(string titleString, string contentString) {
        UpdateTitle(titleString);
        UpdateContent(contentString);
    }

    public void UpdateTitle(string titleString) {
        title.text = titleString;
    }

    public void UpdateContent(string contentString) {
        content.text = contentString;
    }

    public void Hide(bool hide) {
        canvas.gameObject.SetActive(!hide);
    }
}
