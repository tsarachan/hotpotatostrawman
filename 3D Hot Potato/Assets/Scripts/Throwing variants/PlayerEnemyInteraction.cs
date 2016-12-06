using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerEnemyInteraction : MonoBehaviour {

	public float timeToResetGame = 1.0f;

	private const string ENEMY_OBJ = "Enemy";
	private const string BALL_OBJ = "Ball";
	private const string HUNT = "Hunt"; //if this is in the name, it's vulnerable to the ball and destroys non-ball carriers

	private void OnCollisionEnter(Collision collision){
		if (collision.gameObject.name.Contains(ENEMY_OBJ)){
			if (collision.gameObject.name.Contains(HUNT)){
				if (!GetComponent<PlayerBallInteraction>().BallCarrier){  //lose when you get caught by a hunt enemy
					LoseTheGame();
				} else { //destroy hunt enemies when the ballcarrier
					collision.gameObject.GetComponent<EnemyBase>().GetDestroyed();
					GetComponent<PlayerMovement>().Stopped = true;
				}
			}
			else if (!GetComponent<PlayerBallInteraction>().BallCarrier){  //try to destroy other enemies when not the ball carrier
				collision.gameObject.GetComponent<EnemyBase>().GetDestroyed();
				GetComponent<PlayerMovement>().Stopped = true;
			} else { //this player is the ball carrier, and got caught by a non-hunt enemy; the game is over
				LoseTheGame();
			}
		}
	}

	private void OnTriggerEnter(Collider other){
		if (other.gameObject.name.Contains(ENEMY_OBJ)){
			if (!GetComponent<PlayerBallInteraction>().BallCarrier){  //try to destroy non-hunt enemies when not the ball carrier
				other.gameObject.GetComponent<EnemyBase>().GetDestroyed();
				GetComponent<PlayerMovement>().Stopped = true;
			} else { //this player is the ball carrier and got caught by a non-hunt enemy; the game is over
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
			
		transform.GetChild(0).gameObject.SetActive(false); //shut off the point light
		transform.GetChild(1).GetComponent<Renderer>().enabled = false; GetComponent<Renderer>().enabled = false; //make the lightsteed disappear
		transform.GetChild(2).gameObject.SetActive(false); //shut off the rider, so that it disappears as well

		ObjectPooling.ObjectPool.ClearPools();

		StartCoroutine(ResetGame());
	}

	private IEnumerator ResetGame(){
		yield return new WaitForSeconds(timeToResetGame);


		SceneManager.LoadScene(SceneManager.GetActiveScene().name);

		yield break;
	}
}
