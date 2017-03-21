/*
 * 
 * Each player has this script. It determines when they have made an awesome catch, and then calls other functions
 * appropriately.
 * 
 * This script does not determine what awesome catches do--all of those effects are contained in CatchSandbox.
 * All this script does is figure out when one of CatchSandbox's functions should be called.
 * 
 */
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CatchBehavior : CatchSandbox {

	//----------Tunable variables----------

	[Header("Difficulty and SFX for awesome catches")]
	public float awesomeCatchDistance = 10.0f; //how much leeway players have to get the awesome catch
	public int catchResolution = 75; //new horizontal and vertical pixel resolution as juice for awesome catches
	public float catchResolutionChangeDuration = 0.5f;


	//----------Internal variables----------


	//variables for determining whether an awesome catch occurred
	private PlayerBallInteraction myBallScript;
	private PlayerBallInteraction otherBallScript;
	private const string PLAYER_1_OBJ = "Player 1";
	private const string PLAYER_2_OBJ = "Player 2";
	private Transform ball;
	private const string BALL_OBJ = "Ball";
	private bool readyForAwesomeCatch = false;
	private BallBehavior ballBehavior;


	//variables for keeping track of how many inputs the player has made during this throw, and for slowing players who mash
	private int inputs = 0;
	private PlayerMovement movementScript;


	//this helps discard excess inputs when throwing
	private float cantCatchTimer = 0.0f;


	//variables for juice
	private AlpacaSound.RetroPixelPro.RetroPixelPro pixelScript;


	//SFX for missed special catch
	private AudioClip missClip;
	private const string MISS_CLIP = "Audio/MissedCatchSFX";
	private AudioSource audioSource;


	//text notification for special catch
	private Text catchText;
	private const string TEXT_CANVAS = "Catch warning";
	private const string TEXT_OBJ = "Text";
	private const string MISS = "X";
	private const string CATCH = "OK";
	private const string INCOMING = "!";


	//initialize variables
	protected override void Start(){
		myBallScript = GetComponent<PlayerBallInteraction>();
		otherBallScript = FindOtherBallScript();
		ball = GameObject.Find(BALL_OBJ).transform;
		ballBehavior = ball.GetComponent<BallBehavior>();
		movementScript = GetComponent<PlayerMovement>();
		pixelScript = Camera.main.GetComponent<AlpacaSound.RetroPixelPro.RetroPixelPro>();
		missClip = Resources.Load(MISS_CLIP) as AudioClip;
		audioSource = GetComponent<AudioSource>();
		catchText = transform.Find(TEXT_CANVAS).Find(TEXT_OBJ).GetComponent<Text>();

		base.Start();
	}


	/// <summary>
	/// Every time the player catches the ball, see if it's an awesome catch. Either way, reset the values that determine whether
	/// an awesome catch is possible.
	/// </summary>
	/// <param name="other">The collider the player encountered.</param>
	private void OnTriggerEnter(Collider other){
		if (other.gameObject.name == BALL_OBJ){
			if (readyForAwesomeCatch){
				AwesomeCatch();
			}

			inputs = 0;
			readyForAwesomeCatch = false;
			catchText.text = INCOMING; //reset the UI that tells players the ball is approaching
			catchText.color = Color.white;
			//madeAwesomeCatch = false;
		}
	}


	//	/// <summary>
	//	/// Gets a reference to the other player's ball script, so that this player can tell who has the ball.
	//	/// </summary>
	//	/// <returns>The other player's PlayerBallInteraction script.</returns>
	private PlayerBallInteraction FindOtherBallScript(){
		PlayerBallInteraction temp;

		if (gameObject.name == PLAYER_1_OBJ){
			temp = GameObject.Find(PLAYER_2_OBJ).GetComponent<PlayerBallInteraction>();
		} else if (gameObject.name == PLAYER_2_OBJ){
			temp = GameObject.Find(PLAYER_1_OBJ).GetComponent<PlayerBallInteraction>();
		} else {
			//emergency initialization for when something goes wrong
			temp = GameObject.Find(PLAYER_2_OBJ).GetComponent<PlayerBallInteraction>();
			Debug.Log("Unable to find the other player's PlayerBallInteraction");
		}

		return temp;
	}


	//	/// <summary>
	//	/// Call this function whenever a player tries for the awesome catch.
	//	/// 
	//	/// All the logic for whether or not awesome catches occur is in the if-statements in this function.
	//	/// </summary>
	public void AttemptAwesomeCatch(){
		if (ballBehavior.IntendedReceiver == transform){ //make sure the ball is coming to this player
			if(inputs > 0){ //if this is true, the player has already tried for an awesome catch and is mashing
				movementScript.SlowMaxSpeed();
				readyForAwesomeCatch = false;
				MissedCatchFeedback();
			} else if (Vector3.Distance(transform.position, ball.position) > awesomeCatchDistance){ //if true, pushed button too early--missed catch
				inputs++;
				movementScript.SlowMaxSpeed();
				readyForAwesomeCatch = false;
				MissedCatchFeedback();
			} else { //success! An awesome catch can occur
				readyForAwesomeCatch = true;
				catchText.text = CATCH;
				catchText.color = Color.green;
				//madeAwesomeCatch = true;
			}
		}
	}


	/// <summary>
	/// Chooses the correct awesome catch effect for this player.
	/// </summary>
	/// <returns><c>true</c> so that the awesome catch will happen when the player catches the ball.</returns>
	private void AwesomeCatch(){
		if (gameObject.name == PLAYER_1_OBJ){
			Player1AwesomeCatchEffect();
		} else {
			Player2AwesomeCatchEffect();
		}

		pixelScript.SetTemporaryResolution(catchResolution, catchResolution, catchResolutionChangeDuration);
	}


	/// <summary>
	/// Activate the tether between player 1 and player 2
	/// </summary>
	private void Player1AwesomeCatchEffect(){
		Tether();
	}


	/// <summary>
	/// Cause a bolt of lightning to appear at player 2's location
	/// </summary>
	private void Player2AwesomeCatchEffect(){
		StartCoroutine(MultiBurst(transform.position));
	}


	/// <summary>
	/// Called by BallBehavior to get the distance where the awesome catch can happen.
	/// </summary>
	/// <returns>The awesome catch distance.</returns>
	public float GetAwesomeCatchDistance(){
		return awesomeCatchDistance;
	}



	private void MissedCatchFeedback(){
		if (!audioSource.isPlaying){
			audioSource.clip = missClip;
			audioSource.Play();
		}

		catchText.text = MISS;
		catchText.color = Color.red;
	}
}
