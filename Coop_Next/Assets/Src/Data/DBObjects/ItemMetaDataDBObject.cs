using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;

public class ItemMetadataDBObject
{
    public int itemId;
    public string name;
    public string description;
    public string prefabPath;
    public int recipeID;
    public string techTreeId;
    public string actionName;

    public ItemMetadataDBObject(SqliteDataReader reader)
    {
        itemId = reader.GetInt32(0);
        name = reader.GetString(1).Replace("\n", string.Empty);
        description = reader.GetString(2).Replace("\n", string.Empty);
        prefabPath = reader.GetString(3).Replace("\n", string.Empty);
        recipeID = reader.GetInt32(4);
        techTreeId = reader.GetString(5).Replace("\n", string.Empty);
        actionName = reader.GetString(6).Replace("\n", string.Empty);
    }
}
