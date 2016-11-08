using UnityEngine;
using System.Collections;

public class BallAheadOfReceiver : BallBehavior {

	//flight time is set as a fixed number here because getting the real flight time involves some tough trigonometry
	public float flightTime = 1.0f;

	//if the player is within this distance, they pull the ball in
	//this helps deal with the somewhat inaccurate math 
	public float snapToPlayerDist = 1.0f;

	//the moving player's inputs will be used to determine what direction they're moving in
	private const string VERT_AXIS = "PS4_LStick_Vert_";
	private const string HORIZ_AXIS = "PS4_LStick_Horiz_";
	public float inputDeadZone = 0.3f;

	//these are used to make sure the ball isn't thrown out-of-bounds
	private const string TOP_BOUNDARY = "Front";
	private const string BOTTOM_BOUNDARY = "Back";
	private const string LEFT_BOUNDARY = "Left";
	private const string RIGHT_BOUNDARY = "Right";
	private float topLimit = 0.0f;
	private float bottomLimit = 0.0f;
	private float leftLimit = 0.0f;
	private float rightLimit = 0.0f;

	protected override void Start(){
		base.Start();

		topLimit = GameObject.Find(TOP_BOUNDARY).transform.position.z;
		bottomLimit = GameObject.Find(BOTTOM_BOUNDARY).transform.position.z;
		leftLimit = GameObject.Find(LEFT_BOUNDARY).transform.position.x;
		rightLimit = GameObject.Find(RIGHT_BOUNDARY).transform.position.x;
	}

	/// <summary>
	/// Players should call this function to pass between players.
	/// </summary>
	/// <param name="start">The throwing player.</param>
	/// <param name="destination">The catching player.</param>
	public override void Pass(Transform start, Transform destination){
		transform.parent = scene; //stop being a child of the ball carrier, so that the ball can move between players
		co = StartCoroutine(PassToPoint(start.position, FindPointAhead(destination), destination));
	}

	/// <summary>
	/// This coroutine moves the ball to a specific location.
	/// </summary>
	/// <param name="start">The throwing player's location.</param>
	/// <param name="destination">The catching player's location at the moment of the throw.</param>
	/// <param name="intendedReceiver">The catching player.</param>
	public IEnumerator PassToPoint(Vector3 start, Vector3 end, Transform intendedReceiver){
		//float totalFlightTime = Vector3.Distance(start, end)/flightTimePerUnitDistance;
		float timer = 0.0f;

		while (timer <= flightTime){
			timer += Time.deltaTime;

			Vector3 nextPoint = Vector3.Lerp(start,
				end,
				normalTossCurve.Evaluate(timer/flightTime));

			nextPoint.y = Mathf.Lerp(0.0f, verticalHeight, verticalCurve.Evaluate(timer/flightTime));

			rb.MovePosition(nextPoint);

			yield return null;
		}

		FallToGround(end);
		TryToGetPickedUp(intendedReceiver);

		//Debug.Log("Coroutine stopped: y == " + transform.position.y);
		yield break;
	}


	/// <summary>
	/// Finds where the player being thrown to is estimated to be at the end of a throw, given current speed and direction.
	/// </summary>
	/// <returns>The estimated location.</returns>
	/// <param name="player">The player meant to catch the ball.</param>
	protected Vector3 FindPointAhead(Transform player){
		Vector3 currentPosition = player.position;
		float playerVelocity = player.GetComponent<Rigidbody>().velocity.magnitude;


		//figure out which direction the catching player is currently moving
		char playerNum = player.name[7]; //assumes players are named using the convention "Player #"
		Vector3 playerDirection = new Vector3(0.0f, 0.0f, 0.0f);

		if (Input.GetAxis(VERT_AXIS + playerNum) < -inputDeadZone){
			playerDirection.z = 1.0f;
		} else if (Input.GetAxis(VERT_AXIS + playerNum) > inputDeadZone){
			playerDirection.z = -1.0f;
		}

		if (Input.GetAxis(HORIZ_AXIS + playerNum) > inputDeadZone){
			playerDirection.x = 1.0f;
		} else if (Input.GetAxis(HORIZ_AXIS + playerNum) < -inputDeadZone){
			playerDirection.x = -1.0f;
		}

		//this is a bodge. It doesn't take the catching player's movement into account, so it merely gives a short flight time when close, 
		//and a longer one when far away. Players who are far away and moving away will get a shorter flight time than they should, 
		//and those who are moving closer will get an unduly long one.
//		Debug.Log("playerDirection == " + playerDirection);
//		Debug.Log("playerVelocity == " + playerVelocity);
//		Debug.Log("estimated location == " + playerDirection * playerVelocity * flightTime);
		Vector3 estimatedPoint = currentPosition + (playerDirection * playerVelocity * flightTime);

		//limit the location where the ball can land to in-bounds space
		if (estimatedPoint.z > topLimit){
			estimatedPoint.z = topLimit - transform.localScale.z;
		} else if (estimatedPoint.z < bottomLimit){
			estimatedPoint.z = bottomLimit + transform.localScale.z;
		}

		if (estimatedPoint.x < leftLimit){
			estimatedPoint.x = leftLimit + transform.localScale.x;
		} else if (estimatedPoint.x > rightLimit){
			estimatedPoint.x = rightLimit - transform.localScale.x;
		}

		return estimatedPoint;
	}

	/// <summary>
	/// If the player who was supposed to get the ball is close by, snap it to them as though they'd caught it.
	/// This is a brute-force fix to help players deal with the somewhat inaccurate predictive math.
	/// </summary>
	/// <param name="player">The player who's supposed to get the ball.</param>
	private void TryToGetPickedUp(Transform player){
		if (Vector3.Distance(transform.position, player.position) <= snapToPlayerDist){
			player.GetComponent<PlayerBallThrowDelay>().CatchBall(transform);
		}
	}
}
