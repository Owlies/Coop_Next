using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : OverridableMonoBehaviour {
	public GameObject target;
	public float bulletSpeed = 10.0f;
	private float damage;
	
	public void Initialize(GameObject target, float damage) {
		this.target = target;
		this.damage = damage;
	}

	public override void FixedUpdateMe() {
		float step = Time.deltaTime * bulletSpeed;
		transform.position = Vector3.MoveTowards(transform.position, target.transform.position, step);
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
	}
}
