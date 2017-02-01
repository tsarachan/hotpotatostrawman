using System.Collections;
using UnityEngine;

public class CheckpointBehavior : ObjectPooling.Poolable {


	//tunable variables
	public float speed = 1.0f; //how fast the checkpoint slides "down" the screen on the Z axis.


	//internal variables
	private LevelManager levelManager;
	private const string MANAGER_OBJ = "Managers";
	private Rigidbody rb;
	private const string PLAYER_TAG = "Player";
	private UIElementMove checkpointText;
	private const string CHECKPOINT_TEXT = "Checkpoint text";



	private void Start(){
		levelManager = GameObject.Find(MANAGER_OBJ).GetComponent<LevelManager>();
		rb = GetComponent<Rigidbody>();
		checkpointText = GameObject.Find(CHECKPOINT_TEXT).GetComponent<UIElementMove>();
	}


	private void FixedUpdate(){
		rb.MovePosition(transform.position + (-Vector3.forward * speed * Time.fixedDeltaTime));
	}


	public void OnTriggerEnter(Collider other){
		if (other.tag == PLAYER_TAG){ //don't set a checkpoint if, frex., an enemy goes through the point
			levelManager.SetCheckpoint();
			checkpointText.StartUIElementMoving();
		}
	}


	public override void Reset(){
		//intentionally blank
	}


	public override void ShutOff(){
		gameObject.SetActive(false);
	}
}
