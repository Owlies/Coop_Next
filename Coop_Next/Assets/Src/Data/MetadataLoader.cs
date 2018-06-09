using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using UnityEngine.SceneManagement;

public class MetadataLoader : Singleton<MetadataLoader> {   
	private SQLiteHelper sqlHelper;
    List<BuildingMetadataDBObject> buildingList = null;
    List<WaveEnemyConfigMetadataDBObject> configList = null;
    List<ItemMetadataDBObject> itemList = null;
    List<EnemyMetadataDBObject> enemyList = null;
    List<RecipeMetadataDBObject> recipeList = null;

    public void Initialize() {
        sqlHelper = new SQLiteHelper();
		sqlHelper.InitializeDBConnection();

        //load all the metaData into game.
        //todo : do it separately and may do it when loading.
        GetEnemyMetadata();
        GetWaveEnemyConfigMeatadata();
        GetBuildingMetadata();
        GetItemMetadata();
        GetRecipeMetadata();
    }
	
	public List<EnemyMetadataDBObject> GetEnemyMetadata() {
        if (enemyList != null)
            return enemyList;
        enemyList = new List<EnemyMetadataDBObject>();

        SqliteCommand cmd = sqlHelper.CreateTextCommand("SELECT * FROM enemy_config");
        SqliteDataReader reader = sqlHelper.ExecuteCommand(cmd);
        while (reader.Read()) {
            EnemyMetadataDBObject row = new EnemyMetadataDBObject(reader);
            enemyList.Add(row);
        }
        sqlHelper.CloseResultReader(reader);

		return enemyList;
	}

    public List<WaveEnemyConfigMetadataDBObject> GetWaveEnemyConfigMeatadata() {
        if (configList != null)
            return configList;
        configList = new List<WaveEnemyConfigMetadataDBObject>();

        SqliteCommand cmd = sqlHelper.CreateTextCommand("SELECT * FROM wave_enemy_config");
        SqliteDataReader reader = sqlHelper.ExecuteCommand(cmd);

        while (reader.Read()) {
            WaveEnemyConfigMetadataDBObject row = new WaveEnemyConfigMetadataDBObject(reader);
            configList.Add(row);
        }
        sqlHelper.CloseResultReader(reader);

        return configList;
    }

    public List<BuildingMetadataDBObject> GetBuildingMetadata()
    {
        if (buildingList != null)
            return buildingList;
        buildingList = new List<BuildingMetadataDBObject>();

        SqliteCommand cmd = sqlHelper.CreateTextCommand("SELECT * FROM building_metadata");
        SqliteDataReader reader = sqlHelper.ExecuteCommand(cmd);

        while (reader.Read())
        {
            BuildingMetadataDBObject row = new BuildingMetadataDBObject(reader);
            buildingList.Add(row);
        }
        sqlHelper.CloseResultReader(reader);

        return buildingList;
    }

    public List<RecipeMetadataDBObject> GetItemMetadata()
    {
        if (recipeList != null)
            return recipeList;
        recipeList = new List<RecipeMetadataDBObject>();

        SqliteCommand cmd = sqlHelper.CreateTextCommand("SELECT * FROM recipe_metadata");
        SqliteDataReader reader = sqlHelper.ExecuteCommand(cmd);

        while (reader.Read())
        {
            RecipeMetadataDBObject row = new RecipeMetadataDBObject(reader);
            recipeList.Add(row);
        }
        sqlHelper.CloseResultReader(reader);

        return recipeList;
    }
    public List<ItemMetadataDBObject> GetRecipeMetadata()
    {
        if (itemList != null)
            return itemList;
        itemList = new List<ItemMetadataDBObject>();

        SqliteCommand cmd = sqlHelper.CreateTextCommand("SELECT * FROM item_metadata");
        SqliteDataReader reader = sqlHelper.ExecuteCommand(cmd);

        while (reader.Read())
        {
            ItemMetadataDBObject row = new ItemMetadataDBObject(reader);
            itemList.Add(row);
        }
        sqlHelper.CloseResultReader(reader);

        return itemList;
    }
}
