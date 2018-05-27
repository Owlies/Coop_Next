using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using UnityEngine.SceneManagement;

public class MetadataLoader : Singleton<MetadataLoader> {
	private SQLiteHelper sqlHelper;

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        SceneManager.sceneLoaded += this.Initialize;
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        SceneManager.sceneLoaded -= this.Initialize;
    }

    void Initialize(Scene scene, LoadSceneMode sceneMode) {
        sqlHelper = new SQLiteHelper();
		sqlHelper.InitializeDBConnection();
        LoadBuildingMetadata();
    }
	
	public List<EnemyMetadataDBObject> LoadEnemyMetadata() {
		List<EnemyMetadataDBObject> enemyList = new List<EnemyMetadataDBObject>();

        SqliteCommand cmd = sqlHelper.CreateTextCommand("SELECT * FROM enemy_config");
        SqliteDataReader reader = sqlHelper.ExecuteCommand(cmd);
        while (reader.Read()) {
            EnemyMetadataDBObject row = new EnemyMetadataDBObject(reader);
            enemyList.Add(row);
        }
        sqlHelper.CloseResultReader(reader);

		return enemyList;
	}

    public List<WaveEnemyConfigMetadataDBObject> LoadWaveEnemyConfigMeatadata() {
        List<WaveEnemyConfigMetadataDBObject> configList = new List<WaveEnemyConfigMetadataDBObject>();

        SqliteCommand cmd = sqlHelper.CreateTextCommand("SELECT * FROM wave_enemy_config");
        SqliteDataReader reader = sqlHelper.ExecuteCommand(cmd);

        while (reader.Read()) {
            WaveEnemyConfigMetadataDBObject row = new WaveEnemyConfigMetadataDBObject(reader);
            configList.Add(row);
        }
        sqlHelper.CloseResultReader(reader);

        return configList;
    }

    public List<BuildingMetadataDBObject> LoadBuildingMetadata() {
        List<BuildingMetadataDBObject> buildingList = new List<BuildingMetadataDBObject>();

        SqliteCommand cmd = sqlHelper.CreateTextCommand("SELECT * FROM building_metadata");
        SqliteDataReader reader = sqlHelper.ExecuteCommand(cmd);

        while (reader.Read()) {
            BuildingMetadataDBObject row = new BuildingMetadataDBObject(reader);
            buildingList.Add(row);
            Debug.Log(row.buildingName);
        }
        sqlHelper.CloseResultReader(reader);

        return buildingList;
    }
}
