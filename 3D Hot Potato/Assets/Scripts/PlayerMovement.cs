/*
 * 
 * This script moves the player around.
 * 
 * It assumes that the player has a rigidbody attached, and is moving with physics.
 * 
 */
using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {


	private Rigidbody rb;

	public float maxSpeed = 1.0f; //player maximum speed
	public float speed = 0.3f; //amount player accelerates each frame of input

	private bool stopped = false; //players are stopped, for example, when they destroy an enemy
	public bool Stopped{
		get { return stopped; }
		set { stopped = value; }
	}
	public float stopDuration = 0.25f; //how long the player stops
	private float stopTimer = 0.0f;

	//these match the directions in InputManager
	private const string UP = "up";
	private const string DOWN = "down";
	private const string LEFT = "left";
	private const string RIGHT = "right";

	private void Start(){
		rb = GetComponent<Rigidbody>();
	}

	private void FixedUpdate(){
		if (!Stopped){
			//This is a bodge to limit maximum speed. The better way would be to impose a countervailing force.
			//Directly manipulating rigidbody velocity could lead to physics problems.
			if (rb.velocity.magnitude > maxSpeed) { rb.velocity = rb.velocity.normalized * maxSpeed; }
		} else if (Stopped) {
			rb.velocity = Vector3.zero;
			Stopped = RunStopTimer();
		}
	}


	/// <summary>
	/// InputManager calls this function to cause the player to move.
	/// </summary>
	/// <param name="direction">The direction in which the player should move.</param>
	public void Move(string direction){
		if (!Stopped){ //the player can't move when stopped
			Vector3 temp = new Vector3(0.0f, 0.0f, 0.0f);

			if (direction == UP){
				temp.z = -1.0f;
			} else if (direction == DOWN){
				temp.z = 1.0f;
			}

			if (direction == LEFT){
				temp.x = -1.0f;
			} else if (direction == RIGHT){
				temp.x = 1.0f;
			}

			rb.AddForce(temp.normalized * speed, ForceMode.Force);
		}
	}


	/// <summary>
	/// If the player is stopped, this function runs the timer until the player is able to move again.
	/// </summary>
	/// <returns><c>false</c> if the player can move, <c>false</c> otherwise.</returns>
	private bool RunStopTimer(){
		stopTimer += Time.deltaTime;

		if (stopTimer >= stopDuration){
			return false;
		} else {
			return true;
		}
	}
}
