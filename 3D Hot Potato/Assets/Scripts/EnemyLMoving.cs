using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLMoving : EnemyBase {

	//-------tunable variables-------
	public float speed = 1.0f;
	public float enterDistance = 0.0f; 
	public int maxTurns = 1; //the number of times this enemy will make 90 degree turns to chase the ball
	public int myNum = 0;

	//this is how close the ball and this enemy must be on an axis for this enemy to consider itself parallel to the ball
	//do not reduce this to less than ~0.5f, or enemies can miss the ball as they move past each other
	public float parallelTolerance = 1.0f;


	//-------internal variables-------
	private Rigidbody rb;


	//finding where the enemy is going
	private bool enteringScreen = true;
	private Vector3 start;
	private Vector3 end;
	public float enterTime = 2.0f; //time spent lerping into the scene
	private float timer = 0.0f;
	public AnimationCurve enterCurve;
	private Vector3 direction = new Vector3(0.0f, 0.0f, 0.0f); //the direction this enemy is currently moving
	private bool parallelToBall = false;

	//variables for finding the buildings, so that this enemy knows where it is and how to come on the screen
	private float playAreaSide = 0.0f; //we'll find a building, and use its X coordinate to figure out how far out spawners might be
	private const string BUILDINGS_ORGANIZER = "Buildings";


	//the ball, so that this enemy can turn toward it or its intended receiver
	private Transform ball;
	private const string BALL_OBJ = "Ball";
	private BallBehavior ballBehavior;


	//parent the transform
	private const string ENEMY_ORGANIZER = "Enemies";


	//how many times this enemy has turned so far
	private int turns = 0;

	//which direction is the enemy currently moving in? Used to decide which parallel to check for
	private enum Directions { HORIZ, VERT };
	private int currentDir = 0;


	//initialize variables
	private void Start(){
		rb = GetComponent<Rigidbody>();
		transform.parent = GameObject.Find(ENEMY_ORGANIZER).transform;
		playAreaSide = Mathf.Abs(GameObject.Find(BUILDINGS_ORGANIZER).transform.GetChild(0).position.x);
		ball = GameObject.Find(BALL_OBJ).transform;
		ballBehavior = ball.GetComponent<BallBehavior>();
		myNum = Random.Range(1, 100);
	}


	//detect whether this enemy is parallel with the ball; if so, go toward the ball
	private void Update(){
		if (!enteringScreen){
			Transform target;

			if (ballBehavior.IntendedReceiver != null){
				target = ballBehavior.IntendedReceiver;
			} else {
				target = ball;
			}

			if (currentDir == (int)Directions.HORIZ){
				if (CheckVertParallel(target) && turns < maxTurns){
					currentDir = Turn();
				}
			} else if (currentDir == (int)Directions.VERT){
				if (CheckHorizParallel(target) && turns < maxTurns){
					currentDir = Turn();
				}
			}
		}
	}


	//handle movement
	private void FixedUpdate(){
		if (enteringScreen){
			rb.MovePosition(MoveOntoScreen());
		} else {
			rb.MovePosition(transform.position + direction * speed * Time.fixedDeltaTime);
		}
	}


	private bool CheckHorizParallel(Transform target){
		return Mathf.Abs(transform.position.z - target.position.z) <= parallelTolerance;
	}

	private bool CheckVertParallel(Transform target){
		return Mathf.Abs(transform.position.x - target.position.x) <= parallelTolerance;
	}


	private int Turn(){
		parallelToBall = true;
		direction = GetDirection();

		if (currentDir == (int)Directions.HORIZ){
			return (int)Directions.VERT;
		} else {
			return (int)Directions.HORIZ;
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
	/// Basic function for determining how this enemy will move--it starts out going in one direction, and then
	/// heads for the ball.
	/// </summary>
	/// <returns>The direction, as a normalized vector.</returns>
	private Vector3 GetDirection(){
		//first step: if coming onto the screen, get the direction needed to do that
		if (!parallelToBall){
			if (transform.position.x < -Mathf.Abs(playAreaSide)){
				currentDir = (int)Directions.HORIZ;
				return Vector3.right;
			} else if (transform.position.x > Mathf.Abs(playAreaSide)){ //check if the enemy is off to the right side
				currentDir = (int)Directions.HORIZ;
				return -Vector3.right;
			} else { //not off to the side; the enemy is coming from the top
				currentDir = (int)Directions.VERT;
				return -Vector3.forward;
			}
		} //second step; if already on the screen, find the ball and go toward it
		else {
			return (ball.position - transform.position).normalized;
		}
	}


	/// <summary>
	/// Find where this enemy should go to as they move onto the screen.
	/// </summary>
	/// <returns>The entry end point.</returns>
	private Vector3 DetermineEntryEndPoint(){
		//check to see if this enemy is off to the left side; if so, it needs to lerp sideways
		if (transform.position.x < -Mathf.Abs(playAreaSide)){
			return new Vector3(transform.position.x + enterDistance, transform.position.y, transform.position.z);
		} else if (transform.position.x > Mathf.Abs(playAreaSide)){ //check if the enemy is off to the right side
			return new Vector3(transform.position.x - enterDistance, transform.position.y, transform.position.z);
		} else { //not off to the side; the enemy is coming from the top
			return new Vector3(transform.position.x, transform.position.y, transform.position.z - enterDistance);
		}
	}


	public override void GetDestroyed(){
		GameObject destroyParticle = ObjectPooling.ObjectPool.GetObj(DESTROY_PARTICLE);
		destroyParticle.transform.position = transform.position;

		Color myColor = GetComponent<Renderer>().material.color;

		destroyParticle.GetComponent<ParticlePlexus>().LineColor = myColor;


		//for reasons unclear, Unity throws an error if you try to set the start color of the particles without
		//first assigning the main module to its own variable
		ParticleSystem.MainModule mainModule = destroyParticle.GetComponent<ParticleSystem>().main;

		mainModule.startColor = myColor;

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

		turns = 0;

		GetComponent<Rigidbody>().velocity = startVelocity; //sanity check: make absolutely sure the velocity is zero

		direction = GetDirection();
	}
}
