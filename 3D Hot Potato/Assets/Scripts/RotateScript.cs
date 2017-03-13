using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateScript : MonoBehaviour {

	public float rotationSpeed = 50.0f;


	private Vector3 startRotation = new Vector3(0.0f, 0.0f, 0.0f);
	private Vector3 rotationPerFrame = new Vector3(0.0f, 0.0f, 0.0f);


	//variables for finding the buildings, so that this enemy knows where it is and how to rotate
	private float playAreaSide = 0.0f; //we'll find a building, and use its X coordinate to figure out how far out spawners might be
	private const string BUILDINGS_ORGANIZER = "Buildings";


	private void Start(){
		playAreaSide = Mathf.Abs(GameObject.Find(BUILDINGS_ORGANIZER).transform.GetChild(0).position.x);

		transform.rotation = GetStartRotation();
		rotationPerFrame = GetRotation();
	}


	private Quaternion GetStartRotation(){
		if (transform.position.x < -playAreaSide){ //starting on the left
			return Quaternion.Euler(0.0f, 90.0f, 0.0f);
		} else if (transform.position.x > playAreaSide){ //start on the right
			return Quaternion.Euler(0.0f, 90.0f, 0.0f);
		} else { //starting at the top of the screen
			return Quaternion.Euler(0.0f, 0.0f, 0.0f);
		}
	}


	private Vector3 GetRotation(){
		if (transform.position.x < -playAreaSide){ //starting on the left
			return new Vector3(rotationSpeed, 0.0f, 0.0f);
		} else if (transform.position.x > playAreaSide){ //start on the right
			return new Vector3(-rotationSpeed, 0.0f, 0.0f);
		} else { //starting at the top of the screen
			return new Vector3(-rotationSpeed, 0.0f, 0.0f);
		}
	}


	private void Update(){
		transform.Rotate(rotationPerFrame);
	}
}
