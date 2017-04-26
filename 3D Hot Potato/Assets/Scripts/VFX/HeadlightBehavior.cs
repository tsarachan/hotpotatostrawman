using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadlightBehavior : MonoBehaviour {

	//----------Tunable variables----------

	public int numVertices = 10;

	public float newVertDelay = 0.3f; //how long it takes to put down a new vertex

	public Vector3 offset = new Vector3(-2.0f, 2.0f, 0.0f); //distance to the first jag in the headlight smear
	public float xOffsetVariance = 1.0f;
	public float yOffsetRange = 1.0f; //how much can the offsets vary?

	public Vector3 speed = new Vector3(0.0f, 0.0f, -0.85f); //how quickly headlight smear vertices fall backward


	//----------Internal variables----------

	//the renderer for the headlight smear
	private LineRenderer lineRend;


	//a list of the smear's vertices
	private List<Vector3> verts = new List<Vector3>();


	private Vector3 secondVertStart = new Vector3(0.0f, 0.0f, 0.0f);
	private Vector3 secondVertDest = new Vector3(0.0f, 0.0f, 0.0f);

	private float timer = 0.0f;


	private void Start(){
		lineRend = GetComponent<LineRenderer>();
		lineRend.SetPosition(0, transform.position);
		lineRend.SetPosition(1, transform.position + offset);
		verts.Add(transform.position);
		verts.Add(transform.position + offset);
	}


	public void Update(){
		verts[0] = transform.position;

		for (int i = 1; i < verts.Count; i++){
			verts[i] += speed;
		}


		timer += Time.deltaTime;

		if (timer >= newVertDelay){
			verts = SetNewVertices();

			timer = 0.0f;
		}

		Vector3[] newLocs = SetVerticesAsArray();

		lineRend.numPositions = newLocs.Length;
		lineRend.SetPositions(newLocs);
	}


	private List<Vector3> SetNewVertices(){
		List<Vector3> temp = verts;

		temp.Insert(0, transform.position);

		Vector3 newOffset = new Vector3(offset.x + Random.Range(-xOffsetVariance, xOffsetVariance),
										offset.y + Random.Range(-yOffsetRange, yOffsetRange),
										0.0f);

		temp[2] += newOffset;


		if (temp.Count >= numVertices){
			temp.RemoveAt(numVertices - 1);
		}

		return temp;
	}


	private Vector3[] SetVerticesAsArray(){
		Vector3[] temp = new Vector3[verts.Count];

		for (int i = 0; i < temp.Length; i++){
			temp[i] = verts[i];
		}

		return temp;
	}
}
