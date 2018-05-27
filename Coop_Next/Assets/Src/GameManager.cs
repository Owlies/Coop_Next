using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager> {
    private EventCenter eventCenter;
    private PlayerManager playerManager;

    // Use this for initialization
    void Start ()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.LoadScene("Level1", LoadSceneMode.Additive);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void InitializeServices() {
        eventCenter = EventCenter.Instance;
        playerManager = PlayerManager.Instance;
    }

}
