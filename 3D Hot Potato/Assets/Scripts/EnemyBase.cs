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

	public virtual void GetDestroyed(){
		Destroy(gameObject);
	}
}
