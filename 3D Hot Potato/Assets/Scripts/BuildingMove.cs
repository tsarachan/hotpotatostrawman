using UnityEngine;
using System.Collections;

public class BuildingMove : MonoBehaviour {

	public float speed;
	bool gameStart;
	Vector3 startPos;

	void Start(){

	
	}
	void Update(){
		transform.localPosition -= transform.forward * 50 * Time.deltaTime;

		if (transform.localPosition.z == -25) {
		
			transform.localPosition = new Vector3 (-34.9f, 9.04f, 74.8f);
		}

	}
}
