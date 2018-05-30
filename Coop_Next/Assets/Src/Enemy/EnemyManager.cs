using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager> {
    public LevelConfig levelConfig;
    public float waveInterval;
    public GameObject[] enemySpawnLocations;

    private int currentWave;
    private int aliveEnemyQuantity;
    private float waveIntervalTimer = 0.0f;
    private GameObject targetGameObject;
    private List<EnemyBase> allEnemies;
    private Dictionary<int, WaveEnemyConfigMetadataDBObject> waveConfigDictionary;
    private Dictionary<string, EnemyMetadataDBObject> enemyConfigDictionary;
    // Use this for initialization
    void Start () {
        currentWave = 1;
        aliveEnemyQuantity = 0;
        targetGameObject = GameObject.FindGameObjectWithTag("Forge");

        allEnemies = new List<EnemyBase>();
        InitializeWaveConfigDictionary();
        InitializeEnemyConfigDictionary();
    }

    private void InitializeWaveConfigDictionary() {
        waveConfigDictionary = new Dictionary<int, WaveEnemyConfigMetadataDBObject>();
        List<WaveEnemyConfigMetadataDBObject> configList = MetadataLoader.Instance.LoadWaveEnemyConfigMeatadata();
        if (configList == null) {
            Debug.LogError("Failed to InitializeWaveConfigDictionary");
            return;
        }

        foreach(WaveEnemyConfigMetadataDBObject config in configList) {
            waveConfigDictionary[config.waveNumber] = config;
        }
    }

    public string GetKeyForEnemyConfig(int waveNumber, EnemyTypeEnum enemyType) {
        return waveNumber + "_" + enemyType.ToString();
    }

    private void InitializeEnemyConfigDictionary() {
        enemyConfigDictionary = new Dictionary<string, EnemyMetadataDBObject>();
        List<EnemyMetadataDBObject> enemyList = MetadataLoader.Instance.LoadEnemyMetadata();
        if (enemyList == null) {
            Debug.LogError("Failed to InitializeWaveEnemyConfigDictionary");
            return;
        }

        foreach(EnemyMetadataDBObject enemyConfig in enemyList) {
            enemyConfigDictionary[GetKeyForEnemyConfig(enemyConfig.waveNumber, enemyConfig.enemyType)] = enemyConfig;
        }
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
        SpawnEnemyForWaveWithType(EnemyTypeEnum.AVERAGE);
        SpawnEnemyForWaveWithType(EnemyTypeEnum.ATTACK);
        SpawnEnemyForWaveWithType(EnemyTypeEnum.DEFEND);
        SpawnEnemyForWaveWithType(EnemyTypeEnum.SMALL_BOSS);
        SpawnEnemyForWaveWithType(EnemyTypeEnum.BIG_BOSS);

        currentWave++;
    }

    private void SpawnEnemyForWaveWithType(EnemyTypeEnum type) {
        WaveEnemyConfigMetadataDBObject waveConfig = waveConfigDictionary[currentWave];
        int enemyQuantity = 0;

        switch(type) {
            case EnemyTypeEnum.AVERAGE:
                enemyQuantity = waveConfig.averageEnemyQuantity;
                break;
            case EnemyTypeEnum.ATTACK:
                enemyQuantity = waveConfig.attackEnemyQuantity;
                break;
            case EnemyTypeEnum.DEFEND:
                enemyQuantity = waveConfig.defendEnemyQuantity;
                break;
            case EnemyTypeEnum.SMALL_BOSS:
                enemyQuantity = waveConfig.smallBossQuantity;
                break;
            case EnemyTypeEnum.BIG_BOSS:
                enemyQuantity = waveConfig.bigBossQuantity;
                break;

        }

        if (enemyQuantity == 0) {
            return;
        }
        
        aliveEnemyQuantity += enemyQuantity * enemySpawnLocations.Length;

        string key = GetKeyForEnemyConfig(currentWave, type);
        if(!enemyConfigDictionary.ContainsKey(key)) {
            Debug.LogError("Can't find " + key + " config in enemyConfigDictionary");
        }
        EnemyMetadataDBObject config = enemyConfigDictionary[key];

        for (int i = 0; i < enemySpawnLocations.Length; i++) {
            SpawnEnemyWithConfig(enemySpawnLocations[i], enemyQuantity, config);
            break;
        }
        
    }

    private void SpawnEnemyWithConfig(GameObject spawnLocation, int quantity, EnemyMetadataDBObject config) {
        GameObject enemyPrefab = levelConfig.enemyPrefabs[Random.Range(0, levelConfig.enemyPrefabs.Length)];
        for(int i = 0; i < quantity; i++) {
            SpawnEnemyAtPosition(enemyPrefab, spawnLocation.transform.position, config);
        }
    }

    public void OnEnemyKilled(EnemyBase enemy) {
        allEnemies.Remove(enemy);
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

    private void SpawnEnemyAtPosition(GameObject enemyPrefab, Vector3 pos, EnemyMetadataDBObject config) {
        GameObject enemyObject = GameObject.Instantiate(enemyPrefab, pos, Quaternion.identity);
        EnemyBase enemy = enemyObject.GetComponent<EnemyBase>();
        if (enemy == null) {
            Debug.Log("ERROR: No EnemyBase Component");
            DestroyImmediate(enemyObject);
            return;
        }

        allEnemies.Add(enemy);
        enemy.Initialize(currentWave, config, targetGameObject);
    }

    #endregion
}
