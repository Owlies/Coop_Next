using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectResourceEventStart : EventCommandBase {
    public CollectResourceEventStart(GameObject actor) : base(actor)
    {
    }

    public CollectResourceEventStart(GameObject actor, GameObject receiver) : base(actor, receiver)
    {
    }

    public override void Execute()
    {
        throw new System.NotImplementedException();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
