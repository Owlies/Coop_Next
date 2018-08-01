using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : BuildingBase {


    public void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
    }
}
