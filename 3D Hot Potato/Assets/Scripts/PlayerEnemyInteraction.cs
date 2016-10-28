using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerEnemyInteraction : MonoBehaviour {

	public float timeToResetGame = 1.0f;

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

				StartCoroutine(ResetGame());
			}
		}
	}

	private IEnumerator ResetGame(){
		Debug.Log("Resetting game");
		float timer = 0.0f;

		while (timer < timeToResetGame){
			timer += Time.deltaTime;
			Debug.Log(timer);

			yield return null;
		}

		SceneManager.LoadScene(SceneManager.GetActiveScene().name);

		yield break;
	}
}
