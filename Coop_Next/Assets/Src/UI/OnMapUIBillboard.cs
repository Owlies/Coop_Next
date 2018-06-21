using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnMapUIBillboard : OverridableMonoBehaviour {
    Text title;
    Text content;

	// Use this for initialization
	void Start () {
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
}
