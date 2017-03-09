/*
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
	public string filename = "Levels.json";

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

	//these control whether the game has started
	//the game starts when the ball is first passed; InputManager sets this to true as part of being thrown
	private bool gameHasStarted = false;
	public bool GameHasStarted{
		get { return gameHasStarted; }
		set { gameHasStarted = value; }
	}
	public bool RestartingGame { get; set; }


	//bosses set this in order to stop new enemies from spawning
	public bool Hold { get; set; }


	//these are used to restart the game
	private const string ENEMY_ORGANIZER = "Enemies";
	private Transform enemies;
	private const string ENEMY_TAG = "Enemy";
	private PlayerEnemyInteraction p1EnemyScript;
	private const string PLAYER_1 = "Player 1";
	private PlayerEnemyInteraction p2EnemyScript;
	private const string PLAYER_2 = "Player 2";
	private BallBehavior ballScript;
	private const string BALL = "Ball";
	private bool checkpointReached = false; //did the players hit a checkpoint on their last run?
	private int checkpointWorldNum = 0;
	private int checkpointActNum = 0;
	private float checkpointNextReadTime = 0.0f;
	private int checkpointReadIndex = 0;
	public float restartGracePeriod = 1.0f; //how long players have after a restart before enemies appear
	private Transform particles;
	private const string PARTICLE_ORGANIZER = "Particles";
	private const string PARTICLE_TAG = "Particle";


	private void Start(){
		spawners = FindSpawners();
		ObjectPooling.ObjectPool.GameOver = false; //start the game
		enemies = GameObject.Find(ENEMY_ORGANIZER).transform;
		p1EnemyScript = GameObject.Find(PLAYER_1).GetComponent<PlayerEnemyInteraction>();
		p2EnemyScript = GameObject.Find(PLAYER_2).GetComponent<PlayerEnemyInteraction>();
		ballScript = GameObject.Find(BALL).GetComponent<BallBehavior>();
		particles = GameObject.Find(PARTICLE_ORGANIZER).transform;
		RestartingGame = false;
		Hold = false;
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
		if (GameHasStarted && !Hold){
			if (!worldOver){
				timer += Time.deltaTime;

				if (timer >= nextReadTime){
					readIndex = ReadInItem();
				}
			}

			if (spawnIndex < nextSpawnTimes.Count){
				spawnTimer += Time.deltaTime;

				if (spawnTimer >= nextSpawnTimes[spawnIndex]){
					spawnIndex = SpawnAWave();
				}
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
	 * 
	 * HOW TO READ THE JSON FILE
	 * 
	 * Here's an example of how the JSON file might look:
	 *
	 * 
	 * ------
	 * "world1":{
			"act1":[
				{"action":"spawn", "whichEnemy":"HomingEnemy", "waves":1, "numPerWave":3, "whichSpawners":[1, 2, 3], "timeBetweenWaves":1, "timeVariance":0},
				{"action":"wait", "time":5},
			],
			"act2":[
				{"action":"spawn", "whichEnemy":"PlayerHuntEnemy", "waves":1, "numPerWave":1, "whichSpawners":[1, 2, 3], "timeBetweenWaves":1, "timeVariance":0}
				{"action":"wait", "time":5},
			],
			"act3":[
				{"action":"spawn", "whichEnemy":"PlayerHuntEnemy", "waves":1, "numPerWave":1, "whichSpawners":[1, 2, 3], "timeBetweenWaves":1, "timeVariance":0}
			]},
		"world2":{
				"act1":[
					{"action":"spawn", "whichEnemy":"HomingEnemy", "waves":1, "numPerWave":3, "whichSpawners":[1, 2, 3], "timeBetweenWaves":1, "timeVariance":0},
					{"action":"wait", "time":5},
				],
				"act2":[
					{"action":"spawn", "whichEnemy":"PlayerHuntEnemy", "waves":1, "numPerWave":1, "whichSpawners":[1, 2, 3], "timeBetweenWaves":1, "timeVariance":0}
				]}
		}
		------

		The JSON file is divided into worlds, acts, and waves.

		Each world can have as many acts as you like. Add new acts by copying the formatting of existing ones.
		Note that all acts but the very last one in the very last world need to have a comma after the ] or } that closes them.

		Within an act, each line denotes something this script should do.

		Every line starts with an action. The two possible actions are spawn and wait.

		Wait tells the script not to read the next line for the amount of time in the "time" entry. Time is read in as a float,
		so you can enter decimal values.

		Spawn creates something. That can include anything that can appear at a spawner: enemies, terrain features, etc.
		The remaining entries determine what is spawned, where, and when.

		whichEnemy: the exact name of the prefab to the spawned. Again, this says "enemy" because that's the normal use, but it could be other things.
		waves: how many waves of that prefab should be spawned. The example always has 1 wave per line, but it can be more. Must be an int.
		numPerWave: how many of the prefab to spawn in each wave. Must be an int.
		whichSpawners: the spawners where the spawned prefabs will appear. Must be ints.
		timeBetweenWaves: the number of seconds between spawning waves. Can be a float.
		timeVariance: this will be randomly added or subtracted to the time between each wave. Can be a float.

		-- Spawning in random locations --

		Provide more spawners in whichSpawners than the number in numPerWave. This script will randomly choose from the listed spawners 
		for each new wave.
		
		NEVER have numPerWave greater than the number of spawners in whichSpawners! At best the enemy will spawn at (0, 0, 0); at worst
		the game will crash.

		-- How timeVariance works, in detail --
		The number in timeVariance is either added to or subtracted from timeBetweenWaves (50/50 chance of each) to get the actual time
		between each wave.

		The entire timeVariance is added or subtracted each time, not a number between -timeVariance and timeVariance. Otherwise
		it's easy to get near-zero values that eliminate timeVariance's effect.

		-- Total amount of time for a spawn --
		In order to ensure that the entire line is completed before the next line is read, the total amount of time for one spawn line is
		(waves * timeBetweenWaves) + (timeVariance * waves).

		This means that to interleave different types of spawns, it's necessary to have separate lines for each, with 1 wave and
		short (or even zero) timeBetweenWaves and a timeVariance of zero.
	 */
	public int ReadInItem(){
		JSONNode node = JSON.Parse(FileIOUtil.ReadStringFromFile(Application.dataPath + LEVEL_PATH + filename));
//		Debug.Log("ReadInItem() called; action == " + node[WORLDS][WORLD + worldNumber.ToString()][ACT + actNumber.ToString()][readIndex][ACTION]);

		//Debug.Log("current action:" + node[WORLDS][WORLD + worldNumber.ToString()][ACT + actNumber.ToString()][readIndex][ACTION]);

		//decide what to do based on the current action
		switch (node[WORLD + worldNumber.ToString()][ACT + actNumber.ToString()][readIndex][ACTION]) {

			//if the action is "wait," read the next line after the indicated number of seconds.
			case WAIT:
				nextReadTime += node[WORLD + worldNumber.ToString()][ACT + actNumber.ToString()][readIndex][TIME].AsFloat;
				break;

			//if the action is "spawn", make enemies
			case SPAWN:
				//get the values needed to make enemies
				
				//which enemy prefab to spawn
				string enemyName = node[WORLD + worldNumber.ToString()][ACT + actNumber.ToString()][readIndex][WHICH_ENEMY];
				//Debug.Log("enemyName == " + enemyName);

				//how many waves
				int waves = node[WORLD + worldNumber.ToString()][ACT + actNumber.ToString()][readIndex][WAVES].AsInt;

				//how many enemies in each wave
				int numPerWave = node[WORLD + worldNumber.ToString()][ACT + actNumber.ToString()][readIndex][NUM_PER_WAVE].AsInt;

				//time between waves
				float timeBetweenWaves = 
					node[WORLD + worldNumber.ToString()][ACT + actNumber.ToString()][readIndex][TIME_BETWEEN_WAVES].AsFloat;

				//variance in the time between waves
				float timeVariance = node[WORLD + worldNumber.ToString()][ACT + actNumber.ToString()][readIndex][TIME_VARIANCE].AsFloat;

				//a list of all the spawners where this wave might appear
				List<int> spawnLocs = new List<int>();

				for (int i = 0;
					 i < node[WORLD + worldNumber.ToString()][ACT + actNumber.ToString()][readIndex][WHICH_SPAWNERS].Count;
					 i++){
					spawnLocs.Add(node[WORLD + worldNumber.ToString()][ACT + actNumber.ToString()][readIndex][WHICH_SPAWNERS][i].AsInt);
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
		//is there still more to do in this act?
		if (readIndex < node[WORLD + worldNumber.ToString()][ACT + actNumber.ToString()].Count - 1){
			readIndex++;
			return readIndex;
		} else { //if you get here, there's no more to do in this Act; go to the next
			actNumber++;
		}

		//is there an act with this actNumber? If so, restart the readIndex
		if (node[WORLD + worldNumber.ToString()][ACT + actNumber.ToString()] != null){
			return 0;
		} else { //if not, this world is over
			worldOver = true;
			return 0;
		}
	}

	//set up the system to make the next wave(s) of enemies
	private void PrepareFutureWaves(string enemyName, int numWaves, int numPerWave, List<int> spawnLocs, float timeBetweenWaves, float timeVariance){
//		Debug.Log("PrepareFutureWaves() called; enemyName == " + enemyName);
		spawnTimer = 0.0f; //reset the clock that determines when enemies should spawn
		nextSpawnTimes.Clear(); //reset the list of when enemies will spawn
		spawnIndex = 0; //reset the index variable used to go through the list of spawn times

		//add times when enemies will spawn to the list
		//this will always add 0.0, so the first wave will spawn right away
		//if there are additional waves, this will add times when they should appear
		for (int i = 0; i < numWaves; i++){
			nextSpawnTimes.Add(i * timeBetweenWaves + (timeVariance * OneOrNegativeOne()));
		}
			
		//SpawnAWave() needs this information to make enemies
		enemyToMake = enemyName;
		enemiesPerWave = numPerWave;
		possibleSpawnersThisWave = spawnLocs;
//		Debug.Log("nextSpawnTimes.Count == " + nextSpawnTimes.Count);
//		Debug.Log("nextSpawnTimes[0] == " + nextSpawnTimes[0]);
	}


	/// <summary>
	/// Creates a wave of enemies.
	/// 
	/// First, this function shuffles the list of spawner numbers entered for this wave. Then it creates enemies at the locations of 
	/// the spawners with those numbers, starting with the first number in the shuffled list. Finally, it updates spawnIndex to control
	/// whether spawning continues.
	/// 
	/// To create enemies in random positions, make numPerWave in the JSON file smaller than the number of spawners in whichSpawners.
	/// NEVER have numPerWave > the numer of spawners in whichSpawners--that will cause problems, and might crash the game!
	/// </summary>
	/// <returns>spawnIndex's new value.</returns>
	private int SpawnAWave(){
		possibleSpawnersThisWave = ShuffleList(possibleSpawnersThisWave);

		for (int i = 0; i < enemiesPerWave; i++){
			if (i < possibleSpawnersThisWave.Count){ //backstop check to make sure the condition in the comments above is met
				GameObject obj = ObjectPooling.ObjectPool.GetObj(enemyToMake);
				obj.transform.position = FindSpawnerLoc(possibleSpawnersThisWave[i]);
				obj.GetComponent<ObjectPooling.Poolable>().Reset();
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


	public void SetCheckpoint(int worldNum, int actNum, float nextRead, int index){
		checkpointReached = true;
		checkpointWorldNum = worldNum;
		checkpointActNum = actNum;
		checkpointNextReadTime = nextRead;
		checkpointReadIndex = index;
	}

	public void StopGame(){
		if (GameHasStarted){
			GameHasStarted = false;
			RestartingGame = true;

			foreach (Transform enemy in enemies){
				if (enemy.tag == ENEMY_TAG && enemy.gameObject.activeInHierarchy == true){
					enemy.GetComponent<EnemyBase>().GetDestroyed();
				}
			}
		}
	}


	public void RestartGame(){
		if (!GameHasStarted){
			p1EnemyScript.ResetPlayer();
			p2EnemyScript.ResetPlayer();
			ballScript.ResetBall();

			foreach (Transform particle in particles){
				if (particle.tag == PARTICLE_TAG){
					ObjectPooling.ObjectPool.AddObj(particle.gameObject);
				}
			}

			if (checkpointReached){
				timer = checkpointNextReadTime - restartGracePeriod;
				worldNumber = checkpointWorldNum;
				actNumber = checkpointActNum;
				nextReadTime = checkpointNextReadTime;
				readIndex = checkpointReadIndex;
			} else {
				timer = 0.0f;
				worldNumber = 1;
				actNumber = 1;
				nextReadTime = 0.0f;
				readIndex = 0;
			}
				
			spawnIndex = 0;
			spawnTimer = 0.0f;
			nextSpawnTimes.Clear();

			GameHasStarted = true;
			RestartingGame = false;
			worldOver = false;
			ObjectPooling.ObjectPool.GameOver = false;
		}
	}


	//These next functions are for checkpoints. They use them to get the data they'll feed back in if they're collected successfully
	public int GetWorldNum(){
		return worldNumber;
	}

	public int GetActNum(){
		return actNumber;
	}

	public float GetNextReadTime(){
		return nextReadTime;
	}

	public int GetReadIndex(){
		return readIndex;
	}


	public void StartGame(GameObject source){
		if (!RestartingGame && !GameHasStarted){
			GameHasStarted = true;
		}
	}
}
