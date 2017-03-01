using System.Collections;
using UnityEngine;

public class DirectionalLightning : MonoBehaviour {

	//----------tunable variables----------

	public float rotationSpeed = 20.0f; //speed at which the lightning stream spins around the player
	public float duration = 3.0f; //how long the lightning stream continues


	//----------internal variables----------


	//used to track how long the lightning stream should last
	private float timer = 0.0f;


	//these help direct the lightning stream
	private const string ATTRACTOR_OBJ = "Attractor";
	private Transform attractor;
	private const string INPUT_CONTROLLED_OBJ = "Input marker";
	private Transform inputMarker;


	//these allow this script to get inputs from InputManager; they must match the values there
	private const string UP = "up";
	private const string DOWN = "down";
	private const string LEFT = "left";
	private const string RIGHT = "right";


	//initialize variables
	private void Start(){
		attractor = transform.Find(ATTRACTOR_OBJ);
		inputMarker = transform.Find(INPUT_CONTROLLED_OBJ);
		gameObject.SetActive(false);
	}


	//move the attractor (which in turn directs the lightning stream), and shut the stream off when time is up
	private void Update(){
		attractor.rotation = RotateTowardInputDirection();

		timer += Time.deltaTime;

		if (timer >= duration){
			timer = 0.0f;
			transform.parent.GetComponent<PlayerMovement>().MovementLocked = false;
			gameObject.SetActive(false);
		}
	}


	private Quaternion RotateTowardInputDirection(){
		Vector3 newRotation = Vector3.RotateTowards(attractor.forward,
													inputMarker.forward,
													rotationSpeed * Time.deltaTime,
													0.0f);

		return Quaternion.Euler(newRotation);
	}


	public void DirectRotation(string dir){
		switch (dir){
			case UP:
				inputMarker.rotation = Quaternion.LookRotation(Vector3.forward);
				break;
			case DOWN:
				inputMarker.rotation = Quaternion.LookRotation(-Vector3.forward);
				break;
			case LEFT:
				inputMarker.rotation = Quaternion.LookRotation(-Vector3.right);
				break;
			case RIGHT:
				inputMarker.rotation = Quaternion.LookRotation(Vector3.right);
				break;
			default:
				Debug.Log("Illegal direction: " + dir);
				break;
		}
	}


	//establish the lightning stream's initial state, and then switch it on
	public void Activate(){
		timer = 0.0f;
		inputMarker.rotation = Quaternion.LookRotation(Vector3.forward);
		transform.parent.GetComponent<PlayerMovement>().MovementLocked = true;
		gameObject.SetActive(true);
	}
}
