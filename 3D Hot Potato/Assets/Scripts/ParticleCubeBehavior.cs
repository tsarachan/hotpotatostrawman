/*
 * 
 * A simple script for the little cubes that are serving as particles.
 * 
 */

using UnityEngine;
using System.Collections;

public class ParticleCubeBehavior : ObjectPooling.Poolable {

	public float lifetime = 1.0f;
	private float timer = 0.0f;

	private void Update(){
		timer += Time.deltaTime;

		if (timer >= lifetime){
			ObjectPooling.ObjectPool.AddObj(gameObject);
		}
	}

	public override void Reset(){
		base.Reset();

		//reset the timer so that this particle isn't immediately sent back to the pool
		timer = 0.0f;
	}
}
