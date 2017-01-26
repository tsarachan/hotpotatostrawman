using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEntersScreen : EnemyBase {

	//-------tunable variables-------
	public float enterDistance = 0.0f;


	//-------internal variables-------
	private Rigidbody rb;


	//variables for finding the buildings, so that this enemy knows where it is and how to come on the screen
	private float buildingXCoord = 0.0f; //we'll find a building, and use its X coordinate to figure out how far out spawners might be
	private const string BUILDINGS_ORGANIZER = "Buildings";

	//finding where the enemy is going
	private bool enteringScreen = true;
	private Vector3 start;
	private Vector3 end;
	public float enterTime = 2.0f; //time spent lerping into the scene
	private float timer = 0.0f;
	public AnimationCurve enterCurve;



	private void Start(){
		rb = GetComponent<Rigidbody>();
	}


	private void FixedUpdate(){
		if (enteringScreen){
			rb.MovePosition(MoveOntoScreen());
		}
	}


	/// <summary>
	/// Find where this enemy should go to as they move onto the screen.
	/// </summary>
	/// <returns>The entry end point.</returns>
	private Vector3 DetermineEntryEndPoint(){
		//check to see if this enemy is off to the left side; if so, it needs to lerp sideways
		if (transform.position.x < -Mathf.Abs(buildingXCoord)){
			return new Vector3(transform.position.x + enterDistance, transform.position.y, transform.position.z);
		} else if (transform.position.x > Mathf.Abs(buildingXCoord)){ //check if the enemy is off to the right side
			return new Vector3(transform.position.x - enterDistance, transform.position.y, transform.position.z);
		} else { //not off to the side; the enemy is coming from the top
			return new Vector3(transform.position.x, transform.position.y, transform.position.z - enterDistance);
		}
	}


	private Vector3 MoveOntoScreen(){
		timer += Time.deltaTime;

		Vector3 pos = Vector3.LerpUnclamped(start, end, enterCurve.Evaluate(timer/enterTime));

		if (Vector3.Distance(pos, end) <= Mathf.Epsilon) { enteringScreen = false; }

		return pos;
	}


	public override void GetDestroyed(){
		GetComponent<ParticleBurst>().MakeBurst();
		ObjectPooling.ObjectPool.AddObj(gameObject); //shut this off and return it to the pool
	}
}
