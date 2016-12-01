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
		if (transform.Find(BALL_OBJ)){ //sanity check to make sure this player has the ball; avoids null references after losing
			ballBehavior.Pass(transform.Find(BALL_OBJ).position, otherPlayer);
			BallCarrier = false;
		}
	}
}
