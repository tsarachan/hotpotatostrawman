/*
 * 
 * Everything that uses an object pool--in other words, all enemies--inherits from this script.
 * 
 * This script assumes that all enemies have a rigidbody!
 * 
 */

namespace ObjectPooling
{
	using UnityEngine;
	using System.Collections;

	public abstract class Poolable : MonoBehaviour {


		public Vector3 startVelocity = new Vector3(0.0f, 0.0f, 0.0f);


		/// <summary>
		/// Call this function to restore default values when an enemy comes out of the pool and into play.
		/// 
		/// Call this *before* the enemy is moved into position, so that everything is in a predictable state when the enemy's own script takes over.
		/// </summary>
		public virtual void Reset(){
			gameObject.SetActive(true);

			GetComponent<Rigidbody>().velocity = startVelocity; //sanity check: make absolutely sure the velocity is zero
		}

		/// <summary>
		/// Call this function when an enemy is going back to the pool.
		/// </summary>
		public virtual void ShutOff(){
			//stop the enemy in preparation for it to move normally again
			//this avoids having to deal with the existing velocity case-by-case; it's always going to start at zero
			GetComponent<Rigidbody>().velocity = startVelocity;

			gameObject.SetActive(false); //shut the enemy off so that it doesn't move, etc.
		}
	}
}
