using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaberShieldBehavior : MonoBehaviour {


	private const string ENEMY_TAG = "Enemy";

	private void OnTriggerEnter(Collider other){
		if (other.tag == ENEMY_TAG){
			transform.parent.GetComponent<LightsaberBehavior>().DestroyEnemy(other.gameObject.GetComponent<EnemyBase>());
		}
	}
}
