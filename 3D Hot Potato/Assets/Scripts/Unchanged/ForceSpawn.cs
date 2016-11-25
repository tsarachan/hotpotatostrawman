using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ForceSpawn : MonoBehaviour {

	[Header("Which enemies can you spawn?")]
	public GameObject[] enemyToSpawn;

	private List<Transform> spawners = new List<Transform>();

	private void Start(){
		spawners = FindSpawners();
	}

	private List<Transform> FindSpawners(){
		List<Transform> temp = new List<Transform>();

		foreach (Transform spawner in transform){
			temp.Add(spawner);
		}

		if (temp.Count == 0) { Debug.Log("Couldn't find any spawners!"); }

		return temp;
	}

	private void Update(){

		//spawn a chaser at a random spawner
		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			Debug.Log("Finding a random spawner");
//			select a random spawner
//			Debug.Log("Number of spawners: " + spawners.Count);
			Transform randomSpawnerTransform = spawners[Random.Range(0, spawners.Count)];
			Debug.Log ("Got a spawner");
//			Transform randomSpawnerTransform = spawners[Random.Range(0, spawners.Count)];
			//instantiate a chaser there
			GameObject newEnemy = Instantiate (enemyToSpawn [0],
				                      randomSpawnerTransform.position,
				                      enemyToSpawn [0].transform.rotation) as GameObject;
		}

		if (Input.GetKeyDown (KeyCode.Alpha2)) {
			Debug.Log("Finding a random side spawner");
			//			select a random spawner
			//			Debug.Log("Number of spawners: " + spawners.Count);
			Transform randomSpawnerTransform = spawners[Random.Range(7, 12)];
			Debug.Log ("Got a spawner");
			//			Transform randomSpawnerTransform = spawners[Random.Range(0, spawners.Count)];
			//instantiate a chaser there
			GameObject newEnemy = Instantiate (enemyToSpawn [0],
				randomSpawnerTransform.position,
				enemyToSpawn [0].transform.rotation) as GameObject;
		}
	}
}
