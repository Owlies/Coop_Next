using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Catapult : AttackBuilding
{
    public float bulletRange = 2.0f;

    private bool filledRock = false;

    public override bool InteractAction(Player actor)
    {
        if (filledRock)
            return false;
        var interactObj = actor.GetCarryingItem();
        if (interactObj is Resource)
        {
            Resource resource = interactObj as Resource;
            if (resource.resourceEnum == ResourceEnum.Rock)
            {
                actor.UnsetCarryingItem();
                Destroy(interactObj.gameObject);
                filledRock = true;
                return true;
            }
        }

        return false;
    }

    //protected override bool TryAttackEnemy()
    //{
    //    if (!CanAttackEnemy())
    //    {
    //        attackingEnemy = null;
    //        return false;
    //    }
    //    firingPosition = new Vector3(transform.position.x, transform.position.y + (float)(GetComponent<BoxCollider>().size.y * 0.8), transform.position.z);
    //    GameObject bullet = Instantiate(bulletPrefab, firingPosition, Quaternion.LookRotation(attackingEnemy.gameObject.transform.position));
    //    bullet.GetComponent<Bullet>().Initialize(attackingEnemy.gameObject, bulletSpeed, GetAttackDamage());
    //    Destroy(bullet, 10.0f);

    //    attackState = EAttackBuildingState.COOLING_DOWN;
    //    attackCoolDownStartTime = Time.time;

    //    return true;
    //}

    protected override bool CanAttackEnemy()
    {
        if (attackingEnemy == null)
        {
            return false;
        }

        float currentHP = attackingEnemy.GetComponent<EnemyBase>().GetCurrentHitPoint();

        if (currentHP <= Constants.EPS)
        {
            return false;
        }

        if (attackState == EAttackBuildingState.COOLING_DOWN)
        {
            return false;
        }

        if (Vector3.Distance(attackingEnemy.transform.position, transform.position) > attackRange)
        {
            return false;
        }

        if (filledRock == false)
            return false;

        return true;
    }
}
