using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : OverridableMonoBehaviour
{
    public float attackDamage = 1.0f;
    public float attackSpeed = 5.0f;
    public float moveSpeed = 5.0f;
    public float fullHP = 100.0f;

    private float currentHP;
}
