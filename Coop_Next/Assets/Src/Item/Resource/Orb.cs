using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Orb : Resource
{
    public Action<InteractiveObject> applyOrbEffect;

    static private void AddBasicHitPoint(InteractiveObject item)
    {
        if (item is BuildingBase)
            (item as BuildingBase).maxHitPoint *= 1.5f;
    }

    static public void AddAttackDamage(InteractiveObject item)
    {
        AddBasicHitPoint(item);
        if (item is AttackBuilding)
            (item as AttackBuilding).attackDamage *= 1.5f;
    }

    static public void AddHitPoint(InteractiveObject item)
    {
        if (item is BuildingBase)
            (item as BuildingBase).maxHitPoint *= 3f;
    }

    static public void ReduceCoolDown(InteractiveObject item)
    {
        AddBasicHitPoint(item);
        if (item is BuildingBase)
            (item as BuildingBase).coolDownFactor *= 0.5f;
    }
}
