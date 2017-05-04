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

using Rewired;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class InputManager : MonoBehaviour {
	
	//----------Tunable variables----------

	public float resetDelay = 3.0f; //how long the players have to hold the button while paused to reset the game
	public float deadZone = 0.3f; //input deadzone


	//----------Internal variables----------

	//gamepad axes and buttons. Player numbers are intentionally left off.
	private const string VERT_AXIS = "PS4_LStick_Vert_";
	private const string HORIZ_AXIS = "PS4_LStick_Horiz_";
	private const string O_BUTTON = "PS4_O_";
	private const string PS_BUTTON = "PS4_PS_";
	private const string PS_OPTIONS_BUTTON = "PS4_Options_";

	private const string VERT_AXIS_360 = "360_LStick_Vert_";
	private const string HORIZ_AXIS_360 = "360_LStick_Horiz_";
	private const string A_BUTTON = "360_A_";


	//the players
	private const string PLAYER_ORGANIZER = "Players";
	private const string PLAYER_OBJ = "Player ";
	private Dictionary<char, PlayerInfo> players = new Dictionary<char, PlayerInfo>();


	//additional variables needed to start the game with the first pass
	private LevelManager levelManager;
	private const string ROAD_TREADMILL = "Treadmill";
	private const string BUILDING_ORGANIZER = "Buildings";


	//variables for the pause menu with controller map
	private PauseMenuBehavior pauseMenuScript;
	private const string UI_CANVAS_OBJ = "UI canvas";
	private const string CONTROLLER_MAP_OBJ = "Controller map";


	//used to end the game
	//private bool paused = true;
	private float resetTimer = 0.0f;
	private GameEndSystem gameEndSystem;


	//directions sent to other scripts
	private const string UP = "up";
	private const string DOWN = "down";
	private const string LEFT = "left";
	private const string RIGHT = "right";


	//initialize variables and data structures
	private void Start(){
		players = MakePlayers();
		levelManager = GetComponent<LevelManager>();
		pauseMenuScript = GameObject.Find(UI_CANVAS_OBJ).transform.Find(CONTROLLER_MAP_OBJ)
			.GetComponent<PauseMenuBehavior>();
		gameEndSystem = GetComponent<GameEndSystem>();
		StartGame();
	}


	/// <summary>
	/// Creates an object for each player. The object contains references to all of the scripts that player
	/// needs to send input to, as well as the player's keyboard controls.
	/// </summary>
	/// <returns>A dictionary containing the player objects, indexed by the player's number.</returns>
	private Dictionary<char, PlayerInfo> MakePlayers(){
		Dictionary<char, PlayerInfo> temp = new Dictionary<char, PlayerInfo>();

		for (int i = 1; i < 3; i++){
			Transform playerObj = transform.root.Find(PLAYER_ORGANIZER).Find(PLAYER_OBJ + i.ToString());

			if (i == 1){
				PlayerInfo newPlayer = new PlayerInfo(ReInput.players.GetPlayer(i - 1),
											  playerObj.GetComponent<PlayerMovement>(),
											  playerObj.GetComponent<PlayerBallInteraction>(),
											  playerObj.GetComponent<PlayerMovementLean>(),
											  playerObj.GetComponent<CatchBehavior>(),
											  playerObj.GetComponent<PlayerMovementParticles>());
				temp.Add(char.Parse(i.ToString()), newPlayer);
			} else if (i == 2) {
				PlayerInfo newPlayer = new PlayerInfo(ReInput.players.GetPlayer(i - 1),
											  playerObj.GetComponent<PlayerMovement>(),
											  playerObj.GetComponent<PlayerBallInteraction>(),
											  playerObj.GetComponent<PlayerMovementLean>(),
											  playerObj.GetComponent<CatchBehavior>(),
											  playerObj.GetComponent<PlayerMovementParticles>());
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
		foreach (char player in players.Keys){
			if (players[player].ThisPlayer.GetAxis("Move Vert") < -deadZone){
				players[player].MoveScript.Move(UP);
				players[player].LeanScript.Lean(UP);
				players[player].ParticleScript.GetInput(UP);
				gameEndSystem.ResetInputTimer();
			}
			else if (players[player].ThisPlayer.GetAxis("Move Vert") > deadZone){
				players[player].MoveScript.Move(DOWN);
				players[player].LeanScript.Lean(DOWN);
				players[player].ParticleScript.GetInput(DOWN);
				gameEndSystem.ResetInputTimer();
			}

			if (players[player].ThisPlayer.GetAxis("Move Horiz") < -deadZone){
				players[player].MoveScript.Move(LEFT);
				players[player].LeanScript.Lean(LEFT);
				players[player].ParticleScript.GetInput(LEFT);
				gameEndSystem.ResetInputTimer();
			}
			else if (players[player].ThisPlayer.GetAxis("Move Horiz") > deadZone){
				players[player].MoveScript.Move(RIGHT);
				players[player].LeanScript.Lean(RIGHT);
				players[player].ParticleScript.GetInput(RIGHT);
				gameEndSystem.ResetInputTimer();
			}
		}


		//controller buttons
		foreach (char player in players.Keys){
			if (!pauseMenuScript.Paused && 
				players[player].ThisPlayer.GetButtonDown("Pass")){

				players[player].BallScript.Throw();
				players[player].CatchScript.AttemptAwesomeCatch();
				gameEndSystem.ResetInputTimer();
			}

			if (players[player].ThisPlayer.GetButtonDown("Start")){
				pauseMenuScript.ChangePauseMenuState();
			}
		}


		if (players['1'].ThisPlayer.GetButton("Pass") &&
			players['2'].ThisPlayer.GetButton("Pass") &&
			pauseMenuScript.Paused){

			resetTimer += Time.unscaledDeltaTime;

			if (resetTimer >= resetDelay){
				resetTimer = 0.0f;
				gameEndSystem.VoluntaryStop();
			}
		} else {
			resetTimer = 0.0f;
		}


		/* 
		 * 
		 * Player keyboard controls for movement and passing are checked in FixedUpdate().
		 * This may lead to slight variations in play between keyboard and controller play,
		 * since FixedUpdate() can run more than once per frame.
		 * 
		 */
	}


	/// <summary>
	/// Do everything required to set the game in motion.
	/// </summary>
	public void StartGame(){
		levelManager.StartGame(gameObject);

		foreach (Transform roadSection in GameObject.Find(ROAD_TREADMILL).transform){
			roadSection.GetComponent<EnvironmentMove>().GameHasStarted = true;
		}

		foreach (Transform building in GameObject.Find("Buildings").transform){
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
	private class PlayerInfo {
		//scripts each player has that receive input
		private Player thisPlayer;
		private PlayerMovement moveScript;
		private PlayerBallInteraction ballScript;
		private PlayerMovementLean leanScript;
		private CatchBehavior catchScript;
		private PlayerMovementParticles particleScript;

		public Player ThisPlayer { get { return thisPlayer; } }
		public PlayerMovement MoveScript { get { return moveScript; } }
		public PlayerBallInteraction BallScript { get { return ballScript; } }
		public PlayerMovementLean LeanScript { get { return leanScript; } }
		public CatchBehavior CatchScript { get { return catchScript; } }
		public PlayerMovementParticles ParticleScript { get { return particleScript; } }


		//constructor for players
		public PlayerInfo(Player thisPlayer,
					  PlayerMovement moveScript,
					  PlayerBallInteraction ballScript,
					  PlayerMovementLean leanScript,
					  CatchBehavior catchScript,
					  PlayerMovementParticles particleScript){
			this.thisPlayer = thisPlayer;
			this.moveScript = moveScript;
			this.ballScript = ballScript;
			this.leanScript = leanScript;
			this.catchScript = catchScript;
			this.particleScript = particleScript;
		}
	}
}
