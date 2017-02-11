using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerEnemyInteraction : MonoBehaviour {

	//----------Tunable variables----------

	public float timeToResetGame = 1.0f; //how long it takes for the game to reset after losing
	public float riderTumbleSpeed = 1.0f; //how quickly the rider spins after falling off the bike
	public AnimationCurve bounceCurve; //how the rider bounces off the ground after falling
	public float bounceHeight = 1.0f; //the highest Y position the rider reaches while bouncing (lowest is road)


	//----------Internal variables----------

	//variables used to identify enemies
	private const string ENEMY_OBJ = "Enemy";
	private const string BALL_OBJ = "Ball";
	private const string HUNT = "Hunt"; //if this is in the name, it's vulnerable to the ball and destroys non-ball carriers


	//these are involved in resetting the game
	private const string MANAGER_OBJ = "Managers";
	private LevelManager levelManager;
	private Vector3 myStartPos = new Vector3(0.0f, 0.0f, 0.0f);


	//the particle displayed when a player is destroyed
	private const string DEATH_PARTICLE = "Player destruction plexus";


	//variables used to make the rider tumble off-screen
	private Transform rider;
	private Transform cycleAndRider;
	private const string RIDER_ORGANIZER = "Cycle and rider";
	private const string RIDER_OBJ = "lightrunner1";
	private Vector3 riderLocalStartPos = new Vector3(0.0f, 0.0f, 0.0f);
	private Quaternion riderLocalStartRot;
	private float riderFallSpeed = 1.0f;
	private Transform scene; //parent the rider to this when falling
	private const string SCENE_ORGANIZER = "Scene";
	private float bounceLowestPos = 0.0f;
	private const string ROAD_OBJ = "Basic_road";
	private const string IMPACT_PARTICLE = "Impact particle";


	//needed to shut things off and turn them back on correctly
	private const string POINT_LIGHT = "Point light";
	private const string CYCLE = "Model";


	//initialize variables
	private void Start(){
		levelManager = GameObject.Find(MANAGER_OBJ).GetComponent<LevelManager>();
		myStartPos = transform.position;
		cycleAndRider = transform.Find(RIDER_ORGANIZER);
		rider = cycleAndRider.Find(RIDER_OBJ);
		riderLocalStartPos = rider.localPosition;
		riderLocalStartRot = rider.localRotation;
		scene = GameObject.Find(SCENE_ORGANIZER).transform;
		bounceLowestPos = GameObject.Find(ROAD_OBJ).transform.position.y;
	}

	//debug instruction
	private void Update(){
		if (Input.GetKeyDown(KeyCode.Space)){
			LoseTheGame();
		}
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


	/// <summary>
	/// Exploding enemies call this when they catch a player in their blast radius.
	/// </summary>
	public void InAnExplosion(){
		LoseTheGame();
	}
		

	private void LoseTheGame(){
		//de-parent the ball to avoid null reference exceptions
		if (transform.Find(BALL_OBJ) != null){
			transform.Find(BALL_OBJ).parent = transform.root;
		}

		GameObject deathParticle = ObjectPooling.ObjectPool.GetObj(DEATH_PARTICLE);
		deathParticle.transform.position = transform.position;

		Color lineColor = GetComponent<Renderer>().material.color;
		lineColor.a = 1.0f;
		deathParticle.GetComponent<ParticlePlexus>().LineColor = lineColor;

		transform.Find(POINT_LIGHT).gameObject.SetActive(false); //shut off the point light
		transform.Find(RIDER_ORGANIZER).Find(CYCLE).GetComponent<Renderer>().enabled = false; //make the lightsteed disappear
		//transform.GetChild(1).GetChild(1).gameObject.SetActive(false); //shut off the rider, so that it disappears as well

		//ObjectPooling.ObjectPool.ClearPools();

		StartCoroutine(ResetGame());
	}

	private IEnumerator ResetGame(){
		levelManager.StopGame();

		rider.parent = scene; //parent the rider to something in a constant location, so it doesn't move with inputs

		float timer = 0.0f;

		GameObject impactParticle = ObjectPooling.ObjectPool.GetObj(IMPACT_PARTICLE);

		while (timer <= timeToResetGame){
			Vector3 pos = rider.transform.position + -Vector3.forward * riderFallSpeed;
			pos.y = Mathf.LerpUnclamped(bounceLowestPos, bounceHeight, bounceCurve.Evaluate(timer/timeToResetGame));

			rider.transform.position = pos;
			rider.transform.Rotate(Vector3.right * riderTumbleSpeed * Time.deltaTime);

			//create an impact particle when the rider hits the ground
			if (pos.y <= Mathf.Epsilon){
				impactParticle.transform.position = pos;
				impactParticle.GetComponent<ParticleSystem>().Play();
			}

			timer += Time.deltaTime;

			yield return null;
		}

		levelManager.RestartGame();

		yield break;
	}


	public void ResetPlayer(){
		transform.position = myStartPos;
		rider.parent = transform.Find(RIDER_ORGANIZER); //re-parent the rider
		rider.localPosition = riderLocalStartPos;
		rider.localRotation = riderLocalStartRot;

		transform.Find(POINT_LIGHT).gameObject.SetActive(true); //turn the point light back on
		transform.Find(RIDER_ORGANIZER).Find(CYCLE).GetComponent<Renderer>().enabled = true; //restore the lightsteed
//		transform.GetChild(1).GetChild(1).gameObject.SetActive(true); //bring back the rider
		GetComponent<PlayerBallInteraction>().BallCarrier = false; //without this setting, players can be destroyed without the ball on restart
	}
}
