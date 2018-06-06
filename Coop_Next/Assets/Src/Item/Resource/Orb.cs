using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Orb : Resource
{
    public Action<InteractiveItem> applyOrbEffect;

    static private void AddBasicHitPoint(InteractiveItem item)
    {
        if (item is BuildingBase)
            (item as BuildingBase).MaxHitPoint *= 1.5f;
    }

    static public void AddAttackDamage(InteractiveItem item)
    {
        AddBasicHitPoint(item);
        if (item is AttackBuilding)
            (item as AttackBuilding).attackDamage *= 1.5f;
    }

    static public void AddHitPoint(InteractiveItem item)
    {
        if (item is BuildingBase)
            (item as BuildingBase).MaxHitPoint *= 3f;
    }

    static public void ReduceCoolDown(InteractiveItem item)
    {
        AddBasicHitPoint(item);
        if (item is BuildingBase)
            (item as BuildingBase).coolDownFactor *= 0.5f;
    }
}
