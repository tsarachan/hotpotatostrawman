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
	public float acceleration = 0.3f; //amount player accelerates each frame of input

	private void Start(){
		playerNum = transform.name[7]; //assumes players are named using the convention "Player #"
		myHorizAxis = LSTICK_HORIZ + playerNum;
		myVertAxis = LSTICK_VERT + playerNum;
		rb = GetComponent<Rigidbody>();
	}

	private void FixedUpdate(){
		rb.AddForce(Vector3.ClampMagnitude(Move() * acceleration, maxSpeed), ForceMode.Impulse);
	}

	private Vector3 Move(){
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
}
