using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;

public class BuildingMetadata : ObjectMetadata
{
	public int hp;
	public int attack;
	public int attackFrequency;
	public int attackRange;
	public int underAttackPriority;

	public BuildingMetadata(SqliteDataReader reader) {
		objectId = reader.GetInt32(0);
        techTreeId = reader.GetString(1).Replace("\n", string.Empty);
		objectName = reader.GetString(2).Replace("\n", string.Empty);
        level = reader.GetInt32(3);
        hp = reader.GetInt32(4);
		attack = reader.GetInt32(5);
		attackFrequency = reader.GetInt32(6);
		attackRange = reader.GetInt32(7);
        underAttackPriority = reader.GetInt32(8);
        prefabPath = reader.GetString(9).Replace("\n", string.Empty);
        description = reader.GetString(10).Replace("\n", string.Empty);
        string typeName = reader.GetString(11).Replace("\n", string.Empty);
        subType = GetSubType(typeName);
        recipeId = reader.GetInt32(12);

        size.x = reader.GetInt32(13);
        size.y = reader.GetInt32(14);

        gameObject = GameObject.Instantiate(Resources.Load("Prefabs/" + prefabPath) as GameObject);
        gameObject.SetActive(false);

        InitInteractiveObj();
    }
}
