using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;

public class RecipeMetadata
{
    public int recipeId;
    public List<ResourceEnum> resources;

    public ResourceEnum GetResourceEnum(string name)
    {
        if (name.Equals("Wood"))
            return ResourceEnum.Wood;
        if (name.Equals("Rock"))
            return ResourceEnum.Rock;
        if (name.Equals("Ore"))
            return ResourceEnum.Ore;
        if (name.Equals("Coal"))
            return ResourceEnum.Coal;
        if (name.Equals("ArcaneOrb"))
            return ResourceEnum.ArcaneOrb;
        if (name.Equals("NatureOrb"))
            return ResourceEnum.NatureOrb;
        if (name.Equals("StrengthOrb"))
            return ResourceEnum.StrengthOrb;

        return ResourceEnum.None;
    }

    public RecipeMetadata(SqliteDataReader reader)
    {
        recipeId = reader.GetInt32(0);
        resources = new List<ResourceEnum>();
        string resourceEnumName = reader.GetString(1).Replace("\n", string.Empty);
        ResourceEnum resourceEnum =  GetResourceEnum(resourceEnumName);
        if (resourceEnum != ResourceEnum.None)
            resources.Add(resourceEnum);
        resourceEnumName = reader.GetString(2).Replace("\n", string.Empty);
        resourceEnum = GetResourceEnum(resourceEnumName);
        if (resourceEnum != ResourceEnum.None)
            resources.Add(resourceEnum);
        resourceEnumName = reader.GetString(3).Replace("\n", string.Empty);
        resourceEnum = GetResourceEnum(resourceEnumName);
        if (resourceEnum != ResourceEnum.None)
            resources.Add(resourceEnum);
        resourceEnumName = reader.GetString(4).Replace("\n", string.Empty);
        resourceEnum = GetResourceEnum(resourceEnumName);
        if (resourceEnum != ResourceEnum.None)
            resources.Add(resourceEnum);
    }
}
