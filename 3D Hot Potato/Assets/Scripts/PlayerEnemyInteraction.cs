using UnityEngine;
using System.Collections;

public class PlayerEnemyInteraction : MonoBehaviour {


	private void OnCollisionEnter(Collision collision){
		if (collision.gameObject.name.Contains("Enemy")){
			if (!GetComponent<PlayerBallInteraction>().BallCarrier){  //try to destroy enemies when not the ball carrier
				collision.gameObject.GetComponent<EnemyBase>().GetDestroyed();
			} else {
				//do something to lose the game
			}
		}
	}
}
