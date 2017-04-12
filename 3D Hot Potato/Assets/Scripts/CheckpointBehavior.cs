using System.Collections;
using UnityEngine;

public class CheckpointBehavior : EnemyBase {


	//tunable variables
	public float speed = 1.0f; //how fast the checkpoint slides "down" the screen on the Z axis.


	//internal variables

	//these are the variables that this checkpoint will feed into LevelManager if it's hit
	public int CurrentWorldNum { get; set; }
	public int CurrentActNum { get; set; }
	public float CurrentNextReadTime { get; set; }
	public int CurrentReadIndex { get; set; }


	private LevelManager levelManager;
	private const string MANAGER_OBJ = "Managers";
	private Rigidbody rb;
	private const string PLAYER_TAG = "Player";
	private const string ENEMIES_ORGANIZER = "Enemies";


	//variables relating to the checkpoint's sound effect
	private AudioSource audioSource;
	private bool checkpointActivated = false;


	//used to send a message that the checkpoint's been hit
	private SidelineTextControl sidelineTextControl;
	private const string SIDELINE_TEXT_OBJ = "Sideline text";
	private const string CHECKPOINT_MESSAGE = "Checkpoint!";



	private void Start(){
		levelManager = GameObject.Find(MANAGER_OBJ).GetComponent<LevelManager>();
		rb = GetComponent<Rigidbody>();
		GetCurrentLevelNumbers();
		audioSource = GetComponent<AudioSource>();
		transform.parent = GameObject.Find(ENEMIES_ORGANIZER).transform;
		sidelineTextControl = GameObject.Find(SIDELINE_TEXT_OBJ).GetComponent<SidelineTextControl>();
	}


	private void GetCurrentLevelNumbers(){
		CurrentWorldNum = levelManager.GetWorldNum();
		CurrentActNum = levelManager.GetActNum();
		CurrentNextReadTime = levelManager.GetNextReadTime();
		CurrentReadIndex = levelManager.GetReadIndex();
	}


	private void FixedUpdate(){
		rb.MovePosition(transform.position + (-Vector3.forward * speed * Time.fixedDeltaTime));
	}


	public void OnTriggerEnter(Collider other){
		if (!checkpointActivated && other.tag == PLAYER_TAG){ //don't set a checkpoint if, frex., an enemy goes through the point
			StartCoroutine(sidelineTextControl.ShowText(CHECKPOINT_MESSAGE));
			levelManager.SetCheckpoint(CurrentWorldNum, CurrentActNum, CurrentNextReadTime, CurrentReadIndex);
			checkpointActivated = true;
			audioSource.Play();
		}
	}


	public override void Reset(){
		levelManager = GameObject.Find(MANAGER_OBJ).GetComponent<LevelManager>();
		GetCurrentLevelNumbers();
		checkpointActivated = false;
		audioSource = GetComponent<AudioSource>();
		transform.parent = GameObject.Find(ENEMIES_ORGANIZER).transform;
		sidelineTextControl = GameObject.Find(SIDELINE_TEXT_OBJ).GetComponent<SidelineTextControl>();
	}


	public override void ShutOff(){
		gameObject.SetActive(false);
	}
}
