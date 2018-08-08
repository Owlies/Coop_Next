using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;

public class RecipeMetadata
{
    public int recipeId;
    public List<ResourceEnum> resources;

    public RecipeMetadata(SqliteDataReader reader)
    {
        recipeId = reader.GetInt32(0);
        resources = new List<ResourceEnum>();
        string resourceEnumName = reader.GetString(1).Replace("\n", string.Empty);
        ResourceEnum resourceEnum =  MetadataLoader.Instance.GetResourceEnumWithName(resourceEnumName);
        if (resourceEnum != ResourceEnum.None)
            resources.Add(resourceEnum);
        resourceEnumName = reader.GetString(2).Replace("\n", string.Empty);
        resourceEnum = MetadataLoader.Instance.GetResourceEnumWithName(resourceEnumName);
        if (resourceEnum != ResourceEnum.None)
            resources.Add(resourceEnum);
        resourceEnumName = reader.GetString(3).Replace("\n", string.Empty);
        resourceEnum = MetadataLoader.Instance.GetResourceEnumWithName(resourceEnumName);
        if (resourceEnum != ResourceEnum.None)
            resources.Add(resourceEnum);
        resourceEnumName = reader.GetString(4).Replace("\n", string.Empty);
        resourceEnum = MetadataLoader.Instance.GetResourceEnumWithName(resourceEnumName);
        if (resourceEnum != ResourceEnum.None)
            resources.Add(resourceEnum);
    }
}
