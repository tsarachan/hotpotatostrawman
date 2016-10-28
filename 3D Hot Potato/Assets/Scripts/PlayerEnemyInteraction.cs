using UnityEngine;
using System.Collections;

public class PlayerEnemyInteraction : MonoBehaviour {

	private const string ENEMY_OBJ = "Enemy";
	private const string BALL_OBJ = "Ball";

	private void OnCollisionEnter(Collision collision){
		if (collision.gameObject.name.Contains(ENEMY_OBJ)){
			if (!GetComponent<PlayerBallInteraction>().BallCarrier){  //try to destroy enemies when not the ball carrier
				collision.gameObject.GetComponent<EnemyBase>().GetDestroyed();
			} else { //this player is the ball carrier; the game is over
				GetComponent<ParticleBurst>().MakeBurst();

				//de-parent the ball to avoid null reference exceptions
				if (transform.Find(BALL_OBJ) != null){
					transform.Find(BALL_OBJ).parent = transform.root;
				}

				Destroy(gameObject);
			}
		}
	}
}
