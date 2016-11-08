/*
 * 
 * In this script, the player must hold down the circle button for a certain amount of time before she can throw the ball.
 * 
 */

using UnityEngine;
using System.Collections;

public class PlayerBallThrowDelay : MonoBehaviour {

	private const string BALL_OBJ = "Ball";
	private BallBehavior ballBehavior;

	private const string O_BUTTON = "PS4_O_";
	private char playerNum = '0';
	private string myOButton = "";
	public KeyCode pass; //debug control

	private const string PLAYER_ORGANIZER = "Players";
	private Transform otherPlayer;

	private bool ballCarrier = false;
	public bool BallCarrier{
		get { return ballCarrier; }
		set { ballCarrier = value; }
	}
	private bool readyToThrow = false;
	public float chargeTime = 1.0f; //how long the player needs to hold down the circle button before being able to throw
	private float currentCharge = 0.0f; //measures how long the player has held the circle button so far while charging up to throw
	private Light myPointLight; //the player's point light will indicate how charged the player is
	public float maxIntensity = 8.0f; //the player's point light's intensity when the fully charged to throw
	public float normalIntensity = 1.0f; //the point light's intensity when the player doesn't have the ball

	private void Start(){
		ballBehavior = GameObject.Find(BALL_OBJ).GetComponent<BallBehavior>();
		playerNum = transform.name[7]; //assumes players are named using the convention "Player #"
		myOButton = O_BUTTON + playerNum;
		otherPlayer = GetOtherPlayer();
		myPointLight = transform.GetChild(0).GetComponent<Light>();
	}


	/// <summary>
	/// Find the other player's transform, so that this player can pass to them.
	/// </summary>
	/// <returns>The other player's transform.</returns>
	private Transform GetOtherPlayer(){
		Transform temp = transform;

		foreach (Transform player in transform.root.Find(PLAYER_ORGANIZER)){
			if (player.name != transform.name){
				temp = player;
			}
		}

		if (temp == transform) { Debug.Log("Couldn't find other player for " + transform.name); }

		return temp;
	}


	private void Update(){
		//when not yet ready to throw, get ready to throw
		if (Input.GetButton(myOButton) && BallCarrier && !readyToThrow ||
			Input.GetKeyDown(pass) && BallCarrier && !readyToThrow){
			readyToThrow = ChargeUp();
		}

		//if ready to throw, toss the button on command
		if (Input.GetButtonUp(myOButton) && BallCarrier && readyToThrow ||
			Input.GetKeyDown(pass) && BallCarrier && readyToThrow){
			BallCarrier = ThrowBall();
			readyToThrow = false;
			myPointLight.intensity = normalIntensity;
		}
	}


	/// <summary>
	/// Picks up the ball when the player encounters it.
	/// </summary>
	/// <param name="other">The collider the player has encountered.</param>
	private void OnTriggerEnter(Collider other){
		if (other.transform.name.Contains(BALL_OBJ)){
			if (other.transform.GetComponent<BallBehavior>().Co != null) { StopCoroutine(other.transform.GetComponent<BallBehavior>().Co); }
			CatchBall(other.transform);
			//Debug.Log("Coroutine stopped by player catch; y == " + transform.position.y);
		}
	}

	public void CatchBall(Transform ball){
		ball.parent = transform;
		ball.position = transform.position;
		BallCarrier = true;
		myPointLight.intensity = 0.0f;
	}


	/// <summary>
	/// Call this to start the process of throwing the ball
	/// </summary>
	/// <returns><c>false</c> so that this player is no longer the ball carrier.</returns>
	private bool ThrowBall(){
		ballBehavior.Pass(transform, otherPlayer);
		currentCharge = 0.0f;
		return false;
	}


	/// <summary>
	/// This keeps track of how charged the player is when the player is getting ready to throw. It increases currentCharge, and also
	/// increases the intensity of the player's point light to provide feedback.
	/// </summary>
	/// <returns><c>true</c>, if the player is fully charged to throw, <c>false</c> if not.</returns>
	private bool ChargeUp(){
		currentCharge += Time.deltaTime;
		myPointLight.intensity = (currentCharge/chargeTime) * maxIntensity; //make the player's light brighter as the player charges up

		if (currentCharge >= chargeTime){
			return true; //all charged up
		} else {
			return false; //not charged up yet
		}
	}
}