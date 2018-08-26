using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;

public class BuildingMetadata : ObjectMetadata
{
	public int hp;
	public int attack;
	public float attackFrequency;
	public int attackRange;
    public string custom_value;

	public BuildingMetadata(SqliteDataReader reader) {
		objectId = reader.GetInt32(0);
        techTreeId = reader.GetString(1).Replace("\n", string.Empty);
		objectName = reader.GetString(2).Replace("\n", string.Empty);
        level = reader.GetInt32(3);
        hp = reader.GetInt32(4);
		attack = reader.GetInt32(5);
        attackFrequency = reader.GetFloat(6);
		attackRange = reader.GetInt32(7);
        custom_value = reader.GetString(8).Replace("\n", string.Empty);
        prefabPath = reader.GetString(9).Replace("\n", string.Empty);
        string iconName = reader.GetString(10).Replace("\n", string.Empty);
        description = reader.GetString(11).Replace("\n", string.Empty);
        string typeName = reader.GetString(12).Replace("\n", string.Empty);
        subType = GetSubType(typeName);
        recipeId = reader.GetInt32(13);

        size.x = reader.GetInt32(14);
        size.y = reader.GetInt32(15);

        maxAllowed = reader.GetInt32(16);
        fixDir = reader.GetInt32(17) == 1;

        //gameObject = GameObject.Instantiate(Resources.Load("Prefabs/" + prefabPath) as GameObject);
        //gameObject.SetActive(false);

        gameObjectPrefab = Resources.Load<GameObject>("Prefabs/" + prefabPath);
        if (iconName.Length > 0) {
            icon = Resources.Load<Sprite>("Images/UIIcons/BuildingIcons/" + iconName);
        }
    }
}
