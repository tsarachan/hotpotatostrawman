using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyGiant : EnemyBase {

	private Rigidbody rb;

	private Transform playerOrganizer;
	private const string PLAYER_ORGANIZER = "Players";
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

	private void Start(){
		transform.parent = GameObject.Find(ENEMY_ORGANIZER).transform;
		playerOrganizer = GameObject.Find(PLAYER_ORGANIZER).transform;
		ball = GameObject.Find(BALL_OBJ).transform;

		//line this enemy up with a player who doesn't have the ball
		transform.position = ChooseTarget().position;

		rb = GetComponent<Rigidbody>();
		start = transform.position;
		end = new Vector3(transform.position.x,
						  transform.position.y,
						  transform.position.z - enterDistance);
	}

	private Transform ChooseTarget(){
		Transform target = transform; //default initialization for error-checking

		//get a list of all players
		List<Transform> players = new List<Transform>();
		foreach(Transform player in playerOrganizer){
			players.Add(player);
		}

		//take the ball carrier out of the list, if there is one
		foreach (Transform player in playerOrganizer){
			if (player.GetComponent<PlayerBallInteraction>().BallCarrier){
				players.Remove(player);
			}
		}

		//choose a random player to get the target among those who are left
		target = players[Random.Range(0, players.Count)];

		//error check: send a message if the target wasn't set
		if (target == transform) { Debug.Log("Couldn't find target!"); }

		return target;
	}

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


	//this is intentionally blank; giant enemies can't be destroyed by blocking them
	public override void GetDestroyed(){

	}
}
