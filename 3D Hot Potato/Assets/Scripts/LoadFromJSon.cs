/*
 * 
 * This script loads enemies and level elements from a JSon file.
 * 
 */

using SimpleJSON;
using UnityEngine;
using System.Collections;

public class LoadFromJSon : MonoBehaviour {

	public class JSonEntry {

	}

	//names of JSon objects--these are used to read lines from the JSON file
	private const string LEVELS = "levels";
	private const string LEVEL = "level";
	private const string ENEMIES_TO_MAKE = "enemiesToMake";
	private int levelNumber = 1; //which level are we on? Starts at level 1.
	private const string SPAWN = "spawn";
	private const string WHICH_ENEMY = "whichEnemy";
	private const string WAVES = "waves";
	private const string WHICH_SPAWNERS = "whichSpawners";
	private const string TIME_BETWEEN_WAVES = "timeBetweenWaves";
	private const string TIME_VARIANCE = "timeVariance";

	private float timer = 0.0f; //the time the players have spent in the level so far
	private float nextReadTime = 0.0f; //when should the system read the next item from the JSon file?
	private int readIndex = 0; //the next thing to read in the "enemiesToMake" JSon object for the current level

	//these are used to find the JSon file in the file directory
	private const string LEVEL_PATH = "/Levels/";
	private const string FILE_NAME = "Levels.json";

	private void Start(){
		//begin a level by reading the first JSon object
		readIndex = ReadInItem();
	}

	private void Update(){
		timer += Time.deltaTime;
	}

	public int ReadInItem(){
		JSONNode node = JSON.Parse(FileIOUtil.ReadStringFromFile(Application.dataPath + LEVEL_PATH + FILE_NAME));
//		Debug.Log(node);
//		Debug.Log(node[LEVELS][LEVEL + "1"][ENEMIES_TO_MAKE]);
//		Debug.Log(node[LEVELS][LEVEL + "1"][ENEMIES_TO_MAKE][0]);
//		Debug.Log(node[LEVELS][LEVEL + "1"][ENEMIES_TO_MAKE][0]["spawn"]);
//		Debug.Log(node[LEVELS][LEVEL + levelNumber.ToString()][ENEMIES_TO_MAKE][0]["spawn"].AsInt);

		int numWaves = node[LEVELS][LEVEL + levelNumber.ToString()][ENEMIES_TO_MAKE][readIndex][WAVES].AsInt;

		Debug.Log(numWaves);

		readIndex++;

		return readIndex;
	}
}
