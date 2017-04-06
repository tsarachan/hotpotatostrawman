using UnityEngine;
using System.Collections;

public class EnvironmentMove : MonoBehaviour {

	public float speed = 50.0f;
	public Vector3 startPos = new Vector3(-34.9f, 9.04f, 74.8f);
	public float resetZ = -25.0f; //the z-position where this object will cycle back to the front

	//if freezing enemies is in the game, these variables freeze the environment around them
	protected bool frozen = false;
	protected float freezeTimer = 0.0f;
	protected float freezeDuration;

	//these stop the buildings when the game hasn't started, and then bring the buildings up to speed
	//when the game gets underway
	protected bool gameHasStarted = false;
	public bool GameHasStarted{
		get { return gameHasStarted; }
		set { gameHasStarted = value; }
	}
	protected float currentSpeed = 0.0f;
	public AnimationCurve startMovingCurve;
	protected float timer = 0.0f;
	public float timeToFullSpeed = 1.0f;


	protected virtual void Update(){
		if (!frozen && GameHasStarted){
			currentSpeed = GetCurrentSpeed();

			transform.localPosition -= Vector3.forward * currentSpeed * Time.deltaTime;

			if (transform.localPosition.z <= resetZ) {
			
				transform.localPosition = startPos;
			}
		} else {
			frozen = RunFreezeTimer();
		}

	}


	protected float GetCurrentSpeed(){
		timer += Time.deltaTime;

		return Mathf.Lerp(0.0f, speed, startMovingCurve.Evaluate(timer/timeToFullSpeed));
	}

	/// <summary>
	/// Call this function when enemies are frozen in place.
	/// 
	/// The amount of time is passed in, rather than set in a variable here, so that it can easily be made consistent across enemy types.
	/// If freezing is in the game, the amount of time all enemies are frozen will be a public variable in PlayerMovement.
	/// </summary>
	/// <param name="time">How long the enemy will remain frozen.</param>
	public virtual void Freeze(float time){
		frozen = true;
		freezeDuration = time;
	}

	protected virtual bool RunFreezeTimer(){
		freezeTimer += Time.deltaTime;

		if (freezeTimer >= freezeDuration){
			freezeTimer = 0.0f;
			return false;
		} else {
			return true;
		}
	}
}
