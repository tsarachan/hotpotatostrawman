/*
 * 
 * This script handles the ball's movement betwen players
 * 
 */

using UnityEngine;
using System.Collections;

public class BallBehavior : MonoBehaviour {

	protected Rigidbody rb;

	public AnimationCurve normalTossCurve; //can be used to make the ball move faster at start then lose speed over time, etc.
	public float flightTimePerUnitDistance = 0.1f; //effectively speed
	public AnimationCurve verticalCurve; //used to give the ball a "lob" effect
	public float verticalHeight = 2.0f; //height the ball reaches at the apex of the lob

	protected const string PLAYER_OBJ = "Player";

	protected const string SCENE_ORGANIZER = "Scene";
	protected Transform scene;

	//this is used to stop the coroutine that moves the ball between players when a player "catches" a pass
	protected Coroutine co;
	public Coroutine Co{
		get { return co; }
	}

	protected virtual void Start(){
		rb = GetComponent<Rigidbody>();
		scene = GameObject.Find(SCENE_ORGANIZER).transform;
	}


	/// <summary>
	/// Players should call this function to pass between players.
	/// </summary>
	/// <param name="start">The throwing player.</param>
	/// <param name="destination">The catching player.</param>
	public virtual void GetThrown(Transform start, Transform destination){
		transform.parent = scene; //stop being a child of the ball carrier, so that the ball can move between players
		co = StartCoroutine(PassBetweenPlayers(start.position, destination));
	}


	/// <summary>
	/// This coroutine moves the ball between the players.
	/// </summary>
	/// <param name="start">The throwing player's location.</param>
	/// <param name="destination">The catching player.</param>
	public virtual IEnumerator PassBetweenPlayers(Vector3 start, Transform destination){
		float totalFlightTime = Vector3.Distance(start, destination.position)/flightTimePerUnitDistance;
		float timer = 0.0f;

		while (timer <= totalFlightTime){
			timer += Time.deltaTime;

			Vector3 nextPoint = Vector3.Lerp(start,
											 destination.position,
											 normalTossCurve.Evaluate(timer/totalFlightTime));

			nextPoint.y = Mathf.Lerp(0.0f, verticalHeight, verticalCurve.Evaluate(timer/totalFlightTime));

			rb.MovePosition(nextPoint);

			yield return null;
		}

		GetCaught(destination); //if the ball somehow doesn't reach its destination, force it to go there

		yield break;
	}


	/// <summary>
	/// Whenever a pass is done, call this. If a player catches the ball, the player should call this;
	/// if the ball reaches the end of a pass without being caught, call this with the intended recipient
	/// to ensure a valid game state.
	/// </summary>
	/// <param name="catchingPlayer">Catching player.</param>
	public void GetCaught(Transform catchingPlayer){
		StopCoroutine(Co);
		transform.parent = catchingPlayer;
		transform.position = catchingPlayer.position;
	}
}
