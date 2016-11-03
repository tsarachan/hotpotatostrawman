/*
 * 
 * This script takes in all inputs, and sends them out to the appropriate player scripts.
 * 
 * This script requires that player objects have different characters as the last characters in their names.
 * E.g., "Player 1" and "Player 2" are fine, "Player 1" and "Player 21" will cause a problem.
 * 
 * To add another control, follow these steps:
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

	//variables relating to the thumbstick
	public float deadZone = 0.3f; //must be between 0.0 and 1.0
	private const string UP = "up";
	private const string DOWN = "down";
	private const string LEFT = "left";
	private const string RIGHT = "right";


	//initialize data structures
	private void Start(){
		foreach (Transform player in GameObject.Find(PLAYER_ORGANIZER).transform){
			playerMovementScripts.Add(player.name.Last(), player.GetComponent<PlayerMovement>());
			playerBallInteractionScripts.Add(player.name.Last(), player.GetComponent<PlayerBallInteraction>());
		}
	}

	/// <summary>
	/// Send button presses to player scripts when players input them.
	/// </summary>
	private void Update(){
		foreach (char player in playerBallInteractionScripts.Keys){
			if (Input.GetButtonDown(O_BUTTON + player)){
				playerBallInteractionScripts[player].ThrowBall();
			}
		}
	}

	/// <summary>
	/// Send movement instructions to player scripts when their players move the thumbstick.
	/// 
	/// This is in FixedUpdate on the assumption that players move with physics.
	/// </summary>
	private void FixedUpdate(){
		foreach (char player in playerMovementScripts.Keys){
			if (Input.GetAxis(VERT_AXIS + player) > deadZone) { playerMovementScripts[player].Move(UP); }
			else if (Input.GetAxis(VERT_AXIS + player) < -deadZone) { playerMovementScripts[player].Move(DOWN); }

			if (Input.GetAxis(HORIZ_AXIS + player) < -deadZone) { playerMovementScripts[player].Move(LEFT); }
			else if (Input.GetAxis(HORIZ_AXIS + player) > deadZone) { playerMovementScripts[player].Move(RIGHT); }
		}
	}
}
