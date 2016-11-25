/*
 * 
 * Attach this script to anything that stays in play for a limited amount of time to return it to the object pool when time runs out.
 * 
 */

using UnityEngine;
using System.Collections;

public class TimeLimit : ObjectPooling.Poolable {

	public float lifetime = 1.0f;
	private float timer = 0.0f;

	private void Update(){
		timer += Time.deltaTime;

		if (timer >= lifetime){
			ObjectPooling.ObjectPool.AddObj(gameObject);
		}
	}
}
