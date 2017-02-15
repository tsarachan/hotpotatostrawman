/*
 * 
 * This script takes in all inputs, and sends them out to the appropriate player scripts.
 * 
 * This script requires that player objects have successive numbers as the last characters in their names,
 * starting with 1.
 * E.g., "Player 1" and "Player 2" are fine, "Player 1" and "Player 21" will cause a problem, as will "Player A."
 * 
 * Every player's scripts and keyboard controls are contained in a Player object. The Player class is at
 * the bottom of this script.
 * 
 * To add a new script that receives inputs, follow these steps:
 * 1. Declare the script in the Player class.
 * 2. Add the script to the Player constructor, following the format the existing scripts use.
 * 3. Get the script component in MakePlayers().
 * 
 */

using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class InputManager : MonoBehaviour {
	

	//gamepad axes and buttons. Player numbers are intentionally left off.
	private const string VERT_AXIS = "PS4_LStick_Vert_";
	private const string HORIZ_AXIS = "PS4_LStick_Horiz_";
	private const string O_BUTTON = "PS4_O_";

	private const string VERT_AXIS_360 = "360_LStick_Vert_";
	private const string HORIZ_AXIS_360 = "360_LStick_Horiz_";
	private const string A_BUTTON = "360_A_";


	//the players
	private const string PLAYER_ORGANIZER = "Players";
	private const string PLAYER_OBJ = "Player ";
	private Dictionary<char, Player> players = new Dictionary<char, Player>();


	//variables relating to the thumbstick
	public float deadZone = 0.3f; //must be between 0.0 and 1.0
	private const string UP = "up";
	private const string DOWN = "down";
	private const string LEFT = "left";
	private const string RIGHT = "right";


	//keyboard controls
	private const KeyCode p1UpKey = KeyCode.S;
	private const KeyCode p1DownKey = KeyCode.W;
	private const KeyCode p1LeftKey = KeyCode.A;
	private const KeyCode p1RightKey = KeyCode.D;
	private const KeyCode p1PassKey = KeyCode.Z;

	private const KeyCode p2UpKey = KeyCode.K;
	private const KeyCode p2DownKey = KeyCode.I;
	private const KeyCode p2LeftKey = KeyCode.J;
	private const KeyCode p2RightKey = KeyCode.L;
	private const KeyCode p2PassKey = KeyCode.N;


	//additional variables needed to start the game with the first pass
	private LevelManager levelManager;
	private const string ROAD_TREADMILL = "Treadmill";
	private const string BUILDING_ORGANIZER = "Buildings";



	//initialize variables and data structures
	private void Start(){
		players = MakePlayers();
		levelManager = GetComponent<LevelManager>();
	}


	/// <summary>
	/// Creates an object for each player. The object contains references to all of the scripts that player
	/// needs to send input to, as well as the player's keyboard controls.
	/// </summary>
	/// <returns>A dictionary containing the player objects, indexed by the player's number.</returns>
	private Dictionary<char, Player> MakePlayers(){
		Dictionary<char, Player> temp = new Dictionary<char, Player>();

		for (int i = 1; i < 3; i++){
			Transform playerObj = transform.root.Find(PLAYER_ORGANIZER).Find(PLAYER_OBJ + i.ToString());

			if (i == 1){
				Player newPlayer = new Player(char.Parse(i.ToString()),
											  playerObj.GetComponent<PlayerMovement>(),
											  playerObj.GetComponent<PlayerBallInteraction>(),
											  playerObj.GetComponent<PlayerMovementLean>(),
											  playerObj.GetComponent<CatchBehavior>(),
											  p1UpKey,
											  p1DownKey,
											  p1LeftKey,
											  p1RightKey,
											  p1PassKey);
				temp.Add(char.Parse(i.ToString()), newPlayer);
			} else if (i == 2) {
				Player newPlayer = new Player(char.Parse(i.ToString()),
											  playerObj.GetComponent<PlayerMovement>(),
											  playerObj.GetComponent<PlayerBallInteraction>(),
											  playerObj.GetComponent<PlayerMovementLean>(),
											  playerObj.GetComponent<CatchBehavior>(),
											  p2UpKey,
											  p2DownKey,
											  p2LeftKey,
											  p2RightKey,
											  p2PassKey);
				temp.Add(char.Parse(i.ToString()), newPlayer);
			} else {
				Debug.Log("Illegal player number: " + i);
			}
		}

		if (temp.Count == 2) {
			return temp;
		} else {
			Debug.Log("Illegal number of players: " + temp.Count);
			return temp;
		}
	}


	/// <summary>
	/// Send button presses to player scripts when players input them.
	/// </summary>
	private void Update(){
		//controller buttons
		foreach (char player in players.Keys){
			if (Input.GetButtonDown(O_BUTTON + player) ||
				Input.GetButtonDown(A_BUTTON + player)){
				//if a player has picked up the ball, start the game upon the first pass
				//this will try to keep restarting the game--it's inefficient, but not causing performance problems
				if (players[player].BallScript.BallCarrier){
					StartGame();
				}

				players[player].BallScript.Throw();
				players[player].CatchScript.AttemptAwesomeCatch();
			}
		}

		/* 
		 * 
		 * Player keyboard controls are checked in FixedUpdate(). This may lead to slight variations in play
		 * between keyboard and controller play, since FixedUpdate() can run more than once per frame.
		 * 
		 */
	}


	/// <summary>
	/// Send instructions to player scripts when their players move the thumbstick or hit a keyboard key.
	/// 
	/// This is in FixedUpdate on the assumption that players move with physics.
	/// </summary>
	private void FixedUpdate(){
		//thumbstick controls
		foreach (char player in players.Keys){
			if (Input.GetAxis(VERT_AXIS + player) > deadZone ||
				Input.GetAxis(VERT_AXIS_360 + player) > deadZone){
				players[player].MoveScript.Move(UP);
				players[player].LeanScript.Lean(UP);
			}
			else if (Input.GetAxis(VERT_AXIS + player) < -deadZone ||
					 Input.GetAxis(VERT_AXIS_360 + player) < -deadZone){
				players[player].MoveScript.Move(DOWN);
				players[player].LeanScript.Lean(DOWN);
			}

			if (Input.GetAxis(HORIZ_AXIS + player) < -deadZone ||
				Input.GetAxis(HORIZ_AXIS_360 + player) < -deadZone){
				players[player].MoveScript.Move(LEFT);
				players[player].LeanScript.Lean(LEFT);
			}
			else if (Input.GetAxis(HORIZ_AXIS + player) > deadZone ||
					 Input.GetAxis(HORIZ_AXIS_360 + player) > deadZone){
				players[player].MoveScript.Move(RIGHT);
				players[player].LeanScript.Lean(RIGHT);
			}
		}

		//keyboard controls
		foreach (char player in players.Keys){
			foreach (KeyCode control in players[player].keyboardControls){
				if (Input.GetKey(control)){
					InputByKey(player, control);
				}
			}
		}
	}


	/// <summary>
	/// Handles all keyboard inputs.
	/// </summary>
	/// <param name="numOfPlayer">The dictionary index for the player who gave the input.</param>
	/// <param name="keyPressed">The key pressed.</param>
	private void InputByKey(char numOfPlayer, KeyCode keyPressed){
		switch(keyPressed){
			case p1UpKey:
			case p2UpKey:
				players[numOfPlayer].MoveScript.Move(UP);
				players[numOfPlayer].LeanScript.Lean(UP);
				break;
			case p1DownKey:
			case p2DownKey:
				players[numOfPlayer].MoveScript.Move(DOWN);
				players[numOfPlayer].LeanScript.Lean(DOWN);
				break;
			case p1LeftKey:
			case p2LeftKey:
				players[numOfPlayer].MoveScript.Move(LEFT);
				players[numOfPlayer].LeanScript.Lean(LEFT);
				break;
			case p1RightKey:
			case p2RightKey:
				players[numOfPlayer].MoveScript.Move(RIGHT);
				players[numOfPlayer].LeanScript.Lean(RIGHT);
				break;
			case p1PassKey:
			case p2PassKey:
				players[numOfPlayer].BallScript.Throw();
				players[numOfPlayer].CatchScript.AttemptAwesomeCatch();
				StartGame();
				break;
			default:
				Debug.Log("Illegal key: " + keyPressed.ToString());
				break;
		}
	}


	/// <summary>
	/// Do everything required to set the game in motion.
	/// </summary>
	private void StartGame(){
		levelManager.StartGame(gameObject);

		foreach (Transform roadSection in GameObject.Find(ROAD_TREADMILL).transform){
			roadSection.GetComponent<EnvironmentMove>().GameHasStarted = true;
		}

		foreach (Transform building in GameObject.Find(BUILDING_ORGANIZER).transform){
			building.GetComponent<EnvironmentMove>().GameHasStarted = true;
		}

		foreach (Transform player in GameObject.Find(PLAYER_ORGANIZER).transform){
			player.GetComponent<BackwardsTrail>().StartGame();
		}
	}


	/// <summary>
	/// This class contains references to each script a player needs to send inputs to.
	/// It also includes all of a player's keyboard controls.
	/// 
	/// To make a new player, use the syntax that appears in MakePlayers(), above.
	/// 
	/// </summary>
	private class Player {
		//scripts each player has that receive input
		private PlayerMovement moveScript;
		private PlayerBallInteraction ballScript;
		private PlayerMovementLean leanScript;
		private CatchBehavior catchScript;

		public PlayerMovement MoveScript { get { return moveScript; } }
		public PlayerBallInteraction BallScript { get { return ballScript; } }
		public PlayerMovementLean LeanScript { get { return leanScript; } }
		public CatchBehavior CatchScript { get { return catchScript; } }

		//keyboard controls
		private KeyCode upKey;
		private KeyCode downKey;
		private KeyCode leftKey;
		private KeyCode rightKey;
		private KeyCode passKey;
		public List<KeyCode> keyboardControls = new List<KeyCode>();

		//gamepad controls
		private char playerNum;

		//constructor for players
		public Player(char playerNum,
					  PlayerMovement moveScript,
					  PlayerBallInteraction ballScript,
					  PlayerMovementLean leanScript,
					  CatchBehavior catchScript,
					  KeyCode upKey,
					  KeyCode downKey,
					  KeyCode leftKey,
					  KeyCode rightKey,
					  KeyCode passKey){
			this.playerNum = playerNum;
			this.moveScript = moveScript;
			this.ballScript = ballScript;
			this.leanScript = leanScript;
			this.catchScript = catchScript;
			this.upKey = upKey;
			this.downKey = downKey;
			this.leftKey = leftKey;
			this.rightKey = rightKey;
			this.passKey = passKey;
			this.keyboardControls.Add(upKey);
			this.keyboardControls.Add(downKey);
			this.keyboardControls.Add(leftKey);
			this.keyboardControls.Add(rightKey);
			this.keyboardControls.Add(passKey);
		}
	}
}
