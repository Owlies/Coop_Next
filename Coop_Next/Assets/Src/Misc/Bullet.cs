using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : OverridableMonoBehaviour {
	
	private float bulletSpeed = 10.0f;
	private float damage;
    private float range;
	private GameObject target;
    private Vector3 targetPosition;
    private Vector3 targetOffset;

    public void Initialize(GameObject target, float speed, float damage, float range = 0) {
		this.target = target;
        this.targetOffset = target.GetComponent<Collider>().bounds.center - target.transform.position;
        this.targetPosition = target.GetComponent<Collider>().bounds.center;
        this.bulletSpeed = speed;
		this.damage = damage;
        this.range = range;
	}

	public override void FixedUpdateMe() {
		float step = Time.deltaTime * bulletSpeed;
        if (target != null)
            targetPosition = target.transform.position + targetOffset;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
		transform.LookAt(targetPosition);

        if ((transform.position - targetPosition).sqrMagnitude < Constants.EPS)
            Destroy(this.gameObject);
    }


    static private List<EnemyBase> enemies = new List<EnemyBase>();
	/// <summary>
	/// OnTriggerEnter is called when the Collider other enters the trigger.
	/// </summary>
	/// <param name="other">The other Collider involved in this collision.</param>
	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject != target) {
			return;
		}
        if (range < 0.001f)
        {
            other.gameObject.SendMessage("TakeDamage", damage);
        }
        else
        {
            enemies.Clear();
            foreach (EnemyBase enemy in EnemyManager.Instance.GetAllAliveEnemies())
            {
                if (Vector3.Distance(enemy.transform.position, this.transform.position) <= range)
                {
                    enemies.Add(enemy);
                }
            }
            for(int i =0; i < enemies.Count; ++i)
                enemies[i].TakeDamage(damage);
        }
		Destroy(this.gameObject);
	}
}
