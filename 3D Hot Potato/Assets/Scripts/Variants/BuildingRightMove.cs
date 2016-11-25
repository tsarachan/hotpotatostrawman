using UnityEngine;
using System.Collections;

public class BuildingRightMove : BuildingMove {

	public float speed;
	bool gameStart;
	Vector3 startPos;

	void Start(){


	}
	void Update(){
		transform.localPosition -= transform.forward * 75 * Time.deltaTime;

		if (transform.localPosition.z <= -25.0f) {

			transform.localPosition = new Vector3 (35.2f, 9.04f, 89.9f);
		}

	}
}
