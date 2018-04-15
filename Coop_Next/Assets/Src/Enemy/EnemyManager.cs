using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager> {
    public LevelConfig levelConfig;
    public float waveInterval;
    public GameObject[] enemySpawnLocations;
    public int firstWaveEnemyCount = 2;
    public int enemyCountIncreaseBetweenWaves = 1;
    public int maxEnemyCount = 10;
    public float enemyHPIncreasePercentage = 1.2f;

    private int currentWave;
    private int aliveEnemyQuantity;
    private float waveIntervalTimer = 0.0f;
    private Vector3 targetPosition;
    private List<EnemyBase> allEnemies = new List<EnemyBase>();

    // Use this for initialization
    void Start () {
        currentWave = 1;
        aliveEnemyQuantity = 0;
        targetPosition = GameObject.FindGameObjectWithTag("Forge").transform.position;
    }

    private bool CanStartNextWave() {
        if (aliveEnemyQuantity > 0) {
            return false;
        }

        if (waveIntervalTimer < waveInterval) {
            return false;
        }

        return true;
    }

    public void StartNextWave() {
        int enemyQuantity = GetNumberOfEnemiesForCurrentWave();
        if (enemyQuantity > maxEnemyCount) {
            enemyQuantity = maxEnemyCount;
        }

        aliveEnemyQuantity = enemyQuantity * enemySpawnLocations.Length;

        for (int i = 0; i < enemySpawnLocations.Length; i++) {
            SpawnRandomEnemyWithQuantity(enemyQuantity, enemySpawnLocations[i]);
        }

        currentWave++;
    }

    public override void UpdateMe() {
        if (CanStartNextWave())
        {
            StartNextWave();
        }
        else if (aliveEnemyQuantity == 0){
            waveIntervalTimer += Time.deltaTime;
        }
    }

    public void OnEnemyKilled() {
        aliveEnemyQuantity--;
        if (aliveEnemyQuantity == 0) {
            OnWaveClear();
        }
    }

    private void OnWaveClear() {
        waveIntervalTimer = 0.0f;
        // TODO(Huayu): Leave it there for future usage
    }

    #region HelperFunctions

    public List<EnemyBase> GetAllAliveEnemies() {
        return allEnemies;
    }

    private int GetNumberOfEnemiesForCurrentWave() {
        return firstWaveEnemyCount + enemyCountIncreaseBetweenWaves * (currentWave - 1);
    }

    private void SpawnRandomEnemyWithQuantity(int quantity, GameObject spawnLocation) {
        for (int i = 0; i < quantity; i++) {
            SpawnAnRandomEnemy(spawnLocation);
        }
    }

    private void SpawnAnRandomEnemy(GameObject spawnLocation) {
        GameObject enemyPrefab = levelConfig.enemyPrefabs[Random.Range(0, levelConfig.enemyPrefabs.Length)];
        SpawnEnemyAtPosition(enemyPrefab, spawnLocation.transform.position);
    }

    private void SpawnEnemyAtPosition(GameObject enemyPrefab, Vector3 pos) {
        GameObject enemyObject = GameObject.Instantiate(enemyPrefab, pos, Quaternion.identity);
        EnemyBase enemy = enemyObject.GetComponent<EnemyBase>();
        if (enemy == null) {
            Debug.Log("ERROR: No EnemyBase Component");
            DestroyImmediate(enemyObject);
            return;
        }

        allEnemies.Add(enemy);
        enemy.Initialize(currentWave, enemyHPIncreasePercentage, targetPosition);
    }

    #endregion
}
