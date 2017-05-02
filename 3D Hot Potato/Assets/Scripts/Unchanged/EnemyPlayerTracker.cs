using UnityEngine;
using System.Collections;

public class EnemyPlayerTracker : EnemyBase {

	protected const string BALL_OBJ = "Ball";
	public Transform target;
	private Rigidbody rb;
	public float speed = 0.3f;
	public float Speed{
		get { return speed; }
		set { speed = value; }
	}
	public float[] maxSpeeds = { 2.0f, 5.0f, 7.0f };
	private float myMaxSpeed = 0.0f;
	private GameObject destroyParticle;

	private const string PLAYER_ORGANIZER = "Players";
	private Transform playerOrganizer;
	private const string PLAYER_1 = "Player 1";
	private const string PLAYER_2 = "Player 2";

	private const string ENEMY_ORGANIZER = "Enemies";


	//these variables are used to bring enemies onto the screen, allowing players to see them before they start attacking
	private bool enteringScreen = true;
	public float enterDistance = 5.0f; //how far the homing enemy will move before it starts attacking
	public float enterTime = 2.0f;
	private float timer = 0.0f;
	public AnimationCurve enterCurve;
	private Vector3 start;
	private Vector3 end;

	private void Start(){
		transform.parent = GameObject.Find(ENEMY_ORGANIZER).transform;
		playerOrganizer = GameObject.Find(PLAYER_ORGANIZER).transform;
		target = ChooseTarget();
		rb = GetComponent<Rigidbody>();
		//destroyParticle = Resources.Load("DestroyParticle") as GameObject;
		myMaxSpeed = maxSpeeds[Random.Range(0, maxSpeeds.Length)];
		start = transform.position;
		end = new Vector3(transform.position.x,
			transform.position.y,
			transform.position.z - enterDistance);
	}




	/// <summary>
	/// Decide which player to chase.
	/// 
	/// Boss helpers make this decision based on whether they're coming from the left (1P) or right (2P).
	/// </summary>
	/// <returns>The target.</returns>
	private Transform ChooseTarget(){
		if (transform.position.x <= 0.0f) { return playerOrganizer.Find(PLAYER_1); }
		else { return playerOrganizer.Find(PLAYER_2); }
	}

	private void FixedUpdate(){
		//if frozen, this enemy is immobilized. It retains its old force. 
		if (frozen){
			frozen = RunFreezeTimer();
			rb.isKinematic = true;
			return; //don't gain force, change direction, or build toward charging forward while frozen
		} else if (!frozen){
			rb.isKinematic = false; //when not frozen, the enemy can move
		}

		if (enteringScreen){
			rb.MovePosition(MoveOntoScreen());
		} else {
			rb.AddForce(GetDirection() * Speed, ForceMode.Force);

			//This is a bodge to limit maximum speed. The better way would be to impose a countervailing force.
			//Directly manipulating rigidbody velocity could lead to physics problems.
			if (rb.velocity.magnitude > myMaxSpeed) { rb.velocity = rb.velocity.normalized * myMaxSpeed; }
		}
	}

	private Vector3 MoveOntoScreen(){
		timer += Time.deltaTime;

		Vector3 pos = Vector3.LerpUnclamped(start, end, enterCurve.Evaluate(timer/enterTime));

		if (Vector3.Distance(pos, end) <= Mathf.Epsilon) { enteringScreen = false; }

		return pos;
	}

	private Vector3 GetDirection(){
		if (target != null){
			return (target.position - transform.position).normalized;
		} else {
			return transform.position; //stop in place if the player this enemy is searching for has been destroyed
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

		start = transform.position;
		end = new Vector3(transform.position.x,
						  transform.position.y,
						  transform.position.z - enterDistance);

		GetComponent<Rigidbody>().velocity = startVelocity; //sanity check: make absolutely sure the velocity is zero
	}
}
