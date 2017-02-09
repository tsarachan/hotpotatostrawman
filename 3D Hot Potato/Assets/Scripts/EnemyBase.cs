/*
 * 
 * This is the base class for all enemies. All enemies inherit from this class.
 * 
 * This class inherits from Poolable, which takes care of the requirements for object pooling.
 * 
 */

using UnityEngine;
using System.Collections;

public abstract class EnemyBase : ObjectPooling.Poolable {

	protected const string PLAYER_OBJ = "Player";

	protected bool frozen = false;
	protected float freezeDuration = 0.0f;
	protected float freezeTimer = 0.0f;


	//the particle that enemies generate when they're destroyed
	protected const string DESTROY_PARTICLE = "Plexus prefab";


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


	/// <summary>
	/// Most enemies can be destroyed. This provides a standard thing that happens when another object needs to get rid of an enemy.
	/// </summary>
	public virtual void GetDestroyed(){
		gameObject.GetComponent<ObjectPooling.Poolable>().ShutOff();
	}
}
