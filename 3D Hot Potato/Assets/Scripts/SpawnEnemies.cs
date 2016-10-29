using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnEnemies : MonoBehaviour {

	private List<Transform> spawners = new List<Transform>();

	[Header("When enemies should spawn")]
	public float[] spawnTimes;
	[Header("Which enemy prefab to spawn")]
	public GameObject[] enemyToSpawn;
	[Header("Which spawner--1-7, left to right")]
	public int[] whichSpawner;
	private int index = 0;
	private float timer = 0.0f;

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
		timer += Time.deltaTime;


		if (index <= spawnTimes.Length - 1 && index <= enemyToSpawn.Length - 1 && index <= whichSpawner.Length - 1){
			if (timer >= spawnTimes[index]){
				GameObject newEnemy = Instantiate(enemyToSpawn[index],
												  spawners[whichSpawner[index] - 1].position, 
												  enemyToSpawn[index].transform.rotation) as GameObject;
				index++;
			}
		}
	}
}
