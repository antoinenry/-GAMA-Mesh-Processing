using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitForce : MonoBehaviour {

    public Vector3 force;
    
	void Start ()
    {
        GetComponent<Rigidbody2D>().AddForce(force);
	}
}
