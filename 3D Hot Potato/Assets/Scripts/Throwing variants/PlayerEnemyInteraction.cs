using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerEnemyInteraction : MonoBehaviour {

	public float timeToResetGame = 1.0f;

	private const string ENEMY_OBJ = "Enemy";
	private const string BALL_OBJ = "Ball";

	private void OnCollisionEnter(Collision collision){
		if (collision.gameObject.name.Contains(ENEMY_OBJ)){
			if (GetComponent<PlayerBallInteraction>().BallCarrier){  //try to destroy enemies when the ball carrier
				collision.gameObject.GetComponent<EnemyBase>().GetDestroyed();
				GetComponent<PlayerMovement>().Stopped = true;
			} else { //this player is not the ball carrier; the game is over
				LoseTheGame();
			}
		}
	}

	private void OnTriggerEnter(Collider other){
		if (other.gameObject.name.Contains(ENEMY_OBJ)){
			if (GetComponent<PlayerBallInteraction>().BallCarrier){  //try to destroy enemies when the ball carrier
				other.gameObject.GetComponent<EnemyBase>().GetDestroyed();
				GetComponent<PlayerMovement>().Stopped = true;
			} else { //this player is the ball carrier; the game is over
				LoseTheGame();
			}
		}
	}

	private void LoseTheGame(){
		GetComponent<ParticleBurst>().MakeBurst(); //throw some particles

		//de-parent the ball to avoid null reference exceptions
		if (transform.Find(BALL_OBJ) != null){
			transform.Find(BALL_OBJ).parent = transform.root;
		}

		GetComponent<Renderer>().enabled = false; //make the player disappear
		transform.GetChild(0).gameObject.SetActive(false); //shut off the point light

		StartCoroutine(ResetGame());
	}

	private IEnumerator ResetGame(){
		yield return new WaitForSeconds(timeToResetGame);


		SceneManager.LoadScene(SceneManager.GetActiveScene().name);

		yield break;
	}
}
