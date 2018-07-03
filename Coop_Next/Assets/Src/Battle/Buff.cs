using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff
{
    public bool canRepeated = false;
    public bool needRemoved = false;
    float deactiveTime = 0.5f;
    float currentTime = 0.0f;
    public virtual void Activate(InteractiveObject obj) { }
    public virtual void Tick(InteractiveObject obj)
    {
        if (deactiveTime > 0 && currentTime >= deactiveTime)
        {
            needRemoved = true;
        }
        currentTime += Time.deltaTime;
    }
    public virtual void DeActivate(InteractiveObject obj) { }
    public virtual void Reset()
    {
        needRemoved = false;
        currentTime = 0;
    }
}

public class BuffCollection
{
    public List<Buff> buffs = new List<Buff>();

    public Buff GetBuffWithSameType(Buff buff)
    {
        for(int i = 0; i < buffs.Count; i++)
        {
            if (buff.GetType() == buffs[i].GetType())
                return buffs[i];
        }
        return null;
    }

    public void AddBuff(InteractiveObject obj, Buff buff)
    {
        var buffWithSameType = GetBuffWithSameType(buff);
        if (buff.canRepeated || buffWithSameType == null)
        {
            buffs.Add(buff);
            buff.Activate(obj);
        }
        else
        {
            buffWithSameType.Reset();
        }
    }

    public void Tick(InteractiveObject obj)
    {
        foreach (var buff in buffs)
            buff.Tick(obj);
        buffs.RemoveAll(r => r.needRemoved);
    }

    public void RemoveBuff(InteractiveObject obj, Buff buff)
    {
        if (buffs.Contains(buff))
        {
            buff.DeActivate(obj);
            buffs.Remove(buff);
        }
    }
}

public class AtackDamageBuff : Buff
{
    float attackValue = 1.0f;
    public override void Tick(InteractiveObject obj)
    {
        base.Tick(obj);
        if (obj is AttackBuilding)
        {
            AttackBuilding building = obj as AttackBuilding;
            building.attackDamageModifier += building.attackDamage * attackValue;
        }
    }
}