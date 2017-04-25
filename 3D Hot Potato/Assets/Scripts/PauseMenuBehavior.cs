using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PauseMenuBehavior : MonoBehaviour {


	//----------Tunable variables----------
	[SerializeField] private float optInPeriod = 20.0f; //how long players have to restart the game after many deaths


	//----------Internal variables----------

	//other scripts access this to determine whether the game is currently paused
	public bool Paused { get; private set; }


	//the pause menu
	private GameObject controllerMap;
	private const string CONTROLLER_MAP_OBJ = "Controller map";


	//special text used when the players have to opt-in to continue playing
	private GameObject optInDisplay;
	private const string OPT_IN_DISPLAY_OBJ = "Opt-in display";
	private TextMeshProUGUI optInSeconds;
	private const string OPT_IN_SECONDS_OBJ = "Opt-in timer";
	private float optInTimer = 0.0f;


	//used to end the game if the players don't opt into continuing
	private GameEndSystem gameEndSystem;
	private const string MANAGER_OBJ = "Managers";


	private void Start(){
		controllerMap = GameObject.Find(CONTROLLER_MAP_OBJ);
		ShowControllerMap();
		Paused = true;
		optInDisplay = GameObject.Find(OPT_IN_DISPLAY_OBJ);
		optInSeconds = GameObject.Find(OPT_IN_SECONDS_OBJ).GetComponent<TextMeshProUGUI>();
		optInDisplay.SetActive(false);
		optInTimer = optInPeriod;
		gameEndSystem = GameObject.Find(MANAGER_OBJ).GetComponent<GameEndSystem>();
	}


	public void ChangePauseMenuState(){
		Paused = !Paused;

		if (Paused){
			ShowControllerMap();
		} else {
			HideControllerMap();
		}
	}


	private void ShowControllerMap(){
		Time.timeScale = 0.0f;
		controllerMap.SetActive(true);
	}


	private void HideControllerMap(){
		Time.timeScale = 1.0f;
		controllerMap.SetActive(false);
	}


	public IEnumerator RequireOptInContinue(){
		optInDisplay.SetActive(true);

		optInTimer = optInPeriod;

		while (optInTimer >= 0.0f){
			optInTimer -= Time.unscaledDeltaTime;

			optInSeconds.text = ((int)optInTimer).ToString();

			//if the players unpause the game, they've opted into continuing; stop this coroutine
			if (!Paused){
				optInDisplay.SetActive(false);
				yield break;
			}

			yield return null;
		}

		//if the timer runs out, it's game over
		gameEndSystem.VoluntaryStop();

		yield break;
	}
}
