using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

	//all of these are used to get the thumbstick axes
	private const string LSTICK_HORIZ = "PS4_LStick_Horiz_";
	private const string LSTICK_VERT = "PS4_LStick_Vert_";
	private char playerNum = '0';
	private string myHorizAxis = ""; //this player's horizontal left thumbstick axis
	private string myVertAxis = ""; //this player's vertical left thumbstick axis

	private Rigidbody rb;

	public float maxSpeed = 1.0f; //player maximum speed
	public float speed = 0.3f; //amount player accelerates each frame of input

	private bool stopped = false; //players are stopped, for example, when they destroy an enemy
	public bool Stopped{
		get { return stopped; }
		set { stopped = value; }
	}
	public float stopDuration = 0.25f;
	private float stopTimer = 0.0f;

	private void Start(){
		playerNum = transform.name[7]; //assumes players are named using the convention "Player #"
		myHorizAxis = LSTICK_HORIZ + playerNum;
		myVertAxis = LSTICK_VERT + playerNum;
		rb = GetComponent<Rigidbody>();
	}

	private void FixedUpdate(){
		if (!Stopped){
			rb.AddForce(GetDirection() * speed, ForceMode.Force);

			//This is a bodge to limit maximum speed. The better way would be to impose a countervailing force.
			//Directly manipulating rigidbody velocity could lead to physics problems.
			if (rb.velocity.magnitude > maxSpeed) { rb.velocity = rb.velocity.normalized * maxSpeed; }
		} else if (Stopped) {
			rb.velocity = Vector3.zero;
			Stopped = RunStopTimer();
		}
	}

	private Vector3 GetDirection(){
		Vector3 direction = new Vector3(0.0f, 0.0f, 0.0f);

		if (Input.GetAxis(myHorizAxis) < -0.3f){
			direction.x = -1.0f;
		} else if (Input.GetAxis(myHorizAxis) > 0.3f){
			direction.x = 1.0f;
		}

		if (Input.GetAxis(myVertAxis) > 0.3f){
			direction.z = -1.0f;
		} else if (Input.GetAxis(myVertAxis) < -0.3f){
			direction.z = 1.0f;
		}

		return direction.normalized;
	}

	private bool RunStopTimer(){
		stopTimer += Time.deltaTime;

		if (stopTimer >= stopDuration){
			return false;
		} else {
			return true;
		}
	}
}
