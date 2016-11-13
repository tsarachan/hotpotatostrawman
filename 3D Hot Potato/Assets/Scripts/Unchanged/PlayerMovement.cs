using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

	//all of these are used to get the thumbstick axes
	private const string LSTICK_HORIZ = "PS4_LStick_Horiz_";
	private const string LSTICK_VERT = "PS4_LStick_Vert_";
	private char playerNum = '0';
	private string myHorizAxis = ""; //this player's horizontal left thumbstick axis
	private string myVertAxis = ""; //this player's vertical left thumbstick axis

	//debug controls
	public KeyCode up;
	public KeyCode down;
	public KeyCode left;
	public KeyCode right;

	//dash controls
	public KeyCode dash; //debug control; set to the same as the pass debug in PlayerBallInteraction
	private const string O_BUTTON = "PS4_O_";

	private Rigidbody rb;

	public float maxSpeed = 1.0f; //player maximum speed
	public float speed = 0.3f; //amount player accelerates each frame of input
	private float zMultiplier = 1.0f;
	public float accelDecel = 0.1f;
	private Vector3 direction = new Vector3(0.0f, 0.0f, 0.0f); //the direction in which the player is moving

	private bool stopped = false; //players are stopped, for example, when they destroy an enemy
	public bool Stopped{
		get { return stopped; }
		set { stopped = value; }
	}
	public float stopDuration = 0.25f;
	private float stopTimer = 0.0f;

	//these variables relate to dashing
	private bool dashing = false; //is the player dashing? IF so, the player will move differently
	public float dashSpeed = 10.0f; //the acceleration applied when the player starts dashing
	public float dashMaxSpeed = 10.0f; //the speed the player moves at when dashing
	public float dashDuration = 0.5f; //how long the dash lasts
	private float dashTimer = 0.0f;
	private PlayerBallInteraction playerBallInteraction; //used to determine whether the player is the ball carrier; can't dash if so
	public float freezeTime = 0.2f; //how long enemies will stay in place when the player dashes through a bunch of enemies;
	public int enemiesToFreeze = 3; //how many enemies does the player have to dash through to freeze the screen?
	private int enemyLayer = 9; //enemies are on layer 9; this enables the dash spherecast to see them
	private int enemyMask; //layermasks work in bits, rather than ints; we'll need this below
	public float freezeCheckRadius = 1.0f; //radius of the sphere that will be cast
	private Transform enemyOrganizer;
	private const string ENEMY_ORGANIZER = "Enemies";
	private Transform buildingOrganizer;
	private const string BUILDING_ORGANIZER = "Buildings";


	private void Start(){
		playerNum = transform.name[7]; //assumes players are named using the convention "Player #"
		myHorizAxis = LSTICK_HORIZ + playerNum;
		myVertAxis = LSTICK_VERT + playerNum;
		rb = GetComponent<Rigidbody>();
		playerBallInteraction = GetComponent<PlayerBallInteraction>();
		enemyMask = 1 << enemyLayer;
		enemyOrganizer = GameObject.Find(ENEMY_ORGANIZER).transform;
		buildingOrganizer = GameObject.Find(BUILDING_ORGANIZER).transform;
	}

	private void Update(){
		if (Input.GetButtonDown(O_BUTTON + playerNum) && !playerBallInteraction.BallCarrier ||
			Input.GetKeyDown(dash) && !playerBallInteraction.BallCarrier){
			dashing = CheckForTimeFreeze(); ;
		}
	}

	private void FixedUpdate(){

		//normal movement
		if (!Stopped && !dashing){
			direction = GetDirection(); //when not dashing, the player can turn

			rb.AddForce(direction * GetAcceleration() * speed, ForceMode.Force);

			//This is a bodge to limit maximum speed. The better way would be to impose a countervailing force.
			//Directly manipulating rigidbody velocity could lead to physics problems.
			if (rb.velocity.magnitude > maxSpeed) { rb.velocity = rb.velocity.normalized * maxSpeed; }

			//not moving because movement paused while fighting an enemy
		} else if (Stopped && !dashing) {
			rb.velocity = Vector3.zero;
			Stopped = RunStopTimer();

			//dashing
		} else if (dashing){
			//no call to GetDirection() when dashing--the player is committed to a single direction
			rb.AddForce(direction * dashSpeed, ForceMode.Force);
			if (rb.velocity.magnitude > dashMaxSpeed) { rb.velocity = rb.velocity.normalized * dashMaxSpeed; }

			dashing = RunDashTimer();
		}
	}

	private Vector3 GetDirection(){
		Vector3 direction = new Vector3(0.0f, 0.0f, 0.0f);

		if (Input.GetAxis(myHorizAxis) < -0.3f || Input.GetKey(left)){
			direction.x = -1.0f;
		} else if (Input.GetAxis(myHorizAxis) > 0.3f || Input.GetKey(right)){
			direction.x = 1.0f;
		}

		if (Input.GetAxis(myVertAxis) > 0.3f || Input.GetKey(down)){
			direction.z = -1.0f;
		} else if (Input.GetAxis(myVertAxis) < -0.3f || Input.GetKey(up)){
			direction.z = 1.0f;
		}

		return direction.normalized;
	}

	private float GetAcceleration(){
		if (Input.GetAxis(myVertAxis) > 0.3f || Input.GetKey(down)){
			zMultiplier -= accelDecel;
		} else if (Input.GetAxis(myVertAxis) < -0.3f || Input.GetKey(up)){
			zMultiplier += accelDecel;
		}

		zMultiplier = Mathf.Clamp(zMultiplier, 1.0f, 5.0f);

		return Mathf.Abs(zMultiplier);
	}

	private bool RunStopTimer(){
		stopTimer += Time.deltaTime;

		if (stopTimer >= stopDuration){
			return false;
		} else {
			return true;
		}
	}

	private bool RunDashTimer(){
		dashTimer += Time.deltaTime;

		if (dashTimer >= dashDuration){
			dashTimer = 0.0f;
			return false;
		} else {
			return true;
		}
	}

	private bool CheckForTimeFreeze(){
		Vector3 direction = GetDirection();

		//if the player isn't inputting a direction, don't freeze time and don't dash
		if (direction == Vector3.zero) { return false; }

		//if the player is inputting a direction, do a spherecast to see if the player will destroy enough enemies to freeze time
		RaycastHit[] hits;
		hits = Physics.SphereCastAll(transform.position, freezeCheckRadius, direction, dashMaxSpeed * dashDuration, enemyMask);

		if (hits.Length >= enemiesToFreeze){
			foreach (Transform enemy in enemyOrganizer){
				enemy.GetComponent<EnemyBase>().Freeze(freezeTime);
			}

			foreach (Transform building in buildingOrganizer){
				building.GetComponent<BuildingMove>().Freeze(freezeTime);
			}
		}

		return true;
	}
}
