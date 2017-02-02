/*
 * 
 * Each player has this script. It determines when they have made an awesome catch, and then calls other functions
 * appropriately.
 * 
 * This script does not carry out the awesome catch's effect. It only establishes that an awesome catch occurred.
 * 
 */
using System.Collections;
using System.Linq;
using UnityEngine;

public class CatchBehavior : MonoBehaviour {

	//----------Tunable variables----------
	public float awesomeCatchDistance = 10.0f; //how much leeway players have to get the awesome catch
	public int catchResolution = 75; //new horizontal and vertical pixel resolution as juice for awesome catches
	public float catchResolutionChangeDuration = 0.5f;


	//----------Internal variables----------

	//variables used to load the player powers
	private GameObject myPower;
	private const string LIGHTNING_OBJ = "Burst prefab";
	private const string TETHER_OBJ = "Lightning prefab";
	private const string PARTICLE_ORGANIZER = "Particles";


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


	//initialize variables
	private void Start(){
		myPower = LoadMyPower();
		myBallScript = GetComponent<PlayerBallInteraction>();
		otherBallScript = FindOtherBallScript();
		ball = GameObject.Find(BALL_OBJ).transform;
		ballBehavior = ball.GetComponent<BallBehavior>();
		movementScript = GetComponent<PlayerMovement>();
		pixelScript = Camera.main.GetComponent<AlpacaSound.RetroPixelPro.RetroPixelPro>();
	}


	/// <summary>
	/// Finds the appropriate prefab for each player's catch power, and then sets any necessary initial states.
	/// </summary>
	/// <returns>A gameobject, either instantiated or loaded from Resources, for the player's power.</returns>
	private GameObject LoadMyPower(){
		if (gameObject.name == PLAYER_1_OBJ){
			GameObject tether = Instantiate(Resources.Load(TETHER_OBJ),
											new Vector3(0.0f, 0.0f, 0.0f),
											Quaternion.identity,
											GameObject.Find(PARTICLE_ORGANIZER).transform) as GameObject;

			tether.SetActive(false);

				
			return tether;
		} else if (gameObject.name == PLAYER_2_OBJ){
			return Resources.Load(LIGHTNING_OBJ) as GameObject;
		} else {
			Debug.Log("Couldn't figure out which player power to load for " + gameObject.name);
			return Resources.Load(TETHER_OBJ) as GameObject;
		}
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
			} else if (Vector3.Distance(transform.position, ball.position) > awesomeCatchDistance){ //if true, pushed button too early--missed catch
				inputs++;
				movementScript.SlowMaxSpeed();
				readyForAwesomeCatch = false;
			} else { //success! An awesome catch can occur
				readyForAwesomeCatch = AwesomeCatch();
			}
		}
	}


	/// <summary>
	/// Chooses the correct awesome catch effect for this player.
	/// </summary>
	/// <returns><c>true</c> so that the awesome catch will happen when the player catches the ball.</returns>
	private bool AwesomeCatch(){
		if (gameObject.name == PLAYER_1_OBJ){
			Player1AwesomeCatchEffect();
		} else {
			Player2AwesomeCatchEffect();
		}

		pixelScript.SetTemporaryResolution(catchResolution, catchResolution, catchResolutionChangeDuration);

		return true;
	}


	/// <summary>
	/// Activate the tether between player 1 and player 2
	/// </summary>
	private void Player1AwesomeCatchEffect(){
		myPower.SetActive(true);
	}


	/// <summary>
	/// Cause a bolt of lightning to appear at player 2's location
	/// </summary>
	private void Player2AwesomeCatchEffect(){
		GameObject burst = ObjectPooling.ObjectPool.GetObj(myPower.name);
		burst.transform.position = transform.position;
	}


	/// <summary>
	/// Called by BallBehavior to get the distance where the awesome catch can happen.
	/// </summary>
	/// <returns>The awesome catch distance.</returns>
	public float GetAwesomeCatchDistance(){
		return awesomeCatchDistance;
	}
}
