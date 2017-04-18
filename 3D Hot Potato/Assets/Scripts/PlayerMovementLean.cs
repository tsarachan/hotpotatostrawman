/*
 * 
 * This script rotates the player's cycle and rider according to the current input (or lack thereof).
 * 
 */
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
	public float maxBackwardLean = 60.0f; //this must be a positive number!
	public float maxSideLean = 45.0f;
	public float maxTurnLean = 25.0f; //how far the lightcycle will turn into a lateral movement

	//possible directions for leaning; these must match the directions in InputManager
	private const string UP = "up";
	private const string DOWN = "down";
	private const string LEFT = "left";
	private const string RIGHT = "right";

	//the rate at which the parent object rotates
	public float rotationSpeed = 10.0f; //speed of rotating into a turn
	public float returnSpeed = 280.0f; //speed of rotating back to straight
	public float backRotateSpeed = 500.0f; //speed of rotating into the braking move for backwards movement


	//the rotation target is aligned differently when the players are jumping
	public bool Jumping { get; set; }


	//initialize variables
	private void Start(){
		cycleAndRider = transform.Find(CYCLE_AND_RIDER_OBJ);
		rotationTarget = transform.Find(ROTATION_TARGET_OBJ);
	}


	/// <summary>
	/// InputManager calls this to set the target rotation the cycle and rider will turn towards.
	/// 
	/// Note that case DOWN (moving backwards) is substantially different, owing to the 
	/// </summary>
	/// <param name="dir">The direction of movement, provided by InputManager.</param>
	public void Lean(string dir){
		if (!Jumping){ //the cycle always leans backwards when jumping
			Vector3 temp = rotationTarget.eulerAngles;

			switch(dir){
				//note that up and down are reversed in the input manager, and thus here as well
				//that is to say, case DOWN is forward movement, case UP is backward movement
				case UP:
					temp.y = 90.0f;
					temp.z = -maxSideLean;
					rotationTarget.rotation = Quaternion.Euler(temp);
					break;
				case DOWN:
					temp.x = maxForwardLean;
					rotationTarget.rotation = Quaternion.Euler(temp);
					break;
				case LEFT:
					temp.z = maxSideLean;
					temp.y = -maxTurnLean;
					rotationTarget.rotation = Quaternion.Euler(temp);
					break;
				case RIGHT:
					temp.z = -maxSideLean;
					temp.y = maxTurnLean;
					rotationTarget.rotation = Quaternion.Euler(temp);
					break;
				default:
					Debug.Log("Illegal direction: " + dir);
					break;
			}
		}
	}


	/// <summary>
	/// Rotate the cycle and rider toward the target, and then pull the rotation target back to no rotation.
	/// 
	/// In each frame of continuous movement, the target will always be set to the intended rotation by InputManager,
	/// then this function will rotate the player toward the target and reset the target. The reset is harmless,
	/// since InputManager will put it back where it's supposed to be before this turns the player.
	/// </summary>
	private void LateUpdate(){
		//determine how fast to rotate based on the rotation target
		float rotateSpeed;

		if (rotationTarget.rotation == Quaternion.identity){
			//Debug.Log("setting rotateSpeed to returnSpeed; returnSpeed == " + returnSpeed);
			rotateSpeed = returnSpeed;
		} else if (rotationTarget.rotation == Quaternion.Euler(new Vector3(0.0f, 90.0f, -maxSideLean))){
			//Debug.Log("rotating backwards");
			rotateSpeed = backRotateSpeed;
		} else {
			//Debug.Log("setting rotateSpeed to rotationSpeed; rotationSpeed == " + rotationSpeed);
			rotateSpeed = rotationSpeed;
		}

		//Debug.Log(rotateSpeed);

		cycleAndRider.rotation = Quaternion.RotateTowards(cycleAndRider.rotation,
														  rotationTarget.rotation,
														  rotateSpeed * Time.deltaTime);

		if (!Jumping){
			rotationTarget.rotation = Quaternion.identity;
		}
	}


	public void StartJumpRise(float angle){
		Debug.Log("StartJumpRise() called for " + gameObject.name);
		Jumping = true;

		rotationTarget.rotation = Quaternion.Euler(new Vector3(angle, 0.0f, 0.0f));
	}


	public void StartJumpFall(float angle){
		Debug.Log("StartJumpFall() called for " + gameObject.name);
		rotationTarget.rotation = Quaternion.Euler(new Vector3(angle, 0.0f, 0.0f));
	}


	public void DoneJumping(){
		Debug.Log("DoneJumping() called for " + gameObject.name);
		Jumping = false;
	}
}
