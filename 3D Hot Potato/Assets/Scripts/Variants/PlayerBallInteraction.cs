using UnityEngine;
using System.Collections;

public class PlayerBallInteraction : MonoBehaviour {

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

	public float verticalOffset = 2.0f;

	private void Start(){
		ballBehavior = GameObject.Find(BALL_OBJ).GetComponent<BallBehavior>();
		playerNum = transform.name[7]; //assumes players are named using the convention "Player #"
		myOButton = O_BUTTON + playerNum;
		otherPlayer = GetOtherPlayer();
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
		if (Input.GetButtonDown(myOButton) && BallCarrier || Input.GetKeyDown(pass) && BallCarrier){
			BallCarrier = ThrowBall();
		}
	}

	/// <summary>
	/// Picks up the ball when the player encounters it.
	/// </summary>
	/// <param name="other">The collider the player has encountered.</param>
	private void OnTriggerEnter(Collider other){
		if (other.transform.name.Contains(BALL_OBJ)){
			other.transform.parent = transform;
			other.transform.localPosition = new Vector3(0.0f, 0.0f, verticalOffset); //the player models are turned, so z is the vertical axis
			if (other.transform.GetComponent<BallBehavior>().Co != null) { StopCoroutine(other.transform.GetComponent<BallBehavior>().Co); }
			BallCarrier = true;
		}
	}

	/// <summary>
	/// Call this to start the process of throwing the ball
	/// </summary>
	/// <returns><c>false</c> so that this player is no longer the ball carrier.</returns>
	private bool ThrowBall(){
		ballBehavior.Pass(transform.Find(BALL_OBJ).position, otherPlayer);

		return false;
	}
}
