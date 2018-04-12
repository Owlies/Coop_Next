using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager> {
    private int currentWave;

    public float waveInterval;
    public GameObject[] enemySpawnLocations;

	// Use this for initialization
	void Start () {
        currentWave = 1;
    }

    public void StartNextWave() {

    }
}
