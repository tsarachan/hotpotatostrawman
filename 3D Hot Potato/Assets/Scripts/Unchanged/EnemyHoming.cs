﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

	public float onScreenTime = 10.0f;
	private float stayTimer = 0.0f;

	private Vector3 direction;
	private Light myPointLight;
	public Color leavingScreenColor;


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
		myPointLight = transform.GetChild(0).GetComponent<Light>();
	}




	/// <summary>
	/// Decide which player to chase.
	/// </summary>
	/// <returns>The target.</returns>
	private Transform ChooseTarget(){
		float randomValue = Random.Range(0.0f, 1.0f); //if this is <= chanceOfGoForBall, the enemy will want to chase the player with the ball
		Transform target = transform; //default initialization for error-checking


		if (randomValue <= chanceOfGoForBall){ //enemy wants to go for the ball
			foreach(Transform player in playerOrganizer){
				if (player.GetComponent<PlayerBallInteraction>().BallCarrier){
					target = player;
					break;
				}
			}
		} else { //didn't want to go for the ball; go for the other player, if possible

			//this section is a little more complicated so that the enemies don't swarm player 1 when there's not a ball carrier
			//(e.g., because it's the start of the game, or the ball is being passed)

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
		}

		//Was the target set to a player? If not (e.g., wanted to chase the ball carrier but there wasn't one), choose a random player to chase
		if (target == transform){
			target = playerOrganizer.GetChild(Random.Range(0, playerOrganizer.childCount));
		}

		//error check: send a message if the target wasn't set
		if (target == transform) { Debug.Log("Couldn't find a target!"); }

		Debug.Log(target);
		return target;
	}

	private void FixedUpdate(){
		if (enteringScreen){
			rb.MovePosition(MoveOntoScreen());
		} else {
			if (stayTimer <= onScreenTime){
				direction = GetDirection();
				stayTimer += Time.deltaTime;
				rb.AddForce(direction * Speed, ForceMode.Force);
			} else {
				//stop changing direction; the enemy goes off-screen
				rb.AddForce(direction * Speed * 2, ForceMode.Force);
				myPointLight.color = leavingScreenColor;
				myPointLight.intensity = 8.0f; //set the light to maximum intensity to signify danger
			}

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
		GetComponent<ParticleBurst>().MakeBurst();
		Destroy(gameObject);
	}
}
