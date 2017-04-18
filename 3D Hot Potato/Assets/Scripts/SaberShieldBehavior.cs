using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaberShieldBehavior : MonoBehaviour {


	private int enemyLayer = 9;


	private const string ENEMY_TAG = "Enemy";


	private void OnTriggerEnter(Collider other){
		if (other.tag == ENEMY_TAG){
			if (other.gameObject.layer == enemyLayer){
				transform.parent.GetComponent<LightsaberBehavior>()
					.DestroyEnemy(other.gameObject.GetComponent<EnemyBase>());
			}
		}
	}
}
