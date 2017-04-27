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
using TMPro;

public class TutorialManager : MonoBehaviour {


	//----------Tunable variables----------

	public float zoomDuration = 0.5f; //how long the camera spends zooming in and out
	public float readDelay = 2.0f; //how long the camera stays zoomed in
	public float powerDuration = 3.0f; //how long the powers tutorial should wait for the power demo to end

	//the camera's position relative to things it zooms in on
	public Vector3 cameraOffset = new Vector3(0.0f, 0.0f, 0.0f);






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
	private TextMeshProUGUI instructionsTopLeft;
	private TextMeshProUGUI instructionsTopRight;
	private TextMeshProUGUI instructionsBottomRight;
	private const string INSTRUCTIONS_1_OBJ = "Tutorial text 1";
	private const string INSTRUCTIONS_2_OBJ = "Tutorial text 2";
	private const string INSTRUCTIONS_3_OBJ = "Tutorial text 3";


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


	//used to find the ball for the passing tutorial
	private const string BALL_OBJ = "Ball";


	//used to find the players for the powers tutorial
	private const string PLAYER_ORGANIZER = "Players";


	private void Start(){
		instructionsTopLeft = GameObject.Find(INSTRUCTIONS_1_OBJ).GetComponent<TextMeshProUGUI>();
		instructionsTopRight = GameObject.Find(INSTRUCTIONS_2_OBJ).GetComponent<TextMeshProUGUI>();
		instructionsBottomRight = GameObject.Find(INSTRUCTIONS_3_OBJ).GetComponent<TextMeshProUGUI>();
		cameras = GameObject.Find(CAMERA_ORGANIZER).transform;
		camerasStartPos = cameras.position;
		levelManager = GetComponent<LevelManager>();

		//first line = top left, middle = top right, bottom = bottom right
		passInstruction = new Instruction("Cooperate with your partner to reach the city center.",
										  "Press the  <sprite=\"Pass_button\" index=0> button to pass the Neon Star",
										  "Try passing now",
										  new Vector3(0.0f, 0.0f, 0.0f), //the pass instruction will find the ball
										  PassRegistration,
										  PlayerPassFunc,
										  0.0f);
		blockInstruction = new Instruction("The Neon Star must be protected from enemies.",
										   "The player without the Neon Star is invincible.",
										   "Have them run into this enemy to continue",
										   new Vector3 (0.0f, -13.8f, 31.5f),
										   EnemyDestroyedRegistration,
										   EnemyDestroyedFunc,
										   0.5f);
		blightrunnerInstruction = new Instruction("Gold enemies change the rules",
												  "Gold enemies are hurt by the Neon Star",
												  "If you don't have the Neon Star, they will take you down!",
												  new Vector3(0.0f, -13.8f, 31.5f),
												  EnemyDestroyedRegistration,
												  EnemyDestroyedFunc,
												  0.5f);
		powerInstruction = new Instruction("Each player has a unique power",
											"Trigger your power just before catching the Neon Star",
											"Use your power by pressing the  <sprite=\"Pass_button\" index=0> button while the Neon Star is sparking  <sprite=\"Spark_ex\" index=0>",
										   new Vector3(0.0f, 0.0f, 0.0f), //this instruction will find the ball
										   PowerTriggeredRegistration,
										   PowerTriggeredFunc,
										   0.0f);
	}


	/// <summary>
	/// LevelManager calls this to start a tutorial.
	/// </summary>
	/// <param name="tutorial">Which tutorial to begin.</param>
	public IEnumerator StartTutorial(string tutorial){
		switch (tutorial){
			case PASS_TUTORIAL:
				passInstruction.CameraZoomPos = GetBallPos() + cameraOffset;
				currentInstruction = passInstruction;
				break;
			case BLOCK_TUTORIAL:
				currentInstruction = blockInstruction;
				break;
			case BLIGHT_TUTORIAL:
				currentInstruction = blightrunnerInstruction;
				break;
			case POWER_TUTORIAL:
				currentInstruction = powerInstruction;
				yield return StartCoroutine(StartPowersTutorial());
				currentInstruction.CameraZoomPos = GetBallPos() + cameraOffset;
				break;
		}

		yield return StartCoroutine(DisplayTutorial(currentInstruction));

		yield break;
	}


