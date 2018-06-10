using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using UnityEngine.SceneManagement;

public class MetadataLoader : Singleton<MetadataLoader> {   
	private SQLiteHelper sqlHelper;
    List<BuildingMetadata> buildingList = null;
    List<WaveEnemyConfigMetadata> configList = null;
    List<ItemMetadata> itemList = null;
    List<EnemyMetadata> enemyList = null;
    List<RecipeMetadata> recipeList = null;

    public RecipeMetadata GetRecipeMetadataByID(int id)
    {
        if (id == -1)
            return null;
        for(int i =0; i < recipeList.Count; i++)
        {
            if (id == recipeList[i].recipeId)
                return recipeList[i];
        }

        return null;
    }

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
	
	public List<EnemyMetadata> GetEnemyMetadata() {
        if (enemyList != null)
            return enemyList;
        enemyList = new List<EnemyMetadata>();

        SqliteCommand cmd = sqlHelper.CreateTextCommand("SELECT * FROM enemy_config");
        SqliteDataReader reader = sqlHelper.ExecuteCommand(cmd);
        while (reader.Read()) {
            EnemyMetadata row = new EnemyMetadata(reader);
            enemyList.Add(row);
        }
        sqlHelper.CloseResultReader(reader);

		return enemyList;
	}

    public List<WaveEnemyConfigMetadata> GetWaveEnemyConfigMeatadata() {
        if (configList != null)
            return configList;
        configList = new List<WaveEnemyConfigMetadata>();

        SqliteCommand cmd = sqlHelper.CreateTextCommand("SELECT * FROM wave_enemy_config");
        SqliteDataReader reader = sqlHelper.ExecuteCommand(cmd);

        while (reader.Read()) {
            WaveEnemyConfigMetadata row = new WaveEnemyConfigMetadata(reader);
            configList.Add(row);
        }
        sqlHelper.CloseResultReader(reader);

        return configList;
    }

    public List<BuildingMetadata> GetBuildingMetadata()
    {
        if (buildingList != null)
            return buildingList;
        buildingList = new List<BuildingMetadata>();

        SqliteCommand cmd = sqlHelper.CreateTextCommand("SELECT * FROM building_metadata");
        SqliteDataReader reader = sqlHelper.ExecuteCommand(cmd);

        while (reader.Read())
        {
            BuildingMetadata row = new BuildingMetadata(reader);
            buildingList.Add(row);
        }
        sqlHelper.CloseResultReader(reader);

        return buildingList;
    }

    public List<RecipeMetadata> GetRecipeMetadata()
    {
        if (recipeList != null)
            return recipeList;
        recipeList = new List<RecipeMetadata>();

        SqliteCommand cmd = sqlHelper.CreateTextCommand("SELECT * FROM recipe_metadata");
        SqliteDataReader reader = sqlHelper.ExecuteCommand(cmd);

        while (reader.Read())
        {
            RecipeMetadata row = new RecipeMetadata(reader);
            recipeList.Add(row);
        }
        sqlHelper.CloseResultReader(reader);

        return recipeList;
    }
    public List<ItemMetadata> GetItemMetadata()
    {
        if (itemList != null)
            return itemList;
        itemList = new List<ItemMetadata>();

        SqliteCommand cmd = sqlHelper.CreateTextCommand("SELECT * FROM item_metadata");
        SqliteDataReader reader = sqlHelper.ExecuteCommand(cmd);

        while (reader.Read())
        {
            ItemMetadata row = new ItemMetadata(reader);
            itemList.Add(row);
        }
        sqlHelper.CloseResultReader(reader);

        return itemList;
    }
}
