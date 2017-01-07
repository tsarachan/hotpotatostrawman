/*
 * 
 * This script takes in all inputs, and sends them out to the appropriate player scripts.
 * 
 * This script requires that player objects have different characters as the last characters in their names.
 * E.g., "Player 1" and "Player 2" are fine, "Player 1" and "Player 21" will cause a problem.
 * 
 * To add another controller button or axis, follow these steps:
 * 1. Declare a string, and initialize it with the name of the control from the input manager.
 * 2. Declare and initialize a Dictionary<char, [script the control should talk to]>.
 * 3. Add a line in Start for the dictionary, following the pattern of the lines already there.
 * 
 */

using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class InputManager : MonoBehaviour {

	/*
	 * 
	 * Put a string for every control players will use. Use the input axes from the input manager.
	 * Leave off the number for the player.
	 * 
	 */
	private const string VERT_AXIS = "PS4_LStick_Vert_";
	private const string HORIZ_AXIS = "PS4_LStick_Horiz_";
	private const string O_BUTTON = "PS4_O_";

	//these are used to get the player scripts so that inputs can be directed
	private const string PLAYER_ORGANIZER = "Players";
	private Dictionary<char, PlayerMovement> playerMovementScripts = new Dictionary<char, PlayerMovement>();
	private Dictionary<char, PlayerBallInteraction> playerBallInteractionScripts = new Dictionary<char, PlayerBallInteraction>();
	private Dictionary<char, PlayerMovementLean> playerLeanScripts = new Dictionary<char, PlayerMovementLean>();

	//variables relating to the thumbstick
	public float deadZone = 0.3f; //must be between 0.0 and 1.0
	private const string UP = "up";
	private const string DOWN = "down";
	private const string LEFT = "left";
	private const string RIGHT = "right";

	//keyboard controls
	private KeyCode p1UpKey = KeyCode.DownArrow;
	private KeyCode p1DownKey = KeyCode.UpArrow;
	private KeyCode p1LeftKey = KeyCode.LeftArrow;
	private KeyCode p1RightKey = KeyCode.RightArrow;
	private KeyCode p1PassKey = KeyCode.Z;
	private Dictionary<KeyCode, string> p1KeyboardControls = new Dictionary<KeyCode, string>();

	private KeyCode p2UpKey = KeyCode.K;
	private KeyCode p2DownKey = KeyCode.I;
	private KeyCode p2LeftKey = KeyCode.J;
	private KeyCode p2RightKey = KeyCode.L;
	private KeyCode p2PassKey = KeyCode.N;

	private Dictionary<PlayerBallInteraction, KeyCode> passKeys = new Dictionary<PlayerBallInteraction, KeyCode>();
	private Dictionary<KeyCode, string> p2KeyboardControls = new Dictionary<KeyCode, string>();
	private Dictionary<PlayerMovement, Dictionary<KeyCode, string>> movementKeys = 
		new Dictionary<PlayerMovement, Dictionary<KeyCode, string>>();
	private Dictionary<PlayerMovementLean, Dictionary<KeyCode, string>> leanKeys = 
		new Dictionary<PlayerMovementLean, Dictionary<KeyCode, string>>();

	//variables needed to start the game with the first pass
	private LevelManager levelManager;
	private const string ROAD_TREADMILL = "Treadmill";
	private const string BUILDING_ORGANIZER = "Buildings";



	//initialize variables and data structures
	private void Start(){
		foreach (Transform player in GameObject.Find(PLAYER_ORGANIZER).transform){
			playerMovementScripts.Add(player.name.Last(), player.GetComponent<PlayerMovement>());
			playerBallInteractionScripts.Add(player.name.Last(), player.GetComponent<PlayerBallInteraction>());
			playerLeanScripts.Add(player.name.Last(), player.GetComponent<PlayerMovementLean>());
		}

		levelManager = GetComponent<LevelManager>();
		movementKeys = SetUpKeyboardMovement();
		leanKeys = SetUpKeyboardLean();
		passKeys = SetUpKeyboardPassing();
	}


	/// <summary>
	/// Populate dictionaries with keyboard movement controls, so that they can be checked in FixedUpdate().
	/// 
	/// The throw keys are intentionally left out, so that FixedUpdate() can just check movement-related inputs.
	/// </summary>
	/// <returns>A dictionary of dictionaries, which gives access to all player movement keyboard controls.</returns>
	private Dictionary<PlayerMovement, Dictionary<KeyCode, string>> SetUpKeyboardMovement(){
		Dictionary<PlayerMovement, Dictionary<KeyCode, string>> temp = new Dictionary<PlayerMovement, Dictionary<KeyCode, string>>();

		p1KeyboardControls.Add(p1UpKey, UP);
		p1KeyboardControls.Add(p1DownKey, DOWN);
		p1KeyboardControls.Add(p1LeftKey, LEFT);
		p1KeyboardControls.Add(p1RightKey, RIGHT);

		p2KeyboardControls.Add(p2UpKey, UP);
		p2KeyboardControls.Add(p2DownKey, DOWN);
		p2KeyboardControls.Add(p2LeftKey, LEFT);
		p2KeyboardControls.Add(p2RightKey, RIGHT);

		foreach (char key in playerMovementScripts.Keys){
			if (key == '1'){
				temp.Add(playerMovementScripts[key], p1KeyboardControls);
			} else if (key == '2'){
				temp.Add(playerMovementScripts[key], p2KeyboardControls);
			}
		}

		return temp;
	}

	/// <summary>
	/// Populate dictionaries with keyboard controls, so that they can be checked in FixedUpdate().
	/// </summary>
	/// <returns>A dictionary of dictionaries, which gives access to keyboard controls for leaning.</returns>
	private Dictionary<PlayerMovementLean, Dictionary<KeyCode, string>> SetUpKeyboardLean(){
		Dictionary<PlayerMovementLean, Dictionary<KeyCode, string>> temp = 
			new Dictionary<PlayerMovementLean, Dictionary<KeyCode, string>>();

		foreach (char key in playerLeanScripts.Keys){
			if (key == '1'){
				temp.Add(playerLeanScripts[key], p1KeyboardControls);
			} else if (key == '2'){
				temp.Add(playerLeanScripts[key], p2KeyboardControls);
			}
		}

		return temp;
	}


	/// <summary>
	/// Adds keyboard passing keys to a dictionary that can be checked in Update().
	/// </summary>
	/// <returns>A dictionary of keyboard keys used for passing.</returns>
	private Dictionary<PlayerBallInteraction, KeyCode> SetUpKeyboardPassing(){
		Dictionary<PlayerBallInteraction, KeyCode> temp = new Dictionary<PlayerBallInteraction, KeyCode>();

		foreach (char key in playerBallInteractionScripts.Keys){
			if (key == '1'){
				temp.Add(playerBallInteractionScripts[key], p1PassKey);
			} else if (key == '2'){
				temp.Add(playerBallInteractionScripts[key], p2PassKey);
			}
		}

		return temp;
	}

	/// <summary>
	/// Send button presses to player scripts when players input them.
	/// </summary>
	private void Update(){
		//controller buttons
		foreach (char player in playerBallInteractionScripts.Keys){
			if (Input.GetButtonDown(O_BUTTON + player)){
				//if a player has picked up the ball, start the game upon the first pass
				//this will try to keep restarting the game--it's inefficient, but not causing performance problems
				if (playerBallInteractionScripts[player].BallCarrier){
					StartGame();
				}

				playerBallInteractionScripts[player].Throw();
			}
		}

		//keyboard controls
		foreach (PlayerBallInteraction script in passKeys.Keys){
			if (Input.GetKeyDown(passKeys[script])){
				//if a player has picked up the ball, start the game upon the first pass
				//this will try to keep restarting the game--it's inefficient, but not causing performance problems
				if (script.BallCarrier){
					StartGame();
				}

				script.Throw();
			}
		}
	}

	/// <summary>
	/// Send movement instructions to player scripts when their players move the thumbstick or hit a keyboard key.
	/// 
	/// This is in FixedUpdate on the assumption that players move with physics.
	/// </summary>
	private void FixedUpdate(){
		//thumbstick controls
		foreach (char player in playerMovementScripts.Keys){
			if (Input.GetAxis(VERT_AXIS + player) > deadZone){
				playerMovementScripts[player].Move(UP);

				//sanity check: playerLeanScripts and playerMovementScripts should always have the same keys,
				//but make sure to avoid null reference exceptions
				if (playerLeanScripts.ContainsKey(player)){
					playerLeanScripts[player].Lean(UP);
				}
			}
			else if (Input.GetAxis(VERT_AXIS + player) < -deadZone){
				playerMovementScripts[player].Move(DOWN);

				if (playerLeanScripts.ContainsKey(player)){
					playerLeanScripts[player].Lean(DOWN);
				}
			}

			if (Input.GetAxis(HORIZ_AXIS + player) < -deadZone){
				playerMovementScripts[player].Move(LEFT);

				if (playerLeanScripts.ContainsKey(player)){
					playerLeanScripts[player].Lean(LEFT);
				}
			}
			else if (Input.GetAxis(HORIZ_AXIS + player) > deadZone){
				playerMovementScripts[player].Move(RIGHT);

				if (playerLeanScripts.ContainsKey(player)){
					playerLeanScripts[player].Lean(RIGHT);
				}
			}
		}

		//keyboard controls
		//check each movement key for each player; if it's pressed, send the instruction to move
		foreach (PlayerMovement moveScript in movementKeys.Keys){
			foreach (KeyCode key in movementKeys[moveScript].Keys){
				if (Input.GetKey(key)){
					moveScript.Move(movementKeys[moveScript][key]);
				}
			}
		}

		//check each movement key for each player; if it's pressed, send the instruction to lean
		foreach (PlayerMovementLean leanScript in leanKeys.Keys){
			foreach (KeyCode key in leanKeys[leanScript].Keys){
				if (Input.GetKey(key)){
					leanScript.Lean(leanKeys[leanScript][key]);
				}
			}
		}
	}


	/// <summary>
	/// Do everything required to set the game in motion.
	/// </summary>
	private void StartGame(){
		levelManager.GameHasStarted = true;

		foreach (Transform roadSection in GameObject.Find(ROAD_TREADMILL).transform){
			roadSection.GetComponent<EnvironmentMove>().GameHasStarted = true;
		}

		foreach (Transform building in GameObject.Find(BUILDING_ORGANIZER).transform){
			building.GetComponent<EnvironmentMove>().GameHasStarted = true;
		}
	}
}
