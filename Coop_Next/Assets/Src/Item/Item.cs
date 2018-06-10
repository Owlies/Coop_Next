using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Item : InteractiveItem {
    
    [HideInInspector]
    public ItemMetadata originData;
    
    //public Action<InteractiveItem> applyItemEffect;
}
