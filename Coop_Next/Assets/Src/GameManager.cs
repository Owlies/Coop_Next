using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager> {

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        SceneManager.sceneLoaded += this.OnSceneLoaded;
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        SceneManager.sceneLoaded -= this.OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode) {
        // Don't initialize server for main scene
        if (scene.name.Equals("main") || scene.name.Equals("Start")) {
            return;
        }

        InitializeServices();
    }

    // Use this for initialization
    void Start ()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.LoadScene("Start", LoadSceneMode.Additive);
    }

    void InitializeServices() {
         Debug.Log("InitializeServices");

        MetadataLoader.Instance.Initialize();
        MetadataManager.Instance.Initialize();

        CraftingManager.Instance.Initialize();
        CraftingUIManager.Instance.Initialize();

        //TechTreeManager.Instance.Initialize();

        MapManager.Instance.Initialize();
        EnemyManager.Instance.Initialize();

        // TODO(Huayu): Hook up with real player number
        PlayerManager.Instance.Initialize(1);
    }

}
