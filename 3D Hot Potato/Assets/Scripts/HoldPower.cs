/*
 * 
 * Base class for hold powers.
 * 
 */
using System.Collections;
using UnityEngine;

public abstract class HoldPower : MonoBehaviour {


	//tunable variables
	public float holdDuration = 10.0f; //how long the player has to hold the battery star to trigger the hold power


	//internal variables
	protected PlayerBallInteraction ballScript;
	protected float holdTimer = 0.0f; //how long this player has been holding the battery star


	//initialize variables
	protected virtual void Start(){
		ballScript = GetComponent<PlayerBallInteraction>();
	}


	//keep track of how long the player has been holding the ball; activate the hold power when it's been long enough
	protected virtual void Update(){
		holdTimer = RunHoldTimer();

		if (holdTimer > holdDuration){
			ActivateHoldPower();
		}
	}


	//returns the new value for the hold timer. If the player isn't holding the ball, returns zero.
	protected float RunHoldTimer(){
		if (!ballScript.BallCarrier){
			return 0.0f;
		} else {
			float temp = holdTimer;

			temp += Time.deltaTime; 

			return temp;
		}
	}


	//override this to establish what happens when the player's hold power goes off
	protected abstract void ActivateHoldPower();
}
