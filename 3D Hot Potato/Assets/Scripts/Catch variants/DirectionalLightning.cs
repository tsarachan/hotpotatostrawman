using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalLightning : MonoBehaviour {

	//----------tunable variables----------

	public float rotationSpeed = 20.0f; //speed at which the lightning stream spins around the player
	public float duration = 3.0f; //how long the lightning stream continues
	public float radius = 1.0f; //the radius of the spherecast that destroys enemies


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


	//layermask so that the stream can detect and destroy enemies (and only enemies)
	private int enemyLayer = 9;
	private int enemyLayerMask;


	//initialize variables
	private void Start(){
		attractor = transform.Find(ATTRACTOR_OBJ);
		inputMarker = transform.Find(INPUT_CONTROLLED_OBJ);
		gameObject.SetActive(false);
		enemyLayerMask = 1 << enemyLayer;
	}


	/// <summary>
	/// Move the attractor (which in turn directs the lightning stream), destroy enemies caught in the stream,
	/// and shut the stream off when time is up.
	/// </summary>
	private void Update(){
		attractor.rotation = RotateTowardInputDirection();
		BlastEnemies();


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


	/// <summary>
	/// Uses a spherecast to detect enemies caught in the lightning stream, and then destroys all enemies detected.
	/// </summary>
	private void BlastEnemies(){
		RaycastHit[] hitInfo = Physics.SphereCastAll(transform.position, 
													 radius,
													 attractor.position - transform.position,
													 Vector3.Distance(attractor.position, transform.position),
													 enemyLayerMask,
													 QueryTriggerInteraction.Ignore);

		if (hitInfo.Length > 0){
			List<EnemyBase> hitEnemies = new List<EnemyBase>();

			foreach (RaycastHit hit in hitInfo){
				if (hit.collider.tag == "Enemy"){
					hitEnemies.Add(hit.collider.GetComponent<EnemyBase>());
				}
			}

			for (int i = hitEnemies.Count - 1; i >= 0; i--){
				hitEnemies[i].GetDestroyed();
			}
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
