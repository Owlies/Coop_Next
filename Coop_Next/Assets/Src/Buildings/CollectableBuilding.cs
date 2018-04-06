using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CollectableBuilding : BuildingBase {
    public abstract void CollectItem(GameObject player);
}
