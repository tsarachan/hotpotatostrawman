/*
 * 
 * Each player has this script. It determines when they have made an awesome catch, and then calls other functions
 * appropriately.
 * 
 * This script does not carry out the awesome catch's effect. It only establishes that an awesome catch occurred.
 * 
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatchBehavior : MonoBehaviour {


	//tunable variables--how close the catch has to be, how strong the burst is, etc.
	public float awesomeCatchDistance = 1.0f;
	public float cantCatchAfterThrow = 1.0f;
	public float shortRange = 10.0f; //within this distance is short range; greater is long range


	//variables for determining who has the ball
	private PlayerBallInteraction myBallScript;
	private PlayerBallInteraction otherBallScript;
	private const string PLAYER_1_OBJ = "Player 1";
	private const string PLAYER_2_OBJ = "Player 2";


	//variables relating to the ball
	private Transform ball;
	private const string BALL_OBJ = "Ball";


	//variables for the different types of catches
	private DigitalRuby.LightningBolt.LightningBoltScript boltScript;
	private const string PARTICLES_ORGANIZER = "Particles";
	private const string LIGHTNING_OBJ = "Lightning prefab";
	private GameObject burst;
	private const string BURST_OBJ = "Burst prefab";


	//internal variables
	private float cantCatchTimer = 0.0f;


	//initialize variables
	private void Start(){
		myBallScript = GetComponent<PlayerBallInteraction>();
		otherBallScript = FindOtherBallScript();
		ball = GameObject.Find(BALL_OBJ).transform;

		boltScript = transform.root.Find(PARTICLES_ORGANIZER).Find(LIGHTNING_OBJ)
			.GetComponent<DigitalRuby.LightningBolt.LightningBoltScript>();
		boltScript.SetState(false);
	}


	//handle timer
	private void Update(){
		cantCatchTimer += Time.deltaTime;
	}


	/// <summary>
	/// Gets a reference to the other player's ball script, so that this player can tell who has the ball.
	/// </summary>
	/// <returns>The other player's PlayerBallInteraction script.</returns>
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


	/// <summary>
	/// Call this function whenever a player tries for the awesome catch.
	/// 
	/// All the logic for whether or not awesome catches occur is in the if-statements in this function.
	/// </summary>
	public void AttemptAwesomeCatch(){
		if (!myBallScript.BallCarrier && !otherBallScript.BallCarrier &&  //neither player has the ball--it's in the air
			cantCatchTimer > cantCatchAfterThrow){ //discard thrower's extra inputs
			if (Vector3.Distance(transform.position, ball.position) <= awesomeCatchDistance){
				AwesomeCatch();
			}
		}
	}


	/// <summary>
	/// If a player makes an awesome catch, this function determines which effect to apply--the short range one or
	/// the long range one.
	/// 
	/// It then uses the cantCatchTimer to prevent multiple awesome catches when the player holds the button down
	/// for more than a frame.
	/// </summary>
	private void AwesomeCatch(){
		bool isShortRange = true;

		if (Vector3.Distance(transform.position, otherBallScript.transform.position) > shortRange){
			isShortRange = false;
		}

		if (isShortRange) {
			ShortRangeAwesomeEffect();
		} else {
			LongRangeAwesomeEffect();
		}
		cantCatchTimer = 0.0f;

	}


	/// <summary>
	/// Sets the short-range awesome catch effect in motion.
	/// </summary>
	private void ShortRangeAwesomeEffect(){
		boltScript.SetState(true);
	}


	/// <summary>
	/// Sets the long-range awesome catch effect in motion.
	/// </summary>
	private void LongRangeAwesomeEffect(){
		burst = ObjectPooling.ObjectPool.GetObj(BURST_OBJ);
		burst.transform.position = transform.position;
	}


	/// <summary>
	/// This timer helps avoid accidental awesome catches when the player holds the button for a few frames.
	/// 
	/// PlayerBallInteraction's Throw() resets this every time the player throws.
	/// It's also called every time the player makes an awesome catch.
	/// </summary>
	public void CantAwesomeCatch(){
		cantCatchTimer = 0.0f;
	}
}
