using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour {

    Vector3 pos1;
    public Vector3 pos2;
    public float speed = 10.0f;

	// Use this for initialization
	void Start () {
        pos1 = transform.position;

    }
	
	// Update is called once per frame
	void Update () {
        transform.position += Vector3.Normalize(pos2 - pos1) * Time.deltaTime * speed;
        if (Vector3.Magnitude(transform.position - pos2) < 0.1f)
        {
            Vector3 tmp = pos1;
            pos1 = pos2;
            pos2 = tmp;
        }
    }
}
