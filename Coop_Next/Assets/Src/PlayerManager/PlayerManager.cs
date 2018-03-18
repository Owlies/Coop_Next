﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : Singleton<PlayerManager> {
    public GameObject playerPrefab;
    private List<PlayerController> players;

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(gameObject);
        SceneManager.LoadSceneAsync("Level1", LoadSceneMode.Additive);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void initialize(int numberOfPlayers) {
        for (int playerId = 0; playerId < numberOfPlayers; playerId++) {
#if UNITY_EDITOR || UNITY_STANDALONE
            PlayerController pc = new PlayerController(new PCInputController(), playerId);
#elif UNITY_IOS || UNITY_ANDROID
            
#endif
            players.Add(pc);
        }

    }
}
