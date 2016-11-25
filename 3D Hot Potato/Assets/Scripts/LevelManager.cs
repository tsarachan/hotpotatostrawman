﻿/*
 * 
 * This script loads enemies and level elements from a JSON file.
 * 
 */

/*
 * 
 * The formatting required to read JSON is a bit strange!
 * 
 * JSON can have objects and arrays. Objects look like this:
 * 
 * { "example":0 }
 * 
 * or this:
 * 
 * { "example":"text" }
 * 
 * Each of those objects has one name/value pair. Objects can have more than one name/value pair:
 * 
 * { "example":0, "text_example":"text" }
 * 
 * Arrays are denoted by square brackets: []. Arrays have objects in them.
 * 
 * Objects and arrays can be arranged hierarchically:
 * 
 * {"levels":
 * 		{"level1":
			{"act1":[
				{"action":0, "whichEnemy":"HomingEnemy", "waves":1, "whichSpawners":[1, 2, 3], "timeBetweenWaves":0, "timeVariance":0 },
				{"action":"wait5"},
			 	{"action":1, "whichEnemy":"GiantEnemy", "waves":1, "whichSpawners":[1, 2, 3], "timeBetweenWaves":0, "timeVariance":0 }
			]}
		}
	}
 * 
 * 
 * The entire hierarchy is really a big object, with a series of name/value pairs. "levels" is a name, and the value associated with that name
 * is the entire rest of the table.
 * 
 * To access each level of the hierarchy, start from the top and give either the name or the index of the next step you want to access.
 * Some examples:

	JSONNode node = JSON.Parse(FileIOUtil.ReadStringFromFile(Application.dataPath + LEVEL_PATH + FILE_NAME)); //loads the JSON file
	Debug.Log(node); //prints the entire JSON file as a string
	Debug.Log(node[LEVELS][LEVEL + "1"][ACT + actNumber.ToString()]); //prints everything acter "act1":
	Debug.Log(node[LEVELS][LEVEL + "1"][ACT + actNumber.ToString()][0]); //prints the entire first object in "act1"--all the name/value pairs
	Debug.Log(node[LEVELS][LEVEL + "1"][ACT + actNumber.ToString()][0]["action"].AsInt); //prints the number 0
 * 
 */

