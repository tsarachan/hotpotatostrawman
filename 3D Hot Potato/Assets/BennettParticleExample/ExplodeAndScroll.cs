using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeAndScroll : MonoBehaviour {

	// Use this for initialization
	void Start () {
		InvokeRepeating ("Explode", 0, 3f);
	}

	void Explode(){
		transform.position = new Vector3 (transform.position.x, transform.position.y, 20f);
		GetComponent<ParticleSystem> ().Stop ();
		GetComponent<ParticleSystem> ().Clear ();
		GetComponent<ParticleSystem> ().Play ();
	}

	// Update is called once per frame
	void Update () {
		transform.position += new Vector3 (0, 0, -0.85f);
	}
}
