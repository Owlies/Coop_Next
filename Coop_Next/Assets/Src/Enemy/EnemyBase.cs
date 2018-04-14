using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : OverridableMonoBehaviour
{
    public float attackDamage = 1.0f;
    public float attackSpeed = 5.0f;
    public float moveSpeed = 5.0f;
    public float maxHP = 100.0f;

    private float currentHP;
    private Vector3 targetPosition;

    public void Initialize(int currentWave, float enemyHPIncreasePercentage, Vector3 targetPos)
    {
        maxHP = maxHP * Mathf.Pow(enemyHPIncreasePercentage, currentWave);
        targetPosition = targetPos;
    }

    public override void UpdateMe() {
        MoveTowardsTarget();
    }

    private void MoveTowardsTarget() {
        float step = moveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
    }
}
