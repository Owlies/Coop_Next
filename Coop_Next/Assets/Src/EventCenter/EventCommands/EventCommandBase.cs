using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EventCommandBase {
    protected GameObject actor;
    protected GameObject receiver;
    public abstract bool Execute();

    public EventCommandBase(GameObject actor)
    {
        this.actor = actor;
    }

    public EventCommandBase(GameObject actor, GameObject receiver) {
        this.actor = actor;
        this.receiver = receiver;
    }

}
