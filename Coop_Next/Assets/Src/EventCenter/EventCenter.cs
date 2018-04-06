using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventCenter : Singleton<EventCenter> {

    public bool ExecuteEvent(EventCommandBase ev) {
        return ev.Execute();
    }

}
