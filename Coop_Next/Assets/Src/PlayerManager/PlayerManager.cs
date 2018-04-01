using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : Singleton<PlayerManager> {
    public GameObject playerPrefab;
    private List<PlayerController> players = new List<PlayerController>();

	// Use this for initialization
	void Start () {
        //TODO(Huayu): Hook up with GameManager

        SceneManager.sceneLoaded += OnSceneLoaded;

    }

    void OnSceneLoaded(Scene scence, LoadSceneMode mod)
    {
        Initialize(1);
    }

    public void Initialize(int numberOfPlayers) {
        for (int playerId = 0; playerId < numberOfPlayers; playerId++) {
            GameObject playerObject = GameObject.Instantiate(playerPrefab) as GameObject;
            playerObject.AddComponent<PlayerController>();
            PlayerController pController = playerObject.GetComponent<PlayerController>();

#if UNITY_EDITOR || UNITY_STANDALONE
            playerObject.AddComponent<PCInputController>();
            playerObject.GetComponent<PCInputController>().RegisterListeners(pController.PlayerMove, pController.CancelPlayerMovement, pController.PlayerAction);
            pController.Initialize(playerObject.GetComponent<PCInputController>(), playerId);
#elif UNITY_IOS || UNITY_ANDROID
            
#endif
            players.Add(pController);
            InitializePlayerLocation(playerObject);
        }
    }

    void InitializePlayerLocation(GameObject player) {
        if (!AppConstant.Instance.isMultiPlayer) {
            GameObject singlePlayerStartPoint = GameObject.FindWithTag("SinglePlayerStartPoint");
            player.transform.position = new Vector3(singlePlayerStartPoint.transform.position.x, singlePlayerStartPoint.transform.position.y, singlePlayerStartPoint.transform.position.z);
            return;
        }

        PlayerController pController = player.GetComponent<PlayerController>();
        if (pController.IsFirstPlayer()) {
            GameObject player1StartPoint = GameObject.FindWithTag("Player1StartPoint");
            player.transform.position = new Vector3(player1StartPoint.transform.position.x, player1StartPoint.transform.position.y, player1StartPoint.transform.position.z);
            return;
        }

        GameObject player2StartPoint = GameObject.FindWithTag("Player2StartPoint");
        player.transform.position = new Vector3(player2StartPoint.transform.position.x, player2StartPoint.transform.position.y, player2StartPoint.transform.position.z);
    }
}
