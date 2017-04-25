using UnityEngine;
using System.Collections;

public class BallEnemyInteraction : MonoBehaviour {

	private const string ENEMY_OBJ = "Enemy";

	private void OnTriggerEnter(Collider other){
		if (other.gameObject.name.Contains(ENEMY_OBJ)){
			Debug.Log(gameObject.name + " destroyed " + other.gameObject.name);
			Services.EventManager.Fire(new EnemyDestroyedEvent(other.gameObject, gameObject));
			other.gameObject.GetComponent<EnemyBase>().GetDestroyed();
		}
	}
}
