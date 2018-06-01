using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProgressBar;

public class HpBarBehaviour : OverridableMonoBehaviour {

	private ProgressBarBehaviour hpProgressBar;
	private Canvas hpBarCanvas;
	// Use this for initialization
	void Start () {
		hpProgressBar = GetComponentInChildren<ProgressBarBehaviour>();
		if (hpProgressBar == null) {
			return;
		}
		
        hpProgressBar.Value = 100.0f;
        hpProgressBar.TransitoryValue = 0.0f;
        hpProgressBar.ProgressSpeed = 1000;

		foreach(Canvas canvas in GetComponents<Canvas>()) {
			if (canvas.gameObject.name.Equals("HpCanvas")) {
				canvas.enabled = false;
				hpBarCanvas = canvas;
				break;
			}
		}
	}
	
	public void UpdateHpBar(float currentHp, float maxHp) {
		if (hpBarCanvas == null || hpProgressBar == null) {
			return;
		}

		if (!hpBarCanvas.enabled) {
			hpBarCanvas.enabled = true;
		}

		hpProgressBar.Value = 100.0f * (currentHp / maxHp);
	}
}