	/// <summary>
	/// Find the ball so that the passing tutorial can zoom in on it
	/// </summary>
	/// <returns>The ball carrier position.</returns>
	private Vector3 GetBallPos(){
		return GameObject.Find(BALL_OBJ).transform.position;
	}


	/// <summary>
	/// This coroutine organizes the overall tutorial process:
	/// 
	/// 1. Stop LevelManager from creating new enemies.
	/// 2. If the tutorial needs to wait for something (e.g., an enemy coming on screen), do that
	/// 3. Zoom in on some key part of the screen.
	/// 4. Display explanatory text, if any.
	/// 5. Clear the text.
	/// 6. Zoom back out.
	/// 7. Register for the event that will end the tutorial.
	/// 7. Wait until something happens.
	/// 8. Restart the LevelManager.
	/// </summary>
	/// <param name="instruction">The current instruction object.</param>
	private IEnumerator DisplayTutorial(Instruction instruction){
		levelManager.Hold = true;
		tutorialFinished = false;
		currentInstruction.registerFunc();
		yield return new WaitForSeconds(instruction.ZoomDelay);
		Time.timeScale = 0.0f;
		yield return StartCoroutine(ZoomCameraIn(instruction.CameraZoomPos));
		DisplayTutorialText(instruction);
		yield return new WaitForSecondsRealtime(readDelay);
		yield return StartCoroutine(ZoomCameraOut(instruction.CameraZoomPos));
		Time.timeScale = 1.0f;

		//wait until the event handler sets tutorialFinished = true
		while (!tutorialFinished){
			yield return null;
		}

		ClearTutorialText();
			
		levelManager.Hold = false;
		yield break;
	}


	/// <summary>
	/// A specialized coroutine for the setup to the powers tutorial
	/// </summary>
	private IEnumerator StartPowersTutorial(){
		levelManager.Hold = true; //stop LevelManager from making new enemies

		//trigger the powers as an example of their existence
		Transform players = GameObject.Find(PLAYER_ORGANIZER).transform;
		foreach (Transform player in players){
			player.GetComponent<CatchBehavior>().AwesomeCatch();
		}

		//show explanatory text
		DisplayTutorialText(currentInstruction);

		//let the example powers end
		yield return new WaitForSeconds(powerDuration);

		//have a player throw the ball
		bool waitingForExampleThrow = true;
		while (waitingForExampleThrow){
			foreach (Transform player in players){
				if (player.GetComponent<PlayerBallInteraction>().BallCarrier){
					player.GetComponent<PlayerBallInteraction>().Throw();

					waitingForExampleThrow = false;
				}
			}

			yield return null;
		}

		//wait for the awesome catch particle
		tutorialFinished = false;
		CatchParticleRegistration();
		while (!tutorialFinished){
			yield return null;
		}

		//stop time with the ball in the air, showing the special particle
		Time.timeScale = 0.0f;

		//start the normal tutorial sequence at this point
		yield break;
	}


	public void ResetTutorials(){
		StopAllCoroutines();
		ClearTutorialText();
	}


	//////////////////////////////////////////////////////
	/// FUNCTIONS USED BY ALL TUTORIALS
	//////////////////////////////////////////////////////

