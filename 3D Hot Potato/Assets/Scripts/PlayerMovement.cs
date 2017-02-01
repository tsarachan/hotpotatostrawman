using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {


	private Rigidbody rb;

	public float maxSpeed = 1.0f; //player maximum speed
	public float accel = 0.3f; //amount player accelerates each frame of input
	public float slowSpeed = 0.5f; //player speed when slowed for a missed awesome catch
	public float slowDuration = 1.0f; //how long the player is slowed after a missed awesome catch

	public float currentMaxSpeed = 0.0f; //the player's maximum speed, either the normal maximum or the slowed maximum
	private float slowTimer = 0.0f; //keeps track of how long the player has been slowed

	private bool stopped = false; //players are stopped, for example, when they destroy an enemy
	public bool Stopped{
		get { return stopped; }
		set { stopped = value; }
	}
	public float stopDuration = 0.25f;
	private float stopTimer = 0.0f;

	//these match the directions in InputManager
	private const string UP = "up";
	private const string DOWN = "down";
	private const string LEFT = "left";
	private const string RIGHT = "right";


	private void Start(){
		rb = GetComponent<Rigidbody>();
		currentMaxSpeed = maxSpeed;
	}


	//if currently slowed down, track time until it's appropriate to speed back up
	private void Update(){
		if (currentMaxSpeed == slowSpeed){
			slowTimer += Time.deltaTime;

			if (slowTimer >= slowDuration){
				currentMaxSpeed = maxSpeed;
			}
		}
	}
		

	private void FixedUpdate(){

		//normal movement
		if (!Stopped){
			//This is a bodge to limit maximum speed. The better way would be to impose a countervailing force.
			//Directly manipulating rigidbody velocity could lead to physics problems.
			if (rb.velocity.magnitude > currentMaxSpeed) { rb.velocity = rb.velocity.normalized * currentMaxSpeed; }

		//not moving because movement paused while fighting an enemy
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

			rb.AddForce(temp.normalized * accel, ForceMode.Force);
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


	public void SlowMaxSpeed(){
		Debug.Log("SlowMaxSpeed() called");
		currentMaxSpeed = slowSpeed;
		slowTimer = 0.0f;
	}
}
