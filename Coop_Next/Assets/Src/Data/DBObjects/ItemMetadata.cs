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

        string iconName = reader.GetString(4).Replace("\n", string.Empty);

        recipeId = reader.GetInt32(5);
        techTreeId = reader.GetString(6).Replace("\n", string.Empty);
        actionName = reader.GetString(7).Replace("\n", string.Empty);
        string typeName = reader.GetString(8).Replace("\n", string.Empty);
        subType = GetSubType(typeName);
        level = reader.GetInt32(9);

        gameObjectPrefab = Resources.Load<GameObject>("Prefabs/" + prefabPath);

        size = new Vector2Int(1, 1);

        if (iconName.Length > 0) {
            icon = Resources.Load<Sprite>("Images/UIIcons/ItemIcons/" + iconName);
        }
    }
}
