using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingMoveTitle : MonoBehaviour {

	// Use this for initialization
	void Start () {

		GameObject managers = GameObject.Find("Managers");
		InputManager input = (InputManager) managers.GetComponent(typeof(InputManager));
		input.StartGame();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
