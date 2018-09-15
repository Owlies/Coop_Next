using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameDetector : OverridableMonoBehaviour
{
    private FlameThrower flameThrower;
    // Use this for initialization
    void Start()
    {
        flameThrower = GetComponentInParent<FlameThrower>();
    }

    private void OnTriggerEnter(Collider other)
    {
        var enemy = other.gameObject.GetComponent<EnemyBase>();
        if (enemy != null)
            flameThrower.attackingEnemys.Add(enemy);
    }

    private void OnTriggerExit(Collider other)
    {
        var enemy = other.gameObject.GetComponent<EnemyBase>();
        if (enemy != null)
            flameThrower.attackingEnemys.Remove(enemy);
    }
}
