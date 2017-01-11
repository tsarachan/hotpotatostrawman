using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BackwardsTrail : MonoBehaviour {

	//the trail will extend to, and end at, this point
	private Transform trailEnd;
	private const string TRAIL_END_ORGANIZER = "Trails";
	private const string TRAIL_END_POINT = " trail end";

	//length of line from the center of the cycle to a point at the base of the triangle
	public float distToCorner = 1.0f;

	public Vector3 renderDirection = new Vector3(0.0f, 1.0f, 0.0f);

	private Mesh mesh; //the mesh that will be used to render the trail
	public Material trailMaterial; //the trail's material. This is set in Start(), and can't be changed at runtime

	//the position, in the player's local space, where the trail should end when there is no input
	public Vector3 neutralPoint = new Vector3(0.0f, 0.0f, -1.0f);

	public float speed = 0.1f; //speed at which the trail returns to neutral
	public Color trailColor = Color.blue; //the color of the trail

	private bool gameHasStarted = false;
	public float trailDeployDuration = 1.0f; //when the game starts, how long does it take the trail to reach full length?
	private float trailDeployTimer = 0.0f;
	public AnimationCurve deployCurve;


	//initialize variables
	private void Start(){
		trailEnd = transform.root.Find(TRAIL_END_ORGANIZER).Find(gameObject.name + TRAIL_END_POINT);
		GameObject trail = new GameObject(gameObject.name + " trail", new[] { typeof(MeshRenderer), typeof(MeshFilter) } );
		mesh = trail.GetComponent<MeshFilter>().mesh = new Mesh();
		trail.GetComponent<Renderer>().material = trailMaterial;
		trail.GetComponent<Renderer>().material.color = trailColor;
	}


	private void Update(){
		if (gameHasStarted){
			if (trailDeployTimer < trailDeployDuration){
				trailDeployTimer += Time.deltaTime;
				trailEnd.position = DeployTrailEnd();
			}

			Vector3 currentNeutralPoint = transform.position + neutralPoint;
			trailEnd.position = ReturnTrailToNeutral(currentNeutralPoint);

			DrawTrail();
		}
	}

	private Vector3 DeployTrailEnd(){
		return Vector3.Lerp(transform.position,
							transform.position + neutralPoint,
							deployCurve.Evaluate(trailDeployTimer/trailDeployDuration));
	}

	private Vector3 ReturnTrailToNeutral(Vector3 neutral){
		return Vector3.MoveTowards(trailEnd.position, neutral, speed * Time.deltaTime);
	}


	/// <summary>
	/// Creates the geometry that will make up the trail.
	/// 
	/// For an explanation of what's going on here, see:
	/// https://docs.unity3d.com/Manual/Example-CreatingaBillboardPlane.html
	/// </summary>
	private void DrawTrail(){
		Vector3[] vertices = new Vector3[3];

		vertices[0] = trailEnd.position;
		vertices[1] = transform.position + new Vector3(distToCorner, 0.0f, 0.0f);
		vertices[2] = transform.position + new Vector3(-distToCorner, 0.0f, 0.0f);

		mesh.vertices = vertices;

		int[] triangles = new int[3];

		triangles[0] = 0;
		triangles[1] = 2;
		triangles[2] = 1;

		mesh.triangles = triangles;

		Vector3[] normals = new Vector3[3];

		normals[0] = Vector3.up;
		normals[1] = Vector3.up;
		normals[2] = Vector3.up;

		mesh.normals = normals;
	}


	public void StartGame(){
		gameHasStarted = true;
		GetComponent<TrailRenderer>().enabled = false;
	}
}
