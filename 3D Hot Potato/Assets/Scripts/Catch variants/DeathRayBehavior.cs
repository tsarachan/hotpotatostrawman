using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathRayBehavior : MonoBehaviour {

	//----------Tunable variables----------


	//the length of time the death ray fires for, before it shuts itself off
	public float activeDuration = 2.0f;


	//the radius of the sphere that's used to destroy enemies
	public float radius = 1.0f;


	//the range of the spherecast that destroys enemies
	//this needs to be matched, at least roughly, to the range of this object's particle effect
	public float range = 30.0f;


	//----------Internal variables----------


	//tracks how long the death ray will fire
	private float timer = 0.0f;


	//the players, whose positions determine where the death ray will aim
	private Transform player1;
	private Transform player2;
	private const string PLAYER_1 = "Player 1";
	private const string PLAYER_2 = "Player 2";


	//a layermask used to ensure that enemies, and only enemies, get blasted
	private int enemyLayer = 9;
	private int enemyLayerMask;


	//initialize variables
	private void Awake(){
		player1 = GameObject.Find(PLAYER_1).transform;
		player2 = GameObject.Find(PLAYER_2).transform;

		enemyLayerMask = 1 << enemyLayer;
	}


	/// <summary>
	/// Handle everything that's involved in the death ray being active. Turn it, have it attack, and
	/// track how long it should remain active.
	/// </summary>
	private void Update(){
		transform.rotation = Quaternion.LookRotation((player1.position - player2.position).normalized);

		BlastEnemies();

		timer += Time.deltaTime;

		if (timer >= activeDuration){
			gameObject.SetActive(false);
		}
	}


	/// <summary>
	/// Finds and destroys enemies
	/// </summary>
	private void BlastEnemies(){
		RaycastHit[] hitInfo = Physics.SphereCastAll(player1.position, 
													 radius,
													 (player1.position - player2.position).normalized,
													 range,
													 enemyLayerMask,
													 QueryTriggerInteraction.Ignore);

		List<EnemyBase> hitEnemies = new List<EnemyBase>();

		foreach (RaycastHit hit in hitInfo){
			if (hit.collider.tag == "Enemy"){
				hitEnemies.Add(hit.collider.GetComponent<EnemyBase>());
			}
		}

		for (int i = hitEnemies.Count - 1; i >= 0; i--){
			hitEnemies[i].GetDestroyed();
		}
	}


	/// <summary>
	/// Activate the death ray, resetting everything necessary in the process.
	/// </summary>
	public void Activate(){
		timer = 0.0f;
		transform.rotation = Quaternion.LookRotation((player1.position - player2.position).normalized);
		gameObject.SetActive(true);
	}
}
