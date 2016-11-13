using UnityEngine;
using System.Collections;

public class BuildingMove : MonoBehaviour {

	public float speed = 50.0f;
	public Vector3 startPos = new Vector3(-34.9f, 9.04f, 74.8f);
	bool gameStart;
	private bool frozen = false;
	private float freezeTimer = 0.0f;
	private float freezeDuration;

	void Start(){

	
	}
	protected virtual void Update(){
		if (!frozen){
			transform.localPosition -= transform.forward * speed * Time.deltaTime;

			if (transform.localPosition.z <= -25.0f) {
			
				transform.localPosition = startPos;
			}
		} else {
			frozen = RunFreezeTimer();
		}

	}

	/// <summary>
	/// Call this function to freeze the enemy in place.
	/// 
	/// The amount of time is passed in, rather than set in a variable here, so that it can easily be made consistent across enemy types.
	/// Changing the public variable in PlayerMovement changes the value for all enemies.
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
