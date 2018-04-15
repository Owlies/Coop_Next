using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EEnemyState {
    IDLE,
    MOVING,
    ATTACKING,
    TAKING_DAMAGE
}

public class EnemyBase : OverridableMonoBehaviour {
    public float AttackDamage = 1.0f;
    public float AttackSpeed = 5.0f;
    public float AttackRange = 10.0f;
    public float MoveSpeed = 5.0f;
    public float MaxHitPoint = 100.0f;

    private float currentHitPoint;
    private Vector3 targetPosition;
    private EEnemyState enemyState;
    private BuildingBase attackingTarget;

    public void Initialize(int currentWave, float enemyHPIncreasePercentage, Vector3 targetPos) {
        MaxHitPoint = MaxHitPoint * Mathf.Pow(enemyHPIncreasePercentage, currentWave);
        targetPosition = targetPos;
        enemyState = EEnemyState.IDLE;
        currentHitPoint = MaxHitPoint;
    }

    public override void UpdateMe() {
        if (TryAttackBuilding()) {
            return;
        }

        TryFindBuildingToAttack();
        MoveTowardsTarget();
    }

    private void MoveTowardsTarget() {
        float step = MoveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
    }

    private void TryFindBuildingToAttack() {
        BuildingBase buildingToAttack = GetHighestAttackPriorityBuildingsWithinRange();
        if (buildingToAttack == null) {
            return;
        }

        attackingTarget = buildingToAttack;
    }

    private bool CanAttckCurrentTarget() {
        if (attackingTarget == null) {
            return false;
        }

        if (Vector3.Distance(attackingTarget.transform.position, transform.position) > AttackRange) {
            return false;
        }

        return true;
    }

    private bool TryAttackBuilding() {
        if (!CanAttckCurrentTarget()) {
            return false;
        }

        attackingTarget.TakeDamage(AttackDamage);

        return true;
    }

    private BuildingBase GetHighestAttackPriorityBuildingsWithinRange() {
        BuildingBase highestAttackPriorityBuilding = null;
        foreach (var item in MapManager.Instance.GetCollectionOf<BuildingBase>()) {
            if (Vector3.Distance(item.transform.position, transform.position) > AttackRange) {
                continue;
            }

            if (highestAttackPriorityBuilding == null || highestAttackPriorityBuilding.AttackingPriority < item.AttackingPriority) {
                highestAttackPriorityBuilding = item;
            }
        }

        return highestAttackPriorityBuilding;
    }

    public void TakeDamage(float damage)
    {
        currentHitPoint -= damage;
        if (currentHitPoint <= 0.0f)
        {
            MapManager.Instance.RemoveItemFromMap(this.gameObject);
            Destroy(this.gameObject);
        }
    }
}