using SimpleJSON;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {

	//names of JSON name/value pairs--these are used to read lines from the JSON file
	private const string WORLDS = "worlds";
	private const string WORLD = "world";
	private const string ACT = "act";
	private const string ACTION = "action";
	private const string WAIT = "wait";
	private const string GROUND = "ground";
	private const string SPAWN = "spawn";
	private const string WHICH_ENEMY = "whichEnemy";
	private const string WAVES = "waves";
	private const string NUM_PER_WAVE = "numPerWave";
	private const string WHICH_SPAWNERS = "whichSpawners";
	private const string TIME_BETWEEN_WAVES = "timeBetweenWaves";
	private const string TIME_VARIANCE = "timeVariance";
	private const string TIME = "time";

	private int worldNumber = 1; //which world are we on? Starts at 1.
	private int actNumber = 1; //which act are we on? Starts at 1.
	private bool worldOver = false; //are we through all the parts of all the acts of all the world? If so, stop reading from the JSON

	private float timer = 0.0f; //the time the players have spent in the level so far
	private float nextReadTime = 0.0f; //when should the system read the next item from the JSon file?
	private int readIndex = 0; //the next thing to read in the "enemiesToMake" JSon object for the current level

	//these are used to find the JSon file in the file directory
	private const string LEVEL_PATH = "/Levels/";
	private const string FILE_NAME = "Levels.json";

	//these are used to spawn enemies
	private List<Transform> spawners = new List<Transform>();
	private const string SPAWNER_ORGANIZER = "Enemy spawners";
	private const string SPAWNER_OBJ = "Spawner ";
	private float spawnTimer = 0.0f;
	private List<float> nextSpawnTimes = new List<float>();
	private int spawnIndex = 0;
	private string enemyToMake = "";
	private int enemiesPerWave = 0;
	private List<int> possibleSpawnersThisWave = new List<int>();


	private void Start(){
		spawners = FindSpawners();
	}

	private List<Transform> FindSpawners(){
		List<Transform> temp = new List<Transform>();

		foreach (Transform spawner in GameObject.Find(SPAWNER_ORGANIZER).transform){
			temp.Add(spawner);
		}

		if (temp.Count == 0) { Debug.Log("Couldn't find any spawners!"); }

		return temp;
	}

	private void Update(){

		//this is the overall loop: whenever the timer reaches the next read time, do something
		//ReadInItem increases nextReadTime, and the cycle continues
		if (!worldOver){
			timer += Time.deltaTime;

			if (timer >= nextReadTime){
				//Debug.Log("calling ReadInItem()");
				readIndex = ReadInItem();
			}
		}

		if (spawnIndex < nextSpawnTimes.Count){
			spawnTimer += Time.deltaTime;

			if (spawnTimer >= nextSpawnTimes[spawnIndex]){
				//Debug.Log("SpawnAWave() called");
				spawnIndex = SpawnAWave();
			}
		}
	}


	/*
	 * 
	 * Read the next JSON line indicating what to do, and act accordingly.
	 * 
	 * Each different action is responsible for updating nextReadTime appropriately, according to what it's doing.
	 * 
	 * No matter which action is chosen, readIndex is handled the same way.
	 * 
	 */
	public int ReadInItem(){
		JSONNode node = JSON.Parse(FileIOUtil.ReadStringFromFile(Application.dataPath + LEVEL_PATH + FILE_NAME));
		Debug.Log("ReadInItem() called; action == " + node[WORLDS][WORLD + worldNumber.ToString()][ACT + actNumber.ToString()][readIndex][ACTION]);

		//Debug.Log("current action:" + node[WORLDS][WORLD + worldNumber.ToString()][ACT + actNumber.ToString()][readIndex][ACTION]);

		//decide what to do based on the current action
		switch (node[WORLDS][WORLD + worldNumber.ToString()][ACT + actNumber.ToString()][readIndex][ACTION]) {

			//if the action is "wait," read the next line after the indicated number of seconds.
			case WAIT:
				nextReadTime = node[WORLDS][WORLD + worldNumber.ToString()][ACT + actNumber.ToString()][readIndex][TIME].AsFloat;
				break;

			//if the action is "spawn", make enemies
			case SPAWN:
				//get the values needed to make enemies
				
				//which enemy prefab to spawn
				string enemyName = node[WORLDS][WORLD + worldNumber.ToString()][ACT + actNumber.ToString()][readIndex][WHICH_ENEMY];
				//Debug.Log("enemyName == " + enemyName);

				//how many waves
				int waves = node[WORLDS][WORLD + worldNumber.ToString()][ACT + actNumber.ToString()][readIndex][WAVES].AsInt;

				//how many enemies in each wave
				int numPerWave = node[WORLDS][WORLD + worldNumber.ToString()][ACT + actNumber.ToString()][readIndex][NUM_PER_WAVE].AsInt;

				//time between waves
				float timeBetweenWaves = 
					node[WORLDS][WORLD + worldNumber.ToString()][ACT + actNumber.ToString()][readIndex][TIME_BETWEEN_WAVES].AsFloat;

				//variance in the time between waves
				float timeVariance = node[WORLDS][WORLD + worldNumber.ToString()][ACT + actNumber.ToString()][readIndex][TIME_VARIANCE].AsFloat;

				//a list of all the spawners where this wave might appear
				List<int> spawnLocs = new List<int>();

				for (int i = 0;
					 i < node[WORLDS][WORLD + worldNumber.ToString()][ACT + actNumber.ToString()][readIndex][WHICH_SPAWNERS].Count;
					 i++){
					spawnLocs.Add(node[WORLDS][WORLD + worldNumber.ToString()][ACT + actNumber.ToString()][readIndex][WHICH_SPAWNERS][i].AsInt);
				}
				
				PrepareFutureWaves(
					enemyName,
					waves,
					numPerWave,
					spawnLocs,
					timeBetweenWaves,
					timeVariance);

				nextReadTime += (waves * timeBetweenWaves) + (timeVariance * waves);
				
				break;
		}

		readIndex = GetNewReadIndex(node);

		//Debug.Log("readIndex == " + readIndex);

		return readIndex;
	}

	//determine what the readIndex should be, based on how far through the Act the game is
	private int GetNewReadIndex(JSONNode node){
		//are is there still more to do in this act?
		if (readIndex < node[WORLDS][WORLD + worldNumber.ToString()][ACT + actNumber.ToString()].Count - 1){
			readIndex++;
			return readIndex;
		} else { //if you get here, there's no more to do in this Act; go to the next
			actNumber++;
		}

		//is there an act with this actNumber? If so, restart the readIndex
		if (node[WORLDS][WORLD + worldNumber.ToString()][ACT + actNumber.ToString()] != null){
			return 0;
		} else { //if not, this world is over
			worldOver = true;
			Debug.Log("world over");
			return 0;
		}
	}

	//set up the system to make the next wave(s) of enemies
	private void PrepareFutureWaves(string enemyName, int numWaves, int numPerWave, List<int> spawnLocs, float timeBetweenWaves, float timeVariance){
		Debug.Log("PrepareFutureWaves() called; enemyName == " + enemyName);
		spawnTimer = 0.0f; //reset the clock that determines when enemies should spawn
		nextSpawnTimes.Clear(); //reset the list of when enemies will spawn
		spawnIndex = 0; //reset the index variable used to go through the list of spawn times

		//add times when enemies will spawn to the list
		//this will always add 0.0, so the first wave will spawn right away
		//if there are additional waves, this will add times when they should appear
		for (int i = 0; i < numWaves; i++){
			nextSpawnTimes.Add(i * timeBetweenWaves * (timeVariance * OneOrNegativeOne()));
		}

		//SpawnAWave() needs this information to make enemies
		enemyToMake = enemyName;
		enemiesPerWave = numPerWave;
		possibleSpawnersThisWave = spawnLocs;
		Debug.Log("nextSpawnTimes.Count == " + nextSpawnTimes.Count);
		Debug.Log("nextSpawnTimes[0] == " + nextSpawnTimes[0]);
	}


	/// <summary>
	/// Creates a wave of enemies.
	/// 
	/// First, this function shuffles the list of spawner numbers entered for this wave. Then it creates enemies at the locations of, 
	/// the spawners with those numbers, starting with the first number in the shuffled list. Finally, it updates spawnIndex to control
	/// whether spawning continues.
	/// 
	/// To create enemies in random positions, make numPerWave in the JSON file smaller than the number of spawners in whichSpawners.
	/// NEVER have numPerWave > the numer of spawners in whichSpawners--that will cause problems, and might crash the game!
	/// </summary>
	/// <returns>spawnIndex's new value.</returns>
	private int SpawnAWave(){
		Debug.Log("SpawnAWave() called; enemyToMake == " + enemyToMake);
		possibleSpawnersThisWave = ShuffleList(possibleSpawnersThisWave);

		for (int i = 0; i < enemiesPerWave; i++){
			GameObject obj = ObjectPooling.ObjectPool.GetObj(enemyToMake);

			if (i < possibleSpawnersThisWave.Count){ //backstop check to make sure the condition in the comments above is met
				obj.transform.position = FindSpawnerLoc(possibleSpawnersThisWave[i]);
			}
		}

		spawnIndex++;

		return spawnIndex;
	}


	/// <summary>
	/// Gives the location of a spawner.
	/// </summary>
	/// <returns>The spawner's location.</returns>
	/// <param name="targetSpawner">The number of the spawner whose location you want.</param>
	private Vector3 FindSpawnerLoc(int targetSpawner){
		for (int i = 0; i < spawners.Count; i++){
			if (spawners[i].name == SPAWNER_OBJ + targetSpawner.ToString()){
				return spawners[i].position;
			}
		}

		return new Vector3(0.0f, 0.0f, 0.0f); //debug value; this should never happen. If it does, the function couldn't find the intended spawner
	}


	//utility function to give 1 or -1
	//use this to randomize whether a number is positive or negative
	private int OneOrNegativeOne(){
		return Random.Range(0, 2) * 2 - 1;
	}


	//utility function to shuffle a list's entries
	//uses the Fisher-Yates algorithm
	private List<int> ShuffleList(List<int> list){
		for (int i = 0; i < list.Count; i++) {
			int temp = list[i];
			int randomIndex = Random.Range(i, list.Count);
			list[i] = list[randomIndex];
			list[randomIndex] = temp;
		}

		return list;
	}
}