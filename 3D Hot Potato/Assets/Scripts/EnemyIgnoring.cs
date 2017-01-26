using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIgnoring : EnemyBase {

	//-------tunable variables-------
	public float speed = 1.0f;
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
	private Vector3 direction;



	private void Start(){
		rb = GetComponent<Rigidbody>();
		direction = GetDirection();
	}


	private void FixedUpdate(){
		if (enteringScreen){
			rb.MovePosition(MoveOntoScreen());
		} else {
			rb.MovePosition(transform.position + direction * speed * Time.fixedDeltaTime);
		}
	}


	private Vector3 GetDirection(){
		if (transform.position.x < -Mathf.Abs(buildingXCoord)){
			return Vector3.right;
		} else if (transform.position.x > Mathf.Abs(buildingXCoord)){ //check if the enemy is off to the right side
			return -Vector3.right;
		} else { //not off to the side; the enemy is coming from the top
			return -Vector3.forward;
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


	/// <summary>
	/// Call this function to restore default values when an enemy comes out of the pool and into play.
	/// 
	/// Call this *before* the enemy is moved into position, so that everything is in a predictable state when the enemy's own script takes over.
	/// </summary>
	public override void Reset(){
		gameObject.SetActive(true);

		//reset timers so that the enemy behaves correctly when coming out of the pool.
		timer = 0.0f;
		enteringScreen = true;

		//find the end point of the enemy's entry onto the screen
		start = transform.position;
		end = DetermineEntryEndPoint();

		GetComponent<Rigidbody>().velocity = startVelocity; //sanity check: make absolutely sure the velocity is zero
	}
}
