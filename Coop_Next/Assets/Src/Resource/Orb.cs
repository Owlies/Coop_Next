using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Orb : Resource {

    //temp
    private void Start()
    {
        applyOrbEffect += Orb.AddAttackDamage;
    }

    public Action<InteractiveItem> applyOrbEffect;

    static public void AddAttackDamage(InteractiveItem item)
    {
        if (item is AttackBuilding)
            (item as AttackBuilding).attackDamage += 10;
    }
}
