using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDetection : OverridableMonoBehaviour {
    private Player player;
	// Use this for initialization
	void Start () {
        player = GetComponentInParent<Player>();
	}

    private void OnTriggerEnter(Collider other) {
        player.AddNearbyInteractiveGameObject(other.gameObject);
    }

    private void OnTriggerExit(Collider other) {
        player.RemoveNearbyInteractiveGameObject(other.gameObject);
    }
}
