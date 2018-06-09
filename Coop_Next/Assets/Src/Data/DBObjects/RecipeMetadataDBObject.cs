using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;

public class RecipeMetadataDBObject
{
    public int recipeId;
    public ResourceEnum[] resourceID;

    public RecipeMetadataDBObject(SqliteDataReader reader)
    {
        recipeId = reader.GetInt32(0);
        resourceID = new ResourceEnum[4];
        resourceID[0] = (ResourceEnum)reader.GetInt32(1);
        resourceID[1] = (ResourceEnum)reader.GetInt32(2);
        resourceID[2] = (ResourceEnum)reader.GetInt32(3);
        resourceID[3] = (ResourceEnum)reader.GetInt32(4);

    }
}
