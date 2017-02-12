/*
 * 
 * A small script for objects that should go into and come out of an object pool.
 * 
 */
using System.Collections;
using UnityEngine;

public class LifetimeLimiter : ObjectPooling.Poolable {

	public string parent;
	public float lifetime = 2.0f;
	private float timer = 0.0f;


	private void Start(){
		transform.parent = GameObject.Find(parent).transform;
	}


	private void Update(){
		timer += Time.deltaTime;

		if (timer >= lifetime){
			ObjectPooling.ObjectPool.AddObj(gameObject);
		}
	}


	public override void Reset(){
		timer = 0.0f; //sanity check; make sure the timer is at zero, so the object doesn't instantly disappear
		gameObject.SetActive(true);
	}


	public override void ShutOff(){
		timer = 0.0f;
		gameObject.SetActive(false);
	}
}
