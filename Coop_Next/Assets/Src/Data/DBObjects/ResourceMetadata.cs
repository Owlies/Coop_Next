using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;

public class ResourceMetadata {
    public string resourceName;
    public ResourceEnum resourceEnum;
    public Sprite icon;

    public ResourceMetadata(SqliteDataReader reader) {
        resourceName = reader.GetString(0).Replace("\n", string.Empty);
        string iconName = reader.GetString(1).Replace("\n", string.Empty);

        if (iconName.Length > 0) {
            icon = Resources.Load<Sprite>("Images/UIIcons/ResourceIcons/" + iconName);
        }
        
        resourceEnum = MetadataLoader.Instance.GetResourceEnumWithName(resourceName);
    }
}
