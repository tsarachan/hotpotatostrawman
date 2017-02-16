using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIgnoring : EnemyBase {


	//-------tunable variables-------
	public float speed = 1.0f;
	public float enterDistance = 0.0f;
	public float maxOffset = 10.0f; //how far the enemy will move as part of the sine wave
	public float frequency = 2.0f; //the frequency of the wave

	//if the enemy is in the scene for this long, it's offscreen and goes back to the object pool
	public float existDuration = 4.0f;


	//-------internal variables-------
	private Rigidbody rb;


	//variables for finding the buildings, so that this enemy knows where it is and how to come on the screen
	private float playAreaSide = 0.0f; //we'll find a building, and use its X coordinate to figure out how far out spawners might be
	private const string BUILDINGS_ORGANIZER = "Buildings";

	//finding where the enemy is going
	private bool enteringScreen = true;
	private Vector3 start;
	private Vector3 end;
	public float enterTime = 2.0f; //time spent lerping into the scene
	private float timer = 0.0f;
	public AnimationCurve enterCurve;
	private Vector3 direction;
	private delegate Vector3 OffsetPosition();
	private OffsetPosition offsetPosition;
	private float offsetTimer = 0.0f;
	private float startXPos = 0.0f;

	//parent the transform
	private const string ENEMY_ORGANIZER = "Enemies";

	//timer for keeping track of how long this enemy should stay in the scene
	private float existTimer = 0.0f;


	//variables for audio
	private AudioSource audioSource;
	private const string SPEAKERS = "Speakers";
	private AudioClip deathClip;
	private const string DEATH_CLIP = "Audio/EnemyDeathSFX";



	private void Start(){
		rb = GetComponent<Rigidbody>();
		direction = GetDirection();
		offsetPosition = GetXOffset;
		startXPos = transform.position.x;
		transform.parent = GameObject.Find(ENEMY_ORGANIZER).transform;
		playAreaSide = Mathf.Abs(GameObject.Find(BUILDINGS_ORGANIZER).transform.GetChild(0).position.x);
		audioSource = GetAudioSource();
		deathClip = Resources.Load(DEATH_CLIP) as AudioClip;

	}


	private AudioSource GetAudioSource(){
		foreach (Transform speaker in transform.root.Find(SPEAKERS)){
			if (speaker.name.Contains(gameObject.name)){
				return speaker.GetComponent<AudioSource>();
			}
		}

		//this should never happen
		Debug.Log("Couldn't find audioSource for " + gameObject.name);
		return null;
	}


	private Vector3 GetDirection(){
		return -Vector3.forward;
	}


	/// <summary>
	/// Keeps track of how long this enemy has been in the scene. If it's long enough that the
	/// enemy is sure to be offscreen and no longer relevant, put it back in the object pool.
	/// </summary>
	private void Update(){
		existTimer += Time.deltaTime;

		if (existTimer >= existDuration){
			ObjectPooling.ObjectPool.AddObj(gameObject);
		}
	}


	private void FixedUpdate(){
		if (enteringScreen){
			rb.MovePosition(MoveOntoScreen());
		} else {
			rb.MovePosition(offsetPosition());

			//rb.MovePosition(transform.position + offset() + direction * speed * Time.fixedDeltaTime);
			//rb.MovePosition(transform.position + direction * speed * Time.fixedDeltaTime);
		}
	}


	private Vector3 GetXOffset(){
		offsetTimer += Time.fixedDeltaTime;

		//get the new X position for this enemy, clamped to the left and right sides of the play area
		float xPos = startXPos + Mathf.Sin(offsetTimer * frequency) * maxOffset;
		if (xPos > playAreaSide){
			xPos = playAreaSide;
		} else if (xPos < -playAreaSide){
			xPos = -playAreaSide;
		}

		Vector3 temp = new Vector3(xPos,
								   transform.position.y,
								   transform.position.z - speed * Time.fixedDeltaTime);

		return temp;
	}


	/// <summary>
	/// Find where this enemy should go to as they move onto the screen.
	/// </summary>
	/// <returns>The entry end point.</returns>
	private Vector3 DetermineEntryEndPoint(){
		return new Vector3(transform.position.x, transform.position.y, transform.position.z - enterDistance);
	}


	private Vector3 MoveOntoScreen(){
		timer += Time.deltaTime;

		Vector3 pos = Vector3.LerpUnclamped(start, end, enterCurve.Evaluate(timer/enterTime));

		if (Vector3.Distance(pos, end) <= Mathf.Epsilon) { enteringScreen = false; }

		return pos;
	}


	public override void GetDestroyed(){
		GameObject destroyParticle = ObjectPooling.ObjectPool.GetObj(DESTROY_PARTICLE);
		destroyParticle.transform.position = transform.position;
	
		Color myColor = GetComponent<Renderer>().material.color;

		destroyParticle.GetComponent<ParticlePlexus>().LineColor = myColor;


		//for reasons unclear, Unity throws an error if you try to set the start color of the particles without
		//first assigning the main module to its own variable
		ParticleSystem.MainModule mainModule = destroyParticle.GetComponent<ParticleSystem>().main;

		mainModule.startColor = myColor;

		audioSource.clip = deathClip;
		audioSource.Play();

		ObjectPooling.ObjectPool.AddObj(gameObject); //shut this off and return it to the pool
	}


	/// <summary>
	/// Call this function to restore default values when an enemy comes out of the pool and into play.
	/// 
	/// Call this *before* the enemy is moved into position, so that everything is in a predictable state when the enemy's own script takes over.
	/// </summary>
	public override void Reset(){
		gameObject.SetActive(true);

		//reset timers so that the enemy behaves correctly when coming out of the pool.
		timer = 0.0f;
		enteringScreen = true;
		offsetTimer = 0.0f;
		existTimer = 0.0f;

		//find the end point of the enemy's entry onto the screen
		start = transform.position;
		end = DetermineEntryEndPoint();

		//revise the starting X position
		startXPos = transform.position.x;

		GetComponent<Rigidbody>().velocity = startVelocity; //sanity check: make absolutely sure the velocity is zero
	}
}
