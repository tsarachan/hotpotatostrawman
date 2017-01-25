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


	private void Start(){
		myBallScript = GetComponent<PlayerBallInteraction>();
		otherBallScript = FindOtherBallScript();
		ball = GameObject.Find(BALL_OBJ).transform;

		boltScript = transform.root.Find(PARTICLES_ORGANIZER).Find(LIGHTNING_OBJ)
			.GetComponent<DigitalRuby.LightningBolt.LightningBoltScript>();
		boltScript.SetState(false);
	}


	private void Update(){
		cantCatchTimer += Time.deltaTime;
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


	public void AttemptAwesomeCatch(){
		if (!myBallScript.BallCarrier && !otherBallScript.BallCarrier &&  //neither player has the ball--it's in the air
			cantCatchTimer > cantCatchAfterThrow){ //discard thrower's extra inputs
			if (Vector3.Distance(transform.position, ball.position) <= awesomeCatchDistance){
				AwesomeCatch();
			}
		}
	}


	private void AwesomeCatch(){
		//determine whether the short range or long range effect should happen
		bool isShortRange = true;

		if (Vector3.Distance(transform.position, otherBallScript.transform.position) > shortRange){
			isShortRange = false;
		}

		if (isShortRange) {
			ShortRangeAwesomeEffect();
		} else {
			//apply the long-range effect to enemies caught in the burst
			LongRangeAwesomeEffect();
		}
		cantCatchTimer = 0.0f;

	}


	private void ShortRangeAwesomeEffect(){
		boltScript.SetState(true);
	}


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
	/// <returns><c>true</c> if this instance cant special catch; otherwise, <c>false</c>.</returns>
	public void CantAwesomeCatch(){
		cantCatchTimer = 0.0f;
	}
}
