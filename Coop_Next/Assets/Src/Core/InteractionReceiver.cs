using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionReceiver : OverridableMonoBehaviour {

    public static bool IsObjectInteractionReciever(GameObject obj)
    {
        if (obj.GetComponent<InteractionReceiver>() == null)
            return false;
        else
            return true;
    }

    public virtual void Interacted(InteractionDispatcher dispacher) { }
    public virtual void PickedUp(InteractionDispatcher dispacher) { }
}
