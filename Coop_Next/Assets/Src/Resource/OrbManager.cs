using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

public class OrbManager : Singleton<OrbManager>
{
    public GameObject arcaneOrb;
    public GameObject natureOrb;
    public GameObject strengthOrb;

    static public GameObject CreateOrbGameObject(OrbData orbData)
    {
        GameObject value = null;
        switch (orbData.type)
        {
            case ResourceEnum.ArcaneOrb:
                value = GameObject.Instantiate<GameObject>(OrbManager.Instance.arcaneOrb);
                break;
            case ResourceEnum.NatureOrb:
                value = GameObject.Instantiate<GameObject>(OrbManager.Instance.natureOrb);
                break;
            case ResourceEnum.StrengthOrb:
                value = GameObject.Instantiate<GameObject>(OrbManager.Instance.strengthOrb);
                break;
        }

        Orb orb = value.GetComponent<Orb>();
        if (orb != null)
        {
            orb.originData = orbData;
            Type t = typeof(OrbManager);
            MethodInfo method = t.GetMethod(orbData.actionName);
            if (method == null)
                Debug.Log("No orb Function " + orbData.actionName + "!!!!!!");
            else
            {
                orb.applyOrbEffect = (item) => method.Invoke(OrbManager.Instance, new object[] { item });
            }
        }

        return value;
    }

    static public void AddBasicHitPoint(InteractiveItem item)
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

[System.Serializable]
public class OrbData
{
    public string name;
    public string actionName;
    public string description;
    public ResourceEnum type;
    public float RareRate;

}
