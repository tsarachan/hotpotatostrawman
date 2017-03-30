using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour {


	//----------Tunable variables----------

	public float zoomDuration = 0.5f; //how long the camera spends zooming in and out
	public float readDelay = 2.0f; //how long the camera stays zoomed in


	private Instruction blockInstruction
		= new Instruction("",
						  "The player without the Battery Star can block",
						  "Run into this enemy to proceed",
						  new Vector3 (0.0f, -14.4f, 57.5f),
						  "");



	//----------Internal variables----------


	//this stores a value that other scripts will send to choose a tutorial
	private const string BLOCK_TUTORIAL = "Block";
	private const string PASS_TUTORIAL = "Pass";
	private const string POWER_TUTORIAL = "Power";
	private const string BLIGHT_TUTORIAL = "Blight";


	//the text blocks where instructions can be displayed
	private Text instructionsTopLeft;
	private Text instructionsTopRight;
	private Text instructionsBottomRight;
	private const string INSTRUCTIONS_1_OBJ = "Tutorial text 1";
	private const string INSTRUCTIONS_2_OBJ = "Tutorial text 2";
	private const string INSTRUCTIONS_3_OBJ = "Tutorial text 2";


	//the camera parent object--move this so that both the normal and the UI camera move
	private Transform cameras;
	private Vector3 camerasStartPos;
	private const string CAMERA_ORGANIZER = "Cameras";



	private void Start(){
		instructionsTopLeft = GameObject.Find(INSTRUCTIONS_1_OBJ).GetComponent<Text>();
		instructionsTopRight = GameObject.Find(INSTRUCTIONS_2_OBJ).GetComponent<Text>();
		instructionsBottomRight = GameObject.Find(INSTRUCTIONS_3_OBJ).GetComponent<Text>();
		cameras = GameObject.Find(CAMERA_ORGANIZER).transform;
		camerasStartPos = cameras.position;
		StartCoroutine(ZoomCameraIn(new Vector3(0.0f, 0.0f, 0.0f)));
	}


	public void StartTutorial(string tutorialName){
		Time.timeScale = 0.0f;

		switch (tutorialName){
			case BLOCK_TUTORIAL:
				
				break;
		}
	}


	private void DisplayTutorialText(Instruction instruction){
		instructionsTopLeft.text = instruction.TopLeftText;
		instructionsTopRight.text = instruction.TopRightText;
		instructionsBottomRight.text = instruction.BottomRightText;
	}


	private IEnumerator ZoomCameraIn(Vector3 destination){
		Debug.Log("ZoomCameraIn started");

		float startTime = Time.realtimeSinceStartup;
		float timer = 0.0f;
		while (timer <= zoomDuration){
			Debug.Log(timer);
			timer += Time.realtimeSinceStartup - startTime;

			cameras.position = Vector3.Lerp(camerasStartPos, destination, timer/zoomDuration);

			yield return null;
		}

		yield return new WaitForSecondsRealtime(readDelay);

		yield break;
	}


	private class Instruction{
		public string TopLeftText { get; private set; }
		public string TopRightText { get; private set; }
		public string BottomRightText { get; private set; }
		public Vector3 CameraZoomPos { get; private set; }
		public string ProceedEvent { get; private set; }


		public Instruction(string topLeft,
						   string topRight,
						   string bottomRight,
						   Vector3 camPos,
						   string proceed){
			TopLeftText = topLeft;
			TopRightText = topRight;
			BottomRightText = bottomRight;
			CameraZoomPos = camPos;
			ProceedEvent = proceed;
		}
	}
}
