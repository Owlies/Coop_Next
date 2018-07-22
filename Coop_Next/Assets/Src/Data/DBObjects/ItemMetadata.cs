using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;

public class ItemMetadata : ObjectMetadata
{
    public string actionName;

    public ItemMetadata(SqliteDataReader reader)
    {
        objectId = reader.GetInt32(0);
        objectName = reader.GetString(1).Replace("\n", string.Empty);
        description = reader.GetString(2).Replace("\n", string.Empty);
        prefabPath = reader.GetString(3).Replace("\n", string.Empty);
        recipeId = reader.GetInt32(4);
        techTreeId = reader.GetString(5).Replace("\n", string.Empty);
        actionName = reader.GetString(6).Replace("\n", string.Empty);
        string typeName = reader.GetString(7).Replace("\n", string.Empty);
        subType = GetSubType(typeName);
        level = reader.GetInt32(8);
        gameObject = GameObject.Instantiate(Resources.Load("Prefabs/" + prefabPath) as GameObject);
        gameObject.SetActive(false);
        size = new Vector2Int(1, 1);

        InitInteractiveObj();
    }
}
