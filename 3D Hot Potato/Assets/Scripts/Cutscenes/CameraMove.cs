using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour {

	//transforms that will move to pan the camera
	private Transform mainCamera;
	private Transform cameraBoom;

	private float timer = 0.0f;



	private void Start(){
		mainCamera = Camera.main.transform;
		cameraBoom = transform;
	}
}
