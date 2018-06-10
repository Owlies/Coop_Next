﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMetadata {
    public int id;
    public string name;
    public string description;
    public string prefabPath;
    public int recipeID;
    public string techTreeId;
    public Vector2Int size;
    public ObjectSubType subType;
    public int level;

    public GameObject gameObject;

    public InteractiveItem item
    {
        get
        {
            return gameObject.GetComponent<InteractiveItem>();
        }
    }

    public RecipeMetadata recipe
    {
        get
        {
            return MetadataLoader.Instance.GetRecipeMetadataByID(recipeID);
        }
    }
}
