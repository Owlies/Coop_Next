using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

public class ItemManager : Singleton<ItemManager>
{
    static public void CreateOrbGameObject(GameObject obj,ItemData itemData)
    {
        Item item = obj.GetComponent<Item>();  
        if (item != null)
        {
            item.originData = itemData;
            Type t = typeof(ItemManager);
            MethodInfo method = t.GetMethod(itemData.actionName);
            if (method == null)
                Debug.Log("No item Function " + itemData.actionName + "!!!!!!");
            else
            {
                method.Invoke(ItemManager.Instance, new object[] { item });
            }
        }
    }

    static private void ApplyOrbItem(Orb orb, string orbEffercName)
    {
        if (orb == null)
            return;
        Type t = typeof(Orb);
        MethodInfo method = t.GetMethod(orbEffercName);
        if (method == null)
            Debug.Log("No orb function :" + orbEffercName + "!!!!!!");
        else
        {
            orb.applyOrbEffect = (i) => method.Invoke(orb, new object[] { i });
        }
    }

    static public void Orb_AddAttackDamage(Item item)
    {
        if (item is Orb)
            ApplyOrbItem(item as Orb, "AddAttackDamage");
    }

    static public void Orb_AddHitPoint(Item item)
    {
        if (item is Orb)
            ApplyOrbItem(item as Orb, "AddHitPoint");
    }

    static public void Orb_ReduceCoolDown(Item item)
    {
        if (item is Orb)
            ApplyOrbItem(item as Orb, "ReduceCoolDown");
    }
}
