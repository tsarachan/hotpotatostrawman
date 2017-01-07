using UnityEngine;
using System.Collections;

public class PlayerMovementLean : MonoBehaviour {

	//the parent object that will rotate to lean both the cycle and the rider
	private Transform cycleAndRider;
	private const string CYCLE_AND_RIDER_OBJ = "Cycle and rider";

	//the object whose rotation the parent object will slerp to match
	private Transform rotationTarget;
	private const string ROTATION_TARGET_OBJ = "Rotation target";

	//how far the rotation target will turn in different directions. These values are in degrees.
	public float maxForwardLean = 45.0f;
	public float maxBackwardLean = -60.0f;
	public float maxSideLean = 45.0f;

	//possible directions for leaning; these must match the directions in InputManager
	private const string UP = "up";
	private const string DOWN = "down";
	private const string LEFT = "left";
	private const string RIGHT = "right";

	//the rate at which the parent object rotates
	public float rotationSpeed = 10.0f;


	private void Start(){
		cycleAndRider = transform.Find(CYCLE_AND_RIDER_OBJ);
		rotationTarget = transform.Find(ROTATION_TARGET_OBJ);
	}


	public void Lean(string dir){
		Vector3 temp = rotationTarget.eulerAngles;

		switch(dir){
			case UP:
				temp.x = maxForwardLean;
				rotationTarget.rotation = Quaternion.Euler(temp);
				break;
			case DOWN:
				temp.x = maxBackwardLean;
				rotationTarget.rotation = Quaternion.Euler(temp);
				break;
			case LEFT:
				temp.z = maxSideLean;
				rotationTarget.rotation = Quaternion.Euler(temp);
				break;
			case RIGHT:
				temp.z = -maxSideLean;
				rotationTarget.rotation = Quaternion.Euler(temp);
				break;
			default:
				Debug.Log("Illegal direction: " + dir);
				break;
		}
	}


	private void Update(){
		cycleAndRider.rotation = Quaternion.RotateTowards(cycleAndRider.rotation,
														  rotationTarget.rotation,
														  rotationSpeed);

		rotationTarget.rotation = Quaternion.RotateTowards(rotationTarget.rotation,
														   Quaternion.identity,
														   rotationSpeed);
	}
}
