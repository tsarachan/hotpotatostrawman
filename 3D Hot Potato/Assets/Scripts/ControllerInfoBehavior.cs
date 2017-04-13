using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerInfoBehavior : MonoBehaviour {

	private bool showingControllerInfo = true;


	private GameObject controllerMap;
	private const string CONTROLLER_MAP_OBJ = "Controller map";


	private void Start(){
		controllerMap = GameObject.Find(CONTROLLER_MAP_OBJ);
		ShowControllerMap();
	}


	public void ReceivePauseInput(){
		showingControllerInfo = !showingControllerInfo;

		if (showingControllerInfo){
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
}
