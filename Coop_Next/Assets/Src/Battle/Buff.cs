using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff {
    public bool canRepeated = false;
    public bool needRemoved = false;
    public virtual void Activate() { }
    public virtual void Tick() { }
    public virtual void DeActivate() { }
}

public class BuffCollection
{
    public List<Buff> buffs = new List<Buff>();

    public void AddBuff(Buff buff)
    {
        if (buff.canRepeated || !buffs.Contains(buff))
        {
            buffs.Add(buff);
            buff.Activate();
        }
    }

    public void Tick()
    {
        buffs.RemoveAll(r => r.needRemoved);
        foreach (var buff in buffs)
            buff.Tick();
    }

    public void Remove(Buff buff)
    {
        if (buffs.Contains(buff))
            buffs.Remove(buff);
    }
}
