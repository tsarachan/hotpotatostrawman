using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCircling : EnemyBase {

	//-------tunable variables-------
	public float speed = 1.0f;
	public float enterDistance = 0.0f;
	public float circleRadius = 5.0f; //the radius of the circle this enemy will sweep through around the ball


	//-------internal variables-------
	private Rigidbody rb;


	//variables for finding the buildings, so that this enemy knows where it is and how to come on the screen
	private float playAreaSide = 0.0f; //we'll find a building, and use its X coordinate to figure out how far out spawners might be
	private const string BUILDINGS_ORGANIZER = "Buildings";


	//finding where the enemy is going
	private bool enteringScreen = true;
	private Vector3 start;
	private Vector3 end;
	public float enterTime = 2.0f; //time spent lerping into the scene
	private float timer = 0.0f;
	public AnimationCurve enterCurve;


	//this delegate will be used to decide what kind of movement the enemy should perform--advancing or circling
	private delegate Vector3 MovementFunction();
	private MovementFunction movementFunction;


	//parent the transform
	private const string ENEMY_ORGANIZER = "Enemies";


	//initialize variables
	private void Start(){
		rb = GetComponent<Rigidbody>();
		transform.parent = GameObject.Find(ENEMY_ORGANIZER).transform;
		playAreaSide = Mathf.Abs(GameObject.Find(BUILDINGS_ORGANIZER).transform.GetChild(0).position.x);
	}


	//handle movement
	private void FixedUpdate(){
		if (enteringScreen){
			rb.MovePosition(MoveOntoScreen());
		} else {
			//rb.MovePosition(offsetPosition());

			//rb.MovePosition(transform.position + offset() + direction * speed * Time.fixedDeltaTime);
			//rb.MovePosition(transform.position + direction * speed * Time.fixedDeltaTime);
		}
	}


	/// <summary>
	/// Bring the enemy onto the screen.
	/// </summary>
	/// <returns>The position in world space that the enemy should be in this frame.</returns>
	private Vector3 MoveOntoScreen(){
		timer += Time.deltaTime;

		Vector3 pos = Vector3.LerpUnclamped(start, end, enterCurve.Evaluate(timer/enterTime));

		if (Vector3.Distance(pos, end) <= Mathf.Epsilon) { enteringScreen = false; }

		return pos;
	}


	/// <summary>
	/// Basic function for getting how this enemy will move before it starts circling.
	/// </summary>
	/// <returns>The direction.</returns>
	private Vector3 GetDirection(){
		return -Vector3.forward;
	}


	/// <summary>
	/// Find where this enemy should go to as they move onto the screen.
	/// </summary>
	/// <returns>The entry end point.</returns>
	private Vector3 DetermineEntryEndPoint(){
		return new Vector3(transform.position.x, transform.position.y, transform.position.z - enterDistance);
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
