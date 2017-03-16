using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {


	private Rigidbody rb;

	public float maxZSpeed = 1.0f; //player maximum speed on forward Z axis
	public float maxXSpeed = 1.0f; //player maximum speed on X axis
	public float maxReverseSpeed = 50.0f; //player maximum speed in -Z direction
	public float zAccel = 0.3f; //amount player accelerates each frame of input on the Z axis
	public float zBrake = 50.0f; //amount player brakes each frame of input on the Z axis
	public float xAccel = 1.0f; //amount player accelerates each frame of input on the X axis
	public float slowSpeed = 0.5f; //player speed when slowed for a missed awesome catch
	public float slowDuration = 1.0f; //how long the player is slowed after a missed awesome catch

	public float currentZMaxSpeed = 0.0f; //the player's maximum speed, either the normal maximum or the slowed maximum
	public float currentXMaxSpeed = 0.0f;
	private float slowTimer = 0.0f; //keeps track of how long the player has been slowed

	private bool stopped = false; //players are stopped, for example, when they destroy an enemy
	public bool Stopped{
		get { return stopped; }
		set { stopped = value; }
	}
	public float stopDuration = 0.25f;
	private float stopTimer = 0.0f;

	//players are locked out of moving when the lightning stream is active; DirectionalLightning controls this
	public bool MovementLocked { get; set; }

	//these match the directions in InputManager
	private const string UP = "up";
	private const string DOWN = "down";
	private const string LEFT = "left";
	private const string RIGHT = "right";


	private void Start(){
		rb = GetComponent<Rigidbody>();
		currentZMaxSpeed = maxZSpeed;
		currentXMaxSpeed = maxXSpeed;
		MovementLocked = false;
	}


	//if currently slowed down, track time until it's appropriate to speed back up
	private void Update(){
		if (currentZMaxSpeed == slowSpeed){
			slowTimer += Time.deltaTime;

			if (slowTimer >= slowDuration){
				currentZMaxSpeed = maxZSpeed;
				currentXMaxSpeed = maxXSpeed;
			}
		}
	}
		

	private void FixedUpdate(){
		//normal movement
		if (!Stopped && !MovementLocked){
			//This is a bodge to limit maximum speed. The better way would be to impose a countervailing force.
			//Directly manipulating rigidbody velocity could lead to physics problems.
			if (rb.velocity.z > currentZMaxSpeed){
				Vector3 vel = rb.velocity;
				vel.z = currentZMaxSpeed;
				rb.velocity = vel;
			} else if (rb.velocity.z < -maxReverseSpeed){
				Vector3 vel = rb.velocity;
				vel.z = -maxReverseSpeed;
				rb.velocity = vel;
			}

			if (rb.velocity.x > currentXMaxSpeed){
				Vector3 vel = rb.velocity;
				vel.x = currentXMaxSpeed;
				rb.velocity = vel;
			} else if (rb.velocity.x < -currentXMaxSpeed){
				Vector3 vel = rb.velocity;
				vel.x = -currentXMaxSpeed;
				rb.velocity = vel;
			}

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
		if (!Stopped && !MovementLocked){ //the player can't move when stopped
			Vector3 temp = new Vector3(0.0f, 0.0f, 0.0f);

			if (direction == UP){
				temp.z = -1.0f;
				rb.AddForce(temp.normalized * zBrake, ForceMode.Force);
			} else if (direction == DOWN){
				temp.z = 1.0f;
				rb.AddForce(temp.normalized * zAccel, ForceMode.Force);
			}

			if (direction == LEFT){
				temp.x = -1.0f;
				rb.AddForce(temp.normalized * xAccel, ForceMode.Force);
			} else if (direction == RIGHT){
				temp.x = 1.0f;
				rb.AddForce(temp.normalized * xAccel, ForceMode.Force);
			}
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
		currentZMaxSpeed = slowSpeed;
		currentXMaxSpeed = slowSpeed;
		slowTimer = 0.0f;
	}
}
