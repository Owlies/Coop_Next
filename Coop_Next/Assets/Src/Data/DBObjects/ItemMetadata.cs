using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;

public class ItemMetadata : ObjectMetadata
{
    public string actionName;

    public ItemMetadata(SqliteDataReader reader)
    {
        id = reader.GetInt32(0);
        name = reader.GetString(1).Replace("\n", string.Empty);
        description = reader.GetString(2).Replace("\n", string.Empty);
        prefabPath = reader.GetString(3).Replace("\n", string.Empty);
        recipeID = reader.GetInt32(4);
        techTreeId = reader.GetString(5).Replace("\n", string.Empty);
        actionName = reader.GetString(6).Replace("\n", string.Empty);
        subType = (ObjectSubType)reader.GetInt32(7);
        level = reader.GetInt32(8);

        gameObject = Resources.Load("Prefabs/" + prefabPath) as GameObject;
        size = new Vector2Int(1, 1);
    }
}
