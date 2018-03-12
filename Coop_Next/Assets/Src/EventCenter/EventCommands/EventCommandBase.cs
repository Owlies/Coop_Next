using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract class EventCommandBase {
    protected GameObject actor;
    public abstract void Execute();
    public EventCommandBase(GameObject actor) {
        this.actor = actor;
    }

}
