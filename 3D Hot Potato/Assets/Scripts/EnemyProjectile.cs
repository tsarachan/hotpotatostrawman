/*
 * 
 * A simple script for simple enemies! This enemy doesn't do anything except fly in a direction set by some
 * other script, and go back to the object pool if nothing destroys it.
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : EnemyBase {

	//----------Tunable variables----------

	public float existDuration = 2.0f; //how long this enemy stays in the scene before going back to the pool


	//----------Internal variables----------

	private float existTimer = 0.0f;
	private const string ENEMY_ORGANIZER = "Enemies";


	//initialize variables
	private void Start(){
		transform.parent = GameObject.Find(ENEMY_ORGANIZER).transform;
	}


	/// <summary>
	/// When this enemy's time runs out, send it back to the object pool.
	/// 
	/// This doesn't do the nice particle explosion; there's no need, since it's offscreen at this point anyway.
	/// </summary>
	private void Update(){
		existTimer += Time.deltaTime;

		if (existTimer >= existDuration){
			ObjectPooling.ObjectPool.AddObj(gameObject);
		}
	}

	public override void GetDestroyed(){
		GetComponent<ParticleBurst>().MakeBurst();
		ObjectPooling.ObjectPool.AddObj(gameObject); //shut this off and return it to the pool
	}

	public override void Reset(){
		gameObject.SetActive(true);
		GetComponent<Rigidbody>().velocity = startVelocity; //sanity check: make absolutely sure the velocity is zero

		existTimer = 0.0f;
	}
}
