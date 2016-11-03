using UnityEngine;
using System.Collections;

public class BallBehavior : MonoBehaviour {

	private Rigidbody rb;

	public AnimationCurve normalTossCurve; //can be used to make the ball move faster at start then lose speed over time, etc.
	public float flightTimePerUnitDistance = 0.1f; //effectively speed
	public AnimationCurve verticalCurve; //used to give the ball a "lob" effect
	public float verticalHeight = 2.0f; //height the ball reaches at the apex of the lob

	private const string PLAYER_OBJ = "Player";

	private const string SCENE_ORGANIZER = "Scene";
	private Transform scene;

	private Coroutine co;
	public Coroutine Co{
		get { return co; }
	}

	private void Start(){
		rb = GetComponent<Rigidbody>();
		scene = GameObject.Find(SCENE_ORGANIZER).transform;
	}

	/// <summary>
	/// Players should call this function to pass between players.
	/// </summary>
	/// <param name="start">The throwing player.</param>
	/// <param name="destination">The catching player.</param>
	public void Pass(Transform start, Transform destination){
		transform.parent = scene; //stop being a child of the ball carrier, so that the ball can move between players
		co = StartCoroutine(PassBetweenPlayers(start.position, destination));
	}

	/// <summary>
	/// This coroutine moves the ball between the players.
	/// </summary>
	/// <param name="start">The throwing player's location.</param>
	/// <param name="destination">The catching player.</param>
	public IEnumerator PassBetweenPlayers(Vector3 start, Transform destination){
		float totalFlightTime = Vector3.Distance(start, destination.position)/flightTimePerUnitDistance;
		float timer = 0.0f;

		while (timer <= totalFlightTime){
			//update totalFlightTime every frame, so that the ball moves faster or slower depending on the distance
			//this should also avoid a bug wherein the ball gets stuck at a high elevation
			totalFlightTime = Vector3.Distance(start, destination.position)/flightTimePerUnitDistance;
			timer += Time.deltaTime;

			Vector3 nextPoint = Vector3.Lerp(start,
											 destination.position,
											 normalTossCurve.Evaluate(timer/totalFlightTime));

			nextPoint.y = Mathf.Lerp(0.0f, verticalHeight, verticalCurve.Evaluate(timer/totalFlightTime));

			rb.MovePosition(nextPoint);

			yield return null;
		}

		Debug.Log("Coroutine stopped: y == " + transform.position.y);
		yield break;
	}


	public void BallCaught(Transform catchingPlayer){
		StopCoroutine(Co);

	}
}
