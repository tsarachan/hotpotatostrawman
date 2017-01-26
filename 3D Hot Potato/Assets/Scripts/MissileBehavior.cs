using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileBehavior : MonoBehaviour {

	public float speed = 4.0f; //how quickly the missile moves in the target direction

	private Transform enemies;
	private const string ENEMY_ORGANIZER = "Enemies";

	private Vector3 targetVector;
	private bool launched = false;
	private Rigidbody rb;

	private const string ENEMY_TAG = "Enemy";

	private void Start(){
		enemies = transform.root.Find(ENEMY_ORGANIZER);
		rb = GetComponent<Rigidbody>();
	}

	private void Update(){
		if (launched){
			rb.MovePosition(transform.position + targetVector * speed * Time.deltaTime);
		}
	}

	public void Launch(){
		Debug.Log("Launch() called for " + gameObject.name);
		transform.parent = transform.root; //de-parent the missile so that it doesn't move with the player anymore

		//find a target
		if (enemies.childCount > 0){
			Transform targetEnemy = enemies.GetChild(Random.Range(0, enemies.childCount));

			targetVector = (targetEnemy.position - transform.position).normalized;
		} else {
			targetVector = Vector3.forward;
		}

		launched = true;
	}
		

	private void OnTriggerEnter(Collider other){
		if (other.tag == ENEMY_TAG){
			other.gameObject.GetComponent<EnemyBase>().GetDestroyed();
		}
	}
}
