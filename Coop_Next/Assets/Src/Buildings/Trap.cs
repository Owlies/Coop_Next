using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : BuildingBase {

    List<EnemyBase> enemysOnTrap = new List<EnemyBase>(16);
    float attackCoolDownSeconds = 1.0f;
    float attackDamage = 20.0f;
    float currentTime = 0;

    protected override void InitializeWithBuildingConfig()
    {
        base.InitializeWithBuildingConfig();
        BuildingMetadata metadata = objectMetadata as BuildingMetadata;
        if (metadata == null)
            return;
        attackDamage = metadata.attack;
        attackCoolDownSeconds = 1.0f / metadata.attackFrequency;
    }

    public override void UpdateMe()
    {
        currentTime += Time.deltaTime;
        if (currentTime > attackCoolDownSeconds)
        {
            currentTime = 0;
            for(int i = 0; i < enemysOnTrap.Count; ++i)
            {
                enemysOnTrap[i].TakeDamage(attackDamage);
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        var enemy = other.GetComponent<EnemyBase>();
        if (enemy != null)
        {
            enemysOnTrap.Add(enemy);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        var enemy = other.GetComponent<EnemyBase>();
        if (enemy != null)
            enemysOnTrap.Remove(enemy);
    }
}
