using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerEnemyInteraction : MonoBehaviour {

	//----------Tunable variables----------

	public float timeToResetGame = 1.0f; //how long it takes for the game to reset after losing


	//----------Internal variables----------

	//variables used to identify enemies
	private const string ENEMY_OBJ = "Enemy";
	private const string BALL_OBJ = "Ball";
	private const string HUNT = "Hunt"; //if this is in the name, it's vulnerable to the ball and destroys non-ball carriers


	//these are involved in resetting the game
	private const string MANAGER_OBJ = "Managers";
	private LevelManager levelManager;
	private Vector3 myStartPos = new Vector3(0.0f, 0.0f, 0.0f);


	//initialize variables
	private void Start(){
		levelManager = GameObject.Find(MANAGER_OBJ).GetComponent<LevelManager>();
		myStartPos = transform.position;
	}


	/// <summary>
	/// Handles collisions with enemies that have non-trigger colliders.
	/// </summary>
	/// <param name="collision">The collision.</param>
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



	/// <summary>
	/// Handles collisions with enemies that have trigger colliders.
	/// </summary>
	/// <param name="other">The enemy's collider.</param>
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
		transform.GetChild(1).GetChild(0).GetComponent<Renderer>().enabled = false; //make the lightsteed disappear
		transform.GetChild(1).GetChild(1).gameObject.SetActive(false); //shut off the rider, so that it disappears as well

		//ObjectPooling.ObjectPool.ClearPools();

		StartCoroutine(ResetGame());
	}

	private IEnumerator ResetGame(){
		levelManager.StopGame();


		yield return new WaitForSeconds(timeToResetGame);
	
		//SceneManager.LoadScene(SceneManager.GetActiveScene().name);

		//yield return new WaitForSeconds(timeToResetGame);

		levelManager.RestartGame();

		yield break;
	}


	public void ResetPlayer(){
		transform.position = myStartPos;

		transform.GetChild(0).gameObject.SetActive(true); //turn the point light back on
		transform.GetChild(1).GetChild(0).GetComponent<Renderer>().enabled = true; //restore the lightsteed
		transform.GetChild(1).GetChild(1).gameObject.SetActive(true); //bring back the rider
		GetComponent<PlayerBallInteraction>().BallCarrier = false; //without this setting, players can be destroyed without the ball on restart
	}
}
