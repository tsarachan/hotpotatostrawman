/*
 * 
 * A simple script for the little cubes that are serving as particles.
 * 
 */

using UnityEngine;
using System.Collections;

public class ParticleCubeBehavior : ObjectPooling.Poolable {

	//tunable variables
	public float lifetime = 1.0f;
	public float speed = 2.0f;


	//internal variables
	private float timer = 0.0f;
	private Rigidbody rb;
	private const string ENEMY_TAG = "Enemy";


	private void Start(){
		rb = GetComponent<Rigidbody>();
	}


	private void Update(){
		rb.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);

		timer += Time.deltaTime;

		if (timer >= lifetime){
			ObjectPooling.ObjectPool.AddObj(gameObject);
		}
	}


	public void SetDirection(Vector3 direction){
		transform.rotation = Quaternion.Euler(direction);
	}


	public override void Reset(){
		base.Reset();

		//reset the timer so that this particle isn't immediately sent back to the pool
		timer = 0.0f;
	}


	private void OnTriggerEnter(Collider other){
		if (other.tag == ENEMY_TAG){
			other.gameObject.GetComponent<EnemyBase>().GetDestroyed();
			ObjectPooling.ObjectPool.AddObj(gameObject);
		}
	}
}
