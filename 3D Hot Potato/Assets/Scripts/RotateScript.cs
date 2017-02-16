using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateScript : MonoBehaviour {

	public float xAxisRotationSpeed = 50.0f;


	private void Update(){
		transform.Rotate(xAxisRotationSpeed, 0.0f, 0.0f);
	}
}
