using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;

public class RecipeMetadata
{
    public int recipeId;
    public ResourceEnum[] resources;

    public RecipeMetadata(SqliteDataReader reader)
    {
        recipeId = reader.GetInt32(0);
        resources = new ResourceEnum[4];
        resources[0] = (ResourceEnum)reader.GetInt32(1);
        resources[1] = (ResourceEnum)reader.GetInt32(2);
        resources[2] = (ResourceEnum)reader.GetInt32(3);
        resources[3] = (ResourceEnum)reader.GetInt32(4);

    }
}
