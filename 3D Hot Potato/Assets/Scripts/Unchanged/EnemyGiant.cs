using UnityEngine;
using System.Collections;

public class EnemyGiant : EnemyBase {

	private Rigidbody rb;

	private const string ENEMY_ORGANIZER = "Enemies";

	private const string BALL_OBJ = "Ball";
	private Transform ball;

	public float speed = 1000.0f;

	//these variables are used to bring enemies onto the screen, allowing players to see them before they start attacking
	private bool enteringScreen = true;
	public float enterDistance = 15.0f; //how far the homing enemy will move before it starts attacking
	public float enterTime = 2.0f;
	private float timer = 0.0f;
	public AnimationCurve enterCurve;
	private Vector3 start;
	private Vector3 end;

	//time before the giant enemy is removed from the scene
	public float existDuration = 2.5f;
	private float existTimer = 0.0f;

	private void Start(){
		transform.parent = GameObject.Find(ENEMY_ORGANIZER).transform;
		LineUpWithBall();
	}

	//line this enemy up with the ball
	private void LineUpWithBall(){
		ball = GameObject.Find(BALL_OBJ).transform;
		transform.position = new Vector3(ball.position.x,
			0.0f,
			transform.position.z);
		rb = GetComponent<Rigidbody>();
		start = transform.position;
		end = new Vector3(transform.position.x,
			transform.position.y,
			transform.position.z - enterDistance);
	}

	//track how long the enemy should exist
	private void Update(){
		existTimer += Time.deltaTime;

		if (existTimer >= existDuration){
			ObjectPooling.ObjectPool.AddObj(gameObject);
		}
	}

	//handle movement
	private void FixedUpdate(){
		if (enteringScreen){
			rb.MovePosition(MoveOntoScreen());
		} else {
			rb.AddForce(new Vector3(0.0f, 0.0f, -speed), ForceMode.Impulse); //move straight down

			//This is a bodge to limit maximum speed. The better way would be to impose a countervailing force.
			//Directly manipulating rigidbody velocity could lead to physics problems.
			if (rb.velocity.magnitude > speed) { rb.velocity = rb.velocity.normalized * speed; }
		}
	}

	private Vector3 MoveOntoScreen(){
		timer += Time.deltaTime;

		Vector3 pos = Vector3.LerpUnclamped(start, end, enterCurve.Evaluate(timer/enterTime));

		if (Vector3.Distance(pos, end) <= Mathf.Epsilon) { enteringScreen = false; }

		return pos;
	}


	//this is intentionally empty; giant enemies can't be destroyed by blocking them
	public override void GetDestroyed(){
		
	}


	/// <summary>
	/// Call this function to restore default values when an enemy comes out of the pool and into play.
	/// 
	/// Call this *before* the enemy is moved into position, so that everything is in a predictable state when the enemy's own script takes over.
	/// </summary>
	public override void Reset(){
		gameObject.SetActive(true);


		//reset timers so that this enemy behaves correctly when taken out of the pool
		timer = 0.0f;
		existTimer = 0.0f;
		enteringScreen = true;

		GetComponent<Rigidbody>().velocity = startVelocity; //sanity check: make absolutely sure the velocity is zero

		LineUpWithBall();
	}
}