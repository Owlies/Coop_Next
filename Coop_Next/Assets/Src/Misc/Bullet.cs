using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
	public GameObject target;

	private float movingSpeed;
	private float damage;
	
	public void Initialize(GameObject target, float speed, float damage) {
		this.target = target;
		this.movingSpeed = speed;
		this.damage = damage;
	}

	/// <summary>
	/// OnTriggerEnter is called when the Collider other enters the trigger.
	/// </summary>
	/// <param name="other">The other Collider involved in this collision.</param>
	void OnTriggerEnter(Collider other)
	{
		
	}
}
