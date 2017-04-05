/*
 * 
 * HOW TO CREATE A NEW TUTORIAL:
 * 
 * 1. Create a new instruction object in Start(). Copy the syntax of the existing ones:
 * - the first string is text to be displayed in the top-left of the screen
 * - the second string is text in the top-right
 * - the third string is text in the lower right.
 * - the Vector3 is where the "Cameras" object (the parent object of the main camera and the UI camera) will zoom in to
 * - the final entry is a function name; that function will end the tutorial.
 * 
 * 
 * The function that ends the tutorial needs to listen for an event, so that it knows when the tutorial is finished.
 * When it receives the event, it must set tutorialFinished = true. The function must be of return type void,
 * and take one parameter of type Event.
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour {


	//----------Tunable variables----------

	public float zoomDuration = 0.5f; //how long the camera spends zooming in and out
	public float readDelay = 2.0f; //how long the camera stays zoomed in
	public float enemyEntryTime = 0.5f; //how long it takes enemies to enter the playing field






	//----------Internal variables----------


	//the tutorials, plus a variable to store the current one
	private Instruction blockInstruction; //basic blocking
	private Instruction passInstruction; //basic passing
	private Instruction blightrunnerInstruction; //how to deal with the hunt enemies
	private Instruction powerInstruction; //how to activate a power
	private Instruction currentInstruction;


	//put these into the JSON file to choose a tutorial
	private const string BLOCK_TUTORIAL = "Block"; //goes with BlockingTutorialEnemy
	private const string PASS_TUTORIAL = "Pass";
	private const string BLIGHT_TUTORIAL = "Blight"; //goes with BlightrunnerTutorialHuntEnemy
	private const string POWER_TUTORIAL = "Power";


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


	//an empty string to clear instructions on the screen
	private const string BLANK = "";


	//delegate for handling events, and associated variables
	private Event.Handler eventHandler;
	private const string TUTORIAL = "Tutorial";


	//the level manager, so it can be shut off while waiting for tutorials to complete
	private LevelManager levelManager;


	//use this bool to hold while waiting for the player to complete a tutorial
	private bool tutorialFinished = false;



	private void Start(){
		instructionsTopLeft = GameObject.Find(INSTRUCTIONS_1_OBJ).GetComponent<Text>();
		instructionsTopRight = GameObject.Find(INSTRUCTIONS_2_OBJ).GetComponent<Text>();
		instructionsBottomRight = GameObject.Find(INSTRUCTIONS_3_OBJ).GetComponent<Text>();
		cameras = GameObject.Find(CAMERA_ORGANIZER).transform;
		camerasStartPos = cameras.position;
		levelManager = GetComponent<LevelManager>();

		blockInstruction = new Instruction("",
										   "The player without the Neon Star can block",
										   "Run into this enemy to proceed",
										   new Vector3 (0.0f, -13.8f, 31.5f),
										   EnemyDestroyedFunc);
		blightrunnerInstruction = new Instruction("Gold enemies change the rules",
												  "Hit them with the Neon Star",
												  "If you don't have it, stay safe!",
												  new Vector3(0.0f, -13.8f, 31.5f),
												  EnemyDestroyedFunc);
	}


	/// <summary>
	/// LevelManager calls this to start a tutorial.
	/// </summary>
	/// <param name="tutorial">Which tutorial to begin.</param>
	public void StartTutorial(string tutorial){
		switch (tutorial){
			case BLOCK_TUTORIAL:
				currentInstruction = blockInstruction;
				break;
			case BLIGHT_TUTORIAL:
				currentInstruction = blightrunnerInstruction;
				break;
		}

		StartCoroutine(DisplayTutorial(currentInstruction));
	}


	/// <summary>
	/// This coroutine organizes the overall tutorial process:
	/// 
	/// 1. Stop LevelManager from creating new enemies.
	/// 2. Zoom in on some key part of the screen.
	/// 3. Display explanatory text, if any.
	/// 4. Clear the text.
	/// 5. Zoom back out.
	/// 6. Wait until something happens.
	/// 7. Restart the LevelManager.
	/// </summary>
	/// <param name="instruction">The current instruction object.</param>
	private IEnumerator DisplayTutorial(Instruction instruction){
		levelManager.Hold = true;
		tutorialFinished = false;
		yield return new WaitForSeconds(enemyEntryTime);
		Time.timeScale = 0.0f;
		yield return StartCoroutine(ZoomCameraIn(instruction.CameraZoomPos));
		DisplayTutorialText(instruction);
		yield return new WaitForSecondsRealtime(readDelay);
		ClearTutorialText();
		yield return StartCoroutine(ZoomCameraOut(instruction.CameraZoomPos));
		Time.timeScale = 1.0f;
		eventHandler = new Event.Handler(instruction.eventHandlerFunc);
		Services.EventManager.Register<EnemyDestroyedEvent>(eventHandler);

		//wait until the event handler sets tutorialFinished = true
		while (!tutorialFinished){
			yield return null;
		}

		levelManager.Hold = false;
		yield break;
	}


	//////////////////////////////////////////////////////
	/// FUNCTIONS USED BY ALL TUTORIALS
	//////////////////////////////////////////////////////

	private void DisplayTutorialText(Instruction instruction){
		instructionsTopLeft.text = instruction.TopLeftText;
		instructionsTopRight.text = instruction.TopRightText;
		instructionsBottomRight.text = instruction.BottomRightText;
	}

	private void ClearTutorialText(){
		instructionsTopLeft.text = BLANK;
		instructionsTopRight.text = BLANK;
		instructionsBottomRight.text = BLANK;
	}


	private IEnumerator ZoomCameraIn(Vector3 destination){

		float startTime = Time.realtimeSinceStartup;
		float timer = 0.0f;
		while (timer <= zoomDuration){
			timer = Time.realtimeSinceStartup - startTime;

			cameras.position = Vector3.Lerp(camerasStartPos, destination, timer/zoomDuration);

			yield return null;
		}

		yield break;
	}


	private IEnumerator ZoomCameraOut(Vector3 zoomedPos){
		float startTime = Time.realtimeSinceStartup;
		float timer = 0.0f;
		while (timer <= zoomDuration){
			timer = Time.realtimeSinceStartup - startTime;

			cameras.position = Vector3.Lerp(zoomedPos, camerasStartPos, timer/zoomDuration);

			yield return null;
		}

		yield break;
	}


	//////////////////////////////////////////////////////
	/// FUNCTIONS USED TO STOP INDIVIDUAL TUTORIALS
	//////////////////////////////////////////////////////


	//Used to stop the tutorials about blocking and the player hunt enemy
	//Listens for when an enemy is destroyed; assuming it's a tutorial enemy, signals that the tutorial is finished
	public void EnemyDestroyedFunc(Event e){
		//Debug.Assert(e.GetType() == EnemyDestroyedEvent);

		EnemyDestroyedEvent enemyDestroyedEvent = e as EnemyDestroyedEvent;

		if (enemyDestroyedEvent.enemy.name.Contains(TUTORIAL)){
			Services.EventManager.Unregister<EnemyDestroyedEvent>(eventHandler);
			tutorialFinished = true;
		}
	}



	//////////////////////////////////////////////////////
	/// Private class used to create tutorials
	//////////////////////////////////////////////////////

	private class Instruction{
		public string TopLeftText { get; private set; } //the text field at the top-left of the screen
		public string TopRightText { get; private set; } //text field at the top-right
		public string BottomRightText { get; private set; } //text field at the bottom-right
		public Vector3 CameraZoomPos { get; private set; } //where the "Cameras" object should lerp to
		public delegate void EventHandlerFunc(Event e);
		public EventHandlerFunc eventHandlerFunc { get; private set; } //the function that will end the tutorial


		public Instruction(string topLeft,
						   string topRight,
						   string bottomRight,
						   Vector3 camPos,
						   EventHandlerFunc eventHandlerFunc){
			TopLeftText = topLeft;
			TopRightText = topRight;
			BottomRightText = bottomRight;
			CameraZoomPos = camPos;
			this.eventHandlerFunc = eventHandlerFunc;
		}
	}
}
