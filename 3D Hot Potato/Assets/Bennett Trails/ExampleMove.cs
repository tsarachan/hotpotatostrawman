using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleMove : MonoBehaviour {
	private Vector3 oldPosition;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		oldPosition = transform.position;
		transform.position = new Vector3 (
			Mathf.Sin (Time.time * 2f) * 10f,
			Mathf.Sin (Time.time) * 10f,
			Mathf.Cos (Time.time * 3f) * 10f);
		Vector3 movementVector = (transform.position - oldPosition).normalized;
		transform.rotation = Quaternion.LookRotation (movementVector);
	}
}
