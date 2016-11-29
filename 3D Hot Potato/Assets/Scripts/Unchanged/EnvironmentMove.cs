using UnityEngine;
using System.Collections;

public class EnvironmentMove : MonoBehaviour {

	public float speed = 50.0f;
	public Vector3 startPos = new Vector3(-34.9f, 9.04f, 74.8f);
	public float resetZ = -25.0f; //the z-position where this object will cycle back to the front

	//if freezing enemies is in the game, these variables freeze the environment around them
	private bool frozen = false;
	private float freezeTimer = 0.0f;
	private float freezeDuration;


	protected virtual void Update(){
		if (!frozen){ //if freezing is not implemented, this will always happen
			transform.localPosition -= Vector3.forward * speed * Time.deltaTime;

			if (transform.localPosition.z <= resetZ) {
			
				transform.localPosition = startPos;
			}
		} else {
			frozen = RunFreezeTimer();
		}

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
