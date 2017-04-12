/*
 * 
 * All interactions between the player and the ball begin here, including catching and throwing.
 * BallBehavior knows how to throw and how to be caught; this just tells BallBehavior that it's time to do those things.
 * 
 */

using UnityEngine;
using System.Collections;

public class PlayerBallInteraction : MonoBehaviour {

	private const string BALL_OBJ = "Ball";
	private BallBehavior ballBehavior;


	private Transform myCycleObj;
	private const string CYCLE_OBJ = "Cycle and rider";


	private const string PLAYER_ORGANIZER = "Players";
	private const string PLAYER_1_NAME = "Player 1";
	private const string PLAYER_2_NAME = "Player 2";
	private Transform otherPlayer;

	private bool ballCarrier = false;
	public bool BallCarrier{
		get { return ballCarrier; }
		set { ballCarrier = value; }
	}

	public float verticalOffset = 2.0f;

	private CatchBehavior catchBehavior;


	//---------------SCORING---------------
	/*
	 * 
	 * All variables relating to scoring go here.
	 * 
	 * IMPORTANT: the strings here must match the strings in ScoreManager; otherwise ScoreManager won't
	 * know where to record the information.
	 * 
	 * 
	 */
	//variables relating to the score manager itself
	//private ScoreManager scoreManager;
	private const string MANAGER_OBJ = "Managers";

	//types of scoring that this script will record. All of these must appear, identically, in ScoreManager.
	private const string NUMBER_OF_PASSES = "number of passes";


	private void Start(){
		ballBehavior = GameObject.Find(BALL_OBJ).GetComponent<BallBehavior>();
		myCycleObj = transform.Find(CYCLE_OBJ);
		otherPlayer = GetOtherPlayer();
		//scoreManager = GameObject.Find(MANAGER_OBJ).GetComponent<ScoreManager>();
		catchBehavior = GetComponent<CatchBehavior>();
	}

	/// <summary>
	/// Find the other player's transform, so that this player can pass to them.
	/// </summary>
	/// <returns>The other player's transform.</returns>
	private Transform GetOtherPlayer(){
		if (transform.name == PLAYER_1_NAME){
			return GameObject.Find(PLAYER_2_NAME).transform;
		} else {
			return GameObject.Find(PLAYER_1_NAME).transform;
		}
	}


	/// <summary>
	/// Picks up the ball when the player encounters it.
	/// </summary>
	/// <param name="other">The collider the player has encountered.</param>
	private void OnTriggerEnter(Collider other){
		if (other.transform.name.Contains(BALL_OBJ)){
			other.transform.GetComponent<BallBehavior>().GetCaught(transform);
			BallCarrier = true;
		}
	}

	/// <summary>
	/// InputManager calls this to start the process of throwing the ball.
	/// </summary>
	public void Throw(){
		if (myCycleObj.Find(BALL_OBJ)){ //sanity check to make sure this player has the ball; avoids null references
			ballBehavior.Pass(transform.Find(BALL_OBJ).position, otherPlayer);
			//scoreManager.Score(NUMBER_OF_PASSES, gameObject.name);
			BallCarrier = false;

			//send out a pass event; the passing tutorial needs to receive these before the players go on
			Services.EventManager.Fire(new PassEvent(gameObject, otherPlayer.gameObject));
		}
	}
}
