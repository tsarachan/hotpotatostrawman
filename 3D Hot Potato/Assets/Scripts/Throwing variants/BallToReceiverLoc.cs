/*
 * 
 * This variation of BallBehavior causes the ball to fly to the receiver's location at the moment of the throw.
 * 
 */

using UnityEngine;
using System.Collections;

public class BallToReceiverLoc : BallBehavior {

	/// <summary>
	/// Players should call this function to pass between players.
	/// </summary>
	/// <param name="start">The throwing player.</param>
	/// <param name="destination">The catching player.</param>
	public override void Pass(Transform start, Transform destination){
		transform.parent = scene; //stop being a child of the ball carrier, so that the ball can move between players
		co = StartCoroutine(PassToPoint(start.position, destination.position));
	}

	/// <summary>
	/// This coroutine moves the ball to a specific location.
	/// </summary>
	/// <param name="start">The throwing player's location.</param>
	/// <param name="destination">The catching player's location at the moment of the throw.</param>
	public IEnumerator PassToPoint(Vector3 start, Vector3 end){
		float totalFlightTime = Vector3.Distance(start, end)/flightTimePerUnitDistance;
		float timer = 0.0f;

		while (timer <= totalFlightTime){
			timer += Time.deltaTime;

			Vector3 nextPoint = Vector3.Lerp(start,
											 end,
											 normalTossCurve.Evaluate(timer/totalFlightTime));

			nextPoint.y = Mathf.Lerp(0.0f, verticalHeight, verticalCurve.Evaluate(timer/totalFlightTime));

			rb.MovePosition(nextPoint);

			yield return null;
		}

		FallToGround(end);

		//Debug.Log("Coroutine stopped: y == " + transform.position.y);
		yield break;
	}
}
