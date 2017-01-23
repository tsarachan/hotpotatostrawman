using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatchBehavior : MonoBehaviour {


	//tunable variables--how close the catch has to be, how strong the burst is, etc.
	public float awesomeCatchDistance = 1.0f;
	public float explosionRadius = 5.0f;
	public float explosionForce = 100.0f;


	//variables for determining who has the ball
	private PlayerBallInteraction myBallScript;
	private PlayerBallInteraction otherBallScript;
	private const string PLAYER_1_OBJ = "Player 1";
	private const string PLAYER_2_OBJ = "Player 2";


	//variables relating to the ball
	private Transform ball;
	private const string BALL_OBJ = "Ball";


	//variables for finding nearby enemies
	private Transform enemies;
	private const string ENEMY_ORGANIZER = "Enemies";
	private int enemyLayer = 9;
	private int enemyLayerMask;


	private void Start(){
		myBallScript = GetComponent<PlayerBallInteraction>();
		otherBallScript = FindOtherBallScript();
		ball = GameObject.Find(BALL_OBJ).transform;
		enemies = GameObject.Find(ENEMY_ORGANIZER).transform;
		enemyLayerMask = 1 << enemyLayer;
	}


	private PlayerBallInteraction FindOtherBallScript(){
		PlayerBallInteraction temp;

		if (gameObject.name == PLAYER_1_OBJ){
			temp = GameObject.Find(PLAYER_2_OBJ).GetComponent<PlayerBallInteraction>();
		} else if (gameObject.name == PLAYER_2_OBJ){
			temp = GameObject.Find(PLAYER_1_OBJ).GetComponent<PlayerBallInteraction>();
		} else {
			//emergency initialization for when something goes wrong
			temp = GameObject.Find(PLAYER_2_OBJ).GetComponent<PlayerBallInteraction>();
			Debug.Log("Unable to find the other player's PlayerBallInteraction");
		}

		return temp;
	}


	public void AttemptSpecialCatch(){
		Debug.Log("Attemping special catch");
		if (!myBallScript.BallCarrier && !otherBallScript.BallCarrier){ //neither player has the ball--it's in the air
			if (Vector3.Distance(transform.position, ball.position) <= awesomeCatchDistance){
				SpecialCatch();
				Debug.Log("Close enough for special catch");
			}
		}
	}


	private void SpecialCatch(){
		Collider[] enemies = Physics.OverlapSphere(transform.position,
												   explosionRadius,
												   enemyLayerMask);
		
		Debug.Log("found " + enemies.Length + " enemies");

		foreach (Collider enemy in enemies){
			Vector3 direction = (enemy.transform.position - transform.position).normalized;

			enemy.attachedRigidbody.AddForce(direction * explosionForce,
											 ForceMode.Impulse);
		}
	}
}
