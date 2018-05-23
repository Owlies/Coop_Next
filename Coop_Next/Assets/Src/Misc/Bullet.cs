using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : OverridableMonoBehaviour {
	
	private float bulletSpeed = 10.0f;
	private float damage;
	private GameObject target;
	private Vector3 targetPosition;
	
	public void Initialize(GameObject target, float speed, float damage) {
		this.target = target;
		this.targetPosition = target.GetComponent<Collider>().bounds.center;
		this.bulletSpeed = speed;
		this.damage = damage;
	}

	public override void FixedUpdateMe() {
		float step = Time.deltaTime * bulletSpeed;
		transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
		transform.LookAt(targetPosition);
	}

	/// <summary>
	/// OnTriggerEnter is called when the Collider other enters the trigger.
	/// </summary>
	/// <param name="other">The other Collider involved in this collision.</param>
	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject != target) {
			return;
		}

		target.SendMessage("TakeDamage", damage);
		Destroy(this.gameObject);
	}
}
