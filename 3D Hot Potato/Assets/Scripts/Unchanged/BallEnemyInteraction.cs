using UnityEngine;
using System.Collections;

public class BallEnemyInteraction : MonoBehaviour {

	private const string ENEMY_OBJ = "Enemy";
	private BallBehavior ballScript;


	private void Start(){
		ballScript = GetComponent<BallBehavior>();
	}

	private void OnTriggerEnter(Collider other){
		if (other.gameObject.name.Contains(ENEMY_OBJ) && ballScript.InAir){
			Services.EventManager.Fire(new EnemyDestroyedEvent(other.gameObject, gameObject));
			other.gameObject.GetComponent<EnemyBase>().GetDestroyed();
		}
	}
}
