using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContrailBehavior : MonoBehaviour {

	public Vector3 endPoint = new Vector3(0.0f, 0.0f, -3.0f);


	//----------Internal variables----------

	//the renderer for the headlight smear
	private LineRenderer lineRend;


	//an array of the contrail's vertices
	private Vector3[] verts = new Vector3[2];


	public bool Active { get; set; }


	private void Awake(){
		lineRend = GetComponent<LineRenderer>();
		Active = false;
	}


	private void Update(){
		if (Active){
			verts[0] = transform.position;
			verts[1] = transform.position + endPoint;

			lineRend.SetPositions(verts);
		}
	}


	public void ChangeState(bool state){
		Active = state;

		verts[0] = transform.position;
		verts[1] = transform.position + endPoint;

		lineRend.SetPositions(verts);

		lineRend.enabled = Active;
	}
}
