/*
 * 
 * This script leans the player into movement.
 * 
 * The general structure is: if this script is receiving input on an axis (forward-back or left-right), it leans
 * the cycle and rider in that direction up to a maximum angle. If there is no input on an axis, the cycle and rider
 * return to zero. When there is no input for a while, this script continues to return the cycle and rider to zero
 * each frame.
 * 
 * InputManager calls Lean(), below, to make the cycle and rider lean in a given direction.
 * 
 */
using UnityEngine;
using System.Collections;

public class PlayerMovementLean : MonoBehaviour {

	private Transform cycleAndRider; //the parent object for the cycle and rider, so that both can be rotated at once
	private const string CYCLE_AND_RIDER_OBJ = "Cycle and rider";

	//InputManager uses these to change the current direction; these must match those in InputManager
	private const string UP = "up";
	private const string DOWN = "down";
	private const string LEFT = "left";
	private const string RIGHT = "right";

	//these are used as starting and ending points for the lerps as the player changes direction
	private float startXAngle = 0.0f;
	private float startZAngle = 0.0f;
	private float endXAngle = 0.0f;
	private float endZAngle = 0.0f;

	//how far will the cycle and rider lean? These are in degrees.
	public float maxZAngle = 30.0f;
	public float maxForwardAngle = 15.0f;
	public float maxBackwardAngle = -15.0f;
	private const float ZERO_ANGLE = 0.0f;

	//how long does it take the cycle and rider to lean a single degree? In seconds.
	public float rotateSpeed = 0.05f;

	//did this script receive an input on each axis this frame? Used to decide whether to go back to neutral
	private bool receivedXInput = false;
	private bool receivedZInput = false;


	//initialize variables
	private void Start(){
		cycleAndRider = transform.Find(CYCLE_AND_RIDER_OBJ);
	}

	/// <summary>
	/// Impart a lean to the cycle and rider based on the direction of input. Also note that an input was 
	/// received for that axis, so that the cycle and rider don't try to return to neutral on that axis during
	/// this frame.
	/// </summary>
	/// <param name="dir">The direction of the input.</param>
	public void Lean(string dir){
		//Debug.Log("Lean() called");
		Vector3 temp = cycleAndRider.eulerAngles;
		//Debug.Log("eulerAngles == " + cycleAndRider.eulerAngles);

		switch (dir){
			case DOWN:
				Debug.Log("going up");
				Debug.Log(cycleAndRider.eulerAngles.x);
			if ((cycleAndRider.eulerAngles.x < maxForwardAngle && cycleAndRider.eulerAngles.x >= 0.0f) ||
				(cycleAndRider.eulerAngles.x > 360.0f - maxForwardAngle && cycleAndRider.eulerAngles.x <= 360.0f)) {
					Debug.Log("in correct if statement");
					temp.x -= rotateSpeed;
					Debug.Log("temp.x == " + temp.x);
					cycleAndRider.rotation = Quaternion.Euler(temp);
					receivedXInput = true;
				}
				break;
			case UP:
				if (cycleAndRider.eulerAngles.x > maxBackwardAngle){
					temp.x += rotateSpeed * Time.deltaTime;
					cycleAndRider.rotation = Quaternion.Euler(temp);
					receivedXInput = true;
				}
				break;
			case LEFT:
				if (cycleAndRider.eulerAngles.z > -maxZAngle){
					temp.z -= rotateSpeed * Time.deltaTime;
					cycleAndRider.rotation = Quaternion.Euler(temp);
					receivedZInput = true;
				}
				break;
			case RIGHT:
				if (cycleAndRider.eulerAngles.z < maxZAngle){
					temp.z += rotateSpeed * Time.deltaTime;
					cycleAndRider.rotation = Quaternion.Euler(temp);
					receivedZInput = true;
				}
				break;
		}
		//Debug.Log("Lean() called; receivedXInput == " + receivedXInput);
	}


	/// <summary>
	/// If no input was received for an axis, go back toward no rotation on that axis. Then
	/// assume that no input will be received next frame; this assumption will be overridden by Lean()
	/// if an input is received.
	/// 
	/// This is all done in LateUpdate() so that inputs will have a chance to arrive.
	/// </summary>
	private void LateUpdate(){
		if (gameObject.name == "Player 1"){
			//Debug.Log("LateUpdate() called; receivedXInput == " + receivedXInput);
			Vector3 temp = cycleAndRider.eulerAngles;

			if (!receivedXInput){
				temp.x = ReturnToZero(cycleAndRider.eulerAngles.x);
				//Debug.Log("returning to zero");
			}

			if (!receivedZInput){
				temp.z = ReturnToZero(cycleAndRider.eulerAngles.z);
				//Debug.Log("returning to zero");
			}

			//cycleAndRider.rotation = Quaternion.Euler(temp);

			receivedXInput = false;
			receivedZInput = false;
			//Debug.Log("LateUpdate() finished; receivedXInput == " + receivedXInput);
		}
	}


	/// <summary>
	/// Used to lean the cycle and rider back toward zero on a given axis.
	/// </summary>
	/// <returns>The new angle, in degrees.</returns>
	/// <param name="currentAngle">The angle, in degrees, the cycle and rider are currently at.</param>
	private float ReturnToZero(float currentAngle){

		//check to see if the current angle is essentially zero; if so, return zero
		//this is necessary to avoid dividing by zero
		if (Mathf.Abs(currentAngle) <= Mathf.Epsilon) { return 0.0f; }
		else {
			return currentAngle + (rotateSpeed * Time.deltaTime * -currentAngle/Mathf.Abs(currentAngle));
		}
	}
}
