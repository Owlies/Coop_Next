using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager> {
    private EventCenter eventCenter;
    private PlayerManager playerManager;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void InitializeServices() {
        eventCenter = EventCenter.Instance;
        playerManager = PlayerManager.Instance;
    }

}
