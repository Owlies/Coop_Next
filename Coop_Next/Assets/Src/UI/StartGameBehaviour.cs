using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameBehaviour : MonoBehaviour {

    public void StartGame()
    {
        SceneManager.UnloadSceneAsync("Start");
        SceneManager.LoadScene("Level1", LoadSceneMode.Additive);
    }
}
