using UnityEngine;
using System.Collections;

public class PlayerBallInteraction : MonoBehaviour {

	//these enable the player to find the ball
	private const string BALL_OBJ = "Ball";
	private BallBehavior ballBehavior;

	//the player this player will throw to
	private const string PLAYER_ORGANIZER = "Players";
	private Transform otherPlayer;

	//is this player currently in possession of the ball?
	private bool ballCarrier = false;
	public bool BallCarrier{
		get { return ballCarrier; }
		set { ballCarrier = value; }
	}

	private void Start(){
		ballBehavior = GameObject.Find(BALL_OBJ).GetComponent<BallBehavior>();
		otherPlayer = GetOtherPlayer();
	}

	/// <summary>
	/// Find the other player's transform, so that this player can pass to them.
	/// </summary>
	/// <returns>The other player's transform.</returns>
	private Transform GetOtherPlayer(){
		Transform temp = transform;

		foreach (Transform player in GameObject.Find(PLAYER_ORGANIZER).transform){
			if (player.name != transform.name){
				temp = player;
			}
		}

		if (temp == transform) { Debug.Log("Couldn't find other player for " + transform.name); }

		return temp;
	}
		

	/// <summary>
	/// Picks up the ball when the player encounters it.
	/// </summary>
	/// <param name="other">The collider the player has encountered.</param>
	private void OnTriggerEnter(Collider other){
		if (other.transform.name.Contains(BALL_OBJ)){
			other.transform.parent = transform;
			other.transform.position = transform.position;
			if (other.transform.GetComponent<BallBehavior>().Co != null) { StopCoroutine(other.transform.GetComponent<BallBehavior>().Co); }
			BallCarrier = true;
		}
	}

	/// <summary>
	/// InputManager calls this to start the process of throwing the ball.
	/// </summary>
	/// <returns><c>false</c> so that this player is no longer the ball carrier.</returns>
	public bool ThrowBall(){
		ballBehavior.Pass(transform, otherPlayer);

		return false;
	}
}
