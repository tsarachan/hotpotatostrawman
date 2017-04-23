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


	//initialize variables
	private void Start(){
		scoreManager = GetComponent<ScoreManager>();
	}



	private void Update(){
		noInputTimer += Time.deltaTime;

		if (noInputTimer >= noInputResetTime){
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
}
