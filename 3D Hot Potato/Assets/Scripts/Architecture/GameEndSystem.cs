using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEndSystem : MonoBehaviour {


	//how long the game will go without an input before resetting
	public float noInputResetTime = 40.0f;


	//----------Internal variables----------


	//timer for tracking whether there's been an input
	private float noInputTimer = 0.0f;


	//the score system
	private ScoreManager scoreManager;


	//scenes to load
	private const string TITLE_SCENE = "TitleScene3";
	private const string HIGH_SCORE_SCENE = "High score scene";


	//variables for the death-tracking system
	private int deathsThisAct = 0;
	private int promptThreshold = 0;
	private List<int> deathsUntilNextPrompt = new List<int>() { 7, 6, 7, 5 };
	private Queue<int> currentPromptDeaths = new Queue<int>();
	private PauseMenuBehavior pauseMenuScript;
	private const string PAUSE_MENU_OBJ = "Controller map";


	//initialize variables
	private void Start(){
		scoreManager = GetComponent<ScoreManager>();
		ResetDeathTracking();
		pauseMenuScript = GameObject.Find(PAUSE_MENU_OBJ).GetComponent<PauseMenuBehavior>();

	}



	private void Update(){
		noInputTimer += Time.deltaTime;

		if (noInputTimer >= noInputResetTime){
			ObjectPooling.ObjectPool.ClearPools();
			SceneManager.LoadScene(TITLE_SCENE);
		}
	}


	public void ResetInputTimer(){
		noInputTimer = 0.0f;
	}


	public void VoluntaryStop(){
		ScoreRepository.Score = (int)scoreManager.Score;
		ObjectPooling.ObjectPool.ClearPools();
		SceneManager.LoadScene(HIGH_SCORE_SCENE);
	}


	public void ResetDeathTracking(){
		//Debug.Log("ResetDeathTracking() called");
		deathsThisAct = 0;

		currentPromptDeaths.Clear();

		foreach (int number in deathsUntilNextPrompt){
			currentPromptDeaths.Enqueue(number);
		}
	
		promptThreshold = SetPromptThreshold();
		//Debug.Log("promptThreshold is now == " + promptThreshold);
	}


	private void IncrementDeathTracking(){
		//Debug.Log("IncrementDeathTracking() called");
		deathsThisAct = 0;
		promptThreshold = SetPromptThreshold();
		//Debug.Log("promptThreshold is now == " + promptThreshold);
	}


	public void CheckDeathTracking(){
		//Debug.Log("CheckDeathTracking() called; promptThreshold == " + promptThreshold);
		deathsThisAct++;
		//Debug.Log("deathsThisAct == " + deathsThisAct);

		if (deathsThisAct >= promptThreshold){
			//Debug.Log("Reached prompt threshold");

			IncrementDeathTracking();
			pauseMenuScript.ChangePauseMenuState();
			StartCoroutine(pauseMenuScript.RequireOptInContinue());
		}
	}


	public int SetPromptThreshold(){
		if (currentPromptDeaths.Count > 1){
			return currentPromptDeaths.Dequeue();
		} else {
			return currentPromptDeaths.Peek();
		}
	}
}
