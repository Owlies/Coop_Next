using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TimingCallbacks
{
    public Action OnAttacking = null;
    public Action OnAttacked = null;
    public Action OnPlacedOnMap = null;
    public Action OnPickedUpFromMap = null;
    public Action OnCreated = null;
    public Action OnDestroyed = null;

    public TimingCallbacks()
    {
    }
}
