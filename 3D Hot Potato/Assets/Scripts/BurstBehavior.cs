/*
 * 
 * This script is on the bursts players create when they get an awesome catch at long range.
 * 
 */
using System.Collections;
using UnityEngine;

public class BurstBehavior : ObjectPooling.Poolable {

	//variables relating to how long the burst stays in play
	public float existDuration = 2.0f;
	private float existTimer = 0.0f;
	private float effectiveDuration = 1.5f; //how long the burst can destroy enemies


	//the size of the blast
	public float explosionRadius = 10.0f;


	//variables for finding nearby enemies
	private Transform enemies;
	private const string ENEMY_ORGANIZER = "Enemies";
	private int enemyLayer = 9;
	private int enemyLayerMask;


	//variables involved in the burst's movement offscreen
	public AnimationCurve moveCurve;
	public float maxSpeed = 10.0f;


	//internal variables
	private const string PARTICLES_ORGANIZER = "Particles";


	//initialize variables
	private void Start(){
		transform.parent = GameObject.Find(PARTICLES_ORGANIZER).transform;
		enemyLayerMask = 1 << enemyLayer;
	}


	//handle timers and move the burst backwards off the screen
	private void Update(){
		existTimer = ManageLifetime();
		transform.position = SlideBackwards();

		if (existTimer <= effectiveDuration){
			BlowAwayEnemies();
		}
	}


	/// <summary>
	/// Keeps track of how long the burst has been in play. When the timer is up, return the burst to
	/// the object pool.
	/// </summary>
	/// <returns>The updated timer.</returns>
	private float ManageLifetime(){
		float temp = existTimer;

		temp += Time.deltaTime;

		if (temp >= existDuration){
			temp = 0.0f;
			ObjectPooling.ObjectPool.AddObj(gameObject); //shut this off and return it to the object pool
		}

		return temp;
	}


	/// <summary>
	/// Calculates how quickly the burst should move. This doesn't *move* the burst--that happens
	/// in Update(). It only determines how fast the burst should be going.
	/// </summary>
	/// <returns>The burst's next position.</returns>
	private Vector3 SlideBackwards(){
		float speed = Mathf.Lerp(0.0f, maxSpeed, moveCurve.Evaluate(existTimer/existDuration));
		
		Vector3 temp = new Vector3(transform.position.x,
								   transform.position.y,
								   transform.position.z - speed);

		return temp;
	}


	/// <summary>
	/// Finds enemies within the burst so that something bad can happen to them.
	/// </summary>
	private void BlowAwayEnemies(){
		Collider[] enemies = Physics.OverlapSphere(transform.position,
												   explosionRadius,
												   enemyLayerMask);

		foreach (Collider enemy in enemies){
			BlowAwayEffect(enemy.gameObject);
		}
		
	}


	/// <summary>
	/// Whatever is going to happen to enemies caught in the burst goes here.
	/// </summary>
	/// <param name="enemy">The enemy to be affected.</param>
	private void BlowAwayEffect(GameObject enemy){
		enemy.GetComponent<EnemyBase>().GetDestroyed();
	}


	//override function to remove Poolable's reference to velocity
	public override void Reset(){
		gameObject.SetActive(true);
	}


	//override function to remove Poolable' reference to velocity
	public override void ShutOff(){
		gameObject.SetActive(false);
	}
}
