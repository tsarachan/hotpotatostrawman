using System.Collections;
using UnityEngine;

public class ConstructBehavior : MonoBehaviour {

	private const string ENEMY_TAG = "Enemy";

	private void OnTriggerEnter(Collider other){
		if (other.tag == ENEMY_TAG){
			other.gameObject.GetComponent<EnemyBase>().GetDestroyed();
		}
	}
}
