using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventCenter : Singleton<EventCenter> {

    public void ExecuteEvent(EventCommandBase ev) {
        ev.Execute();
    }

}