	private void DisplayTutorialText(Instruction instruction){
//		Debug.Log("Top left instruction: " + instruction.TopLeftText);
//		Debug.Log("Top right instruction: " + instruction.TopRightText);
//		Debug.Log("Bottom right instruction: " + instruction.BottomRightText);
//		Debug.Log("Top left display: " + instructionsTopLeft.text);
//		Debug.Log("Top right display: " + instructionsTopRight.text);
//		Debug.Log("Bottom right display: " + instructionsBottomRight.text);
		instructionsTopLeft.text = instruction.TopLeftText;
		instructionsTopRight.text = instruction.TopRightText;
		instructionsBottomRight.text = instruction.BottomRightText;
//		Debug.Log("Top left instruction: " + instruction.TopLeftText);
//		Debug.Log("Top right instruction: " + instruction.TopRightText);
//		Debug.Log("Bottom right instruction: " + instruction.BottomRightText);
//		Debug.Log("Top left display: " + instructionsTopLeft.text);
//		Debug.Log("Top right display: " + instructionsTopRight.text);
//		Debug.Log("Bottom right display: " + instructionsBottomRight.text.ToString());
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
	/// FUNCTIONS THAT REGISTER FOR EVENTS
	//////////////////////////////////////////////////////


	private void EnemyDestroyedRegistration(){
		eventHandler = new Event.Handler(currentInstruction.eventHandlerFunc);
		Services.EventManager.Register<EnemyDestroyedEvent>(eventHandler);
	}


	private void PassRegistration(){
		eventHandler = new Event.Handler(currentInstruction.eventHandlerFunc);
		Services.EventManager.Register<PassEvent>(eventHandler);
	}


	private void CatchParticleRegistration(){
		eventHandler = new Event.Handler(CatchParticleFunc);
		Services.EventManager.Register<PowerReadyEvent>(eventHandler);
	}


	private void PowerTriggeredRegistration(){
		eventHandler = new Event.Handler(currentInstruction.eventHandlerFunc);
		Services.EventManager.Register<PowerTriggeredEvent>(eventHandler);
	}


	//////////////////////////////////////////////////////
	/// FUNCTIONS USED TO STOP INDIVIDUAL TUTORIALS
	//////////////////////////////////////////////////////


	//used to stop the tutorials about blocking and the player hunt enemy
	//Listens for when an enemy is destroyed; assuming it's a tutorial enemy, signals that the tutorial is finished
	public void EnemyDestroyedFunc(Event e){
		//Debug.Assert(e.GetType() == EnemyDestroyedEvent);

		EnemyDestroyedEvent enemyDestroyedEvent = e as EnemyDestroyedEvent;

		if (enemyDestroyedEvent.enemy.name.Contains(TUTORIAL)){
			Services.EventManager.Unregister<EnemyDestroyedEvent>(eventHandler);
			tutorialFinished = true;
		}
	}


	//used to stop the player passing tutorial
	//listens for when a player passes; when a player does, signals that the tutorial is finished
	public void PlayerPassFunc(Event e){
		Services.EventManager.Unregister<PassEvent>(eventHandler);
		tutorialFinished = true;
	}


	//used to stop the first part of the powers tutorial
	//listens for when the awesome catch particle is active
	public void CatchParticleFunc(Event e){
		Services.EventManager.Unregister<PowerReadyEvent>(eventHandler);
		tutorialFinished = true;
	}


	//used to stop the powers tutorial
	//listens for the first use of a power after the tutorial ends
	public void PowerTriggeredFunc(Event e){
		Services.EventManager.Unregister<PowerTriggeredEvent>(eventHandler);
		tutorialFinished = true;
	}


	//////////////////////////////////////////////////////
	/// Private class used to create tutorials
	//////////////////////////////////////////////////////

	private class Instruction{
		public string TopLeftText { get; private set; } //the text field at the top-left of the screen
		public string TopRightText { get; private set; } //text field at the top-right
		public string BottomRightText { get; private set; } //text field at the bottom-right
		public Vector3 CameraZoomPos { get; set; } //where the "Cameras" object should lerp to
		public delegate void RegisterFunc();
		public RegisterFunc registerFunc { get; private set; } //function that registers for events relevant to this tutorial
		public delegate void EventHandlerFunc(Event e);
		public EventHandlerFunc eventHandlerFunc { get; private set; } //the function that will end the tutorial
		public float ZoomDelay { get; private set; } //if an enemy needs to come on screen, set the delay for that here


		public Instruction(string topLeft,
						   string topRight,
						   string bottomRight,
						   Vector3 camPos,
						   RegisterFunc registerFunc,
						   EventHandlerFunc eventHandlerFunc,
						   float zoomDelay){
			TopLeftText = topLeft;
			TopRightText = topRight;
			BottomRightText = bottomRight;
			CameraZoomPos = camPos;
			this.registerFunc = registerFunc;
			this.eventHandlerFunc = eventHandlerFunc;
			ZoomDelay = zoomDelay;
		}
	}
}
