using UnityEngine;
using System.Collections;

public class EnemyHoming : EnemyBase {

	public float chanceOfGoForBall = 0.8f; //between 1.0 and 0.0
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

	private const string ENEMY_ORGANIZER = "Enemies";

	private void Start(){
		transform.parent = GameObject.Find(ENEMY_ORGANIZER).transform;
		playerOrganizer = GameObject.Find(PLAYER_ORGANIZER).transform;
		target = ChooseTarget();
		rb = GetComponent<Rigidbody>();
		//destroyParticle = Resources.Load("DestroyParticle") as GameObject;
		myMaxSpeed = maxSpeeds[Random.Range(0, maxSpeeds.Length)];
	}


	/// <summary>
	/// Decide which player to chase.
	/// </summary>
	/// <returns>The target.</returns>
	private Transform ChooseTarget(){
		float randomValue = Random.Range(0.0f, 1.0f); //if this is <= chanceOfGoForBall, the enemy will want to chase the ball


		if (randomValue <= chanceOfGoForBall){ //enemy wants to go for the ball
			return GameObject.Find(BALL_OBJ).transform;
		} else { //didn't want to go for the ball; chase a random player
			return playerOrganizer.GetChild(Random.Range(0, playerOrganizer.childCount));
		}
	}

	private void FixedUpdate(){
		rb.AddForce(Vector3.ClampMagnitude(GetDirection() * speed, myMaxSpeed), ForceMode.Force);
	}

	private Vector3 GetDirection(){
		return (target.position - transform.position).normalized;
	}
}
