using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Orb : Resource {

    public OrbData originData;

    [SerializeField]
    public Action<InteractiveItem> applyOrbEffect;
}
