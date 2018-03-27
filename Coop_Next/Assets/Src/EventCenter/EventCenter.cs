using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventCenter : Singleton<EventCenter> {

    public void executeEvent(EventCommandBase ev) {
        ev.Execute();
    }

}
