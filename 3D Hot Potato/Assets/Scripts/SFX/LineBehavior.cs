using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineBehavior : MonoBehaviour {

	//----------Tunable variables----------
	public int accelLineVertices = 2;


	//use these to set the position of the line relative to the cycle
	public float yOffset = -1.0f;
	public float zOffset = -2.51f;


	//the speed with which the end of the trail scrolls backward
	public Vector3 accelLineSpeed = new Vector3(0.0f, 0.0f, -0.85f);


	//is the line currently active?
	private GameObject accelLineObj;
	private const string ACCEL_LINE_OBJ = "Acceleration trail";
	public bool AccelLineStatus { get; set; }



	private LineRenderer accelLineRenderer;



	private void Start(){
		accelLineObj = transform.Find(ACCEL_LINE_OBJ).gameObject;
		accelLineRenderer = accelLineObj.GetComponent<LineRenderer>();

		AccelLineStatus = true;
		StartAccelLine();
		accelLineObj.SetActive(AccelLineStatus);
	}


	private void Update(){
		Vector3 lineStart = new Vector3(transform.position.x,
										transform.position.y + yOffset,
										transform.position.z + zOffset);
		Vector3 lineEnd = accelLineRenderer.GetPosition(accelLineVertices - 1) + accelLineSpeed;

		Vector3[] newPositions = new Vector3[2] { lineStart, lineEnd };

		accelLineRenderer.SetPositions(newPositions);
	}


	public void StartAccelLine(){
		Vector3[] vertexLocs = new Vector3[accelLineVertices];

		for (int i = 0; i < accelLineVertices; i++){
			vertexLocs[i] = new Vector3(transform.position.x,
										transform.position.y + yOffset, 
										transform.position.z + zOffset);
		}

		accelLineRenderer.SetPositions(vertexLocs);
	}
}
