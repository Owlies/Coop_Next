using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using SimpleJSON;

public class BuildingMetadata : ObjectMetadata
{
	public int hp;
	public float attack;
	public float attackFrequency;
	public int attackRange;
    private JSONNode custom_value;

	public BuildingMetadata(SqliteDataReader reader) {
		objectId = reader.GetInt32(0);
        techTreeId = reader.GetString(1).Replace("\n", string.Empty);
		objectName = reader.GetString(2).Replace("\n", string.Empty);
        level = reader.GetInt32(3);
        hp = reader.GetInt32(4);
		attack = reader.GetFloat(5);
        attackFrequency = reader.GetFloat(6);
		attackRange = reader.GetInt32(7);
        var customJSonValue = reader.GetString(8).Replace("\n", string.Empty);
        custom_value = JSON.Parse(customJSonValue);
        if (customJSonValue.Length != 0 && custom_value == null)
            Debug.Log("Object " + objectName + " custom value is in wrong format. Plz check!");
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

        gameObjectPrefab = Resources.Load<GameObject>("Prefabs/" + prefabPath);
        if (iconName.Length > 0) {
            icon = Resources.Load<Sprite>("Images/UIIcons/BuildingIcons/" + iconName);
        }
    }

    public string GetStringCustomValue(string key)
    {
        if (custom_value == null)
            return "";
        return custom_value[key];
    }

    public float GetFloatCustomValue(string key)
    {
        if (custom_value == null)
            return 0;
        return float.Parse(custom_value[key]);
    }

    public int GetIntCustomValue(string key)
    {
        if (custom_value == null)
            return 0;
        return int.Parse(custom_value[key]);
    }
}
