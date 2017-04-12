/*
 * 
 * This script handles the ball's movement betwen players
 * 
 */

using UnityEngine;
using System.Collections;

public class BallBehavior : MonoBehaviour {


	//----------Tunable variables----------
	public AnimationCurve normalTossCurve; //can be used to make the ball move faster at start then lose speed over time, etc.
	public float flightTimePerUnitDistance = 0.1f; //effectively speed
	public AnimationCurve verticalCurve; //used to give the ball a "lob" effect
	public float verticalHeight = 2.0f; //height the ball reaches at the apex of the lob
	public float verticalOffset = 2.0f; //how high above a player the ball is held


	//----------Internal variables----------
	protected Rigidbody rb;
	protected AudioSource audioSource;

	//variables for parenting the ball to the root transform while in the air
	protected const string SCENE_ORGANIZER = "Scene";
	protected Transform scene;


	//used to parent the ball to the player model while being held
	protected const string CYCLE_OBJ = "Cycle and rider";

	//this will store the coroutine that moves the ball, so that it can be stopped early if necessary
	protected Coroutine co;
	public Coroutine Co{
		get { return co; }
	}

	//if powerups based on a super meter are being used, these variables set up the meter
	protected const string SUPER_METER = "Super meter";
	protected PowerUp powerUpScript;


	//the player who's going to catch a pass. CatchBehavior uses this to decide who can do awesome catches.
	public Transform IntendedReceiver { get; set; }


	//used to reset the ball when the game starts
	private Vector3 myStartPos = new Vector3(0.0f, 0.0f, 0.0f);


	//variables used to show a particle when the battery star is in position for an awesome catch
	private GameObject awesomeCatchParticle;
	private const string AWESOME_CATCH_PARTICLE = "Ball particle";
	private float awesomeCatchDistance = 0.0f;
	private CatchBehavior p1CatchScript; //player 1's awesome catch distance will serve for both players
	private const string PLAYER_1 = "Player 1";


	//variables used to signal a player tht the ball is on the way to them
	private const string CATCH_WARNING = "Catch warning";


	//used to track whether the awesome catch particle is active or not, and to send an event when it turns active
	private bool activeParticleOn = false;
	public bool ActiveParticleOn{
		get { return activeParticleOn; }
		set {
			if (value != activeParticleOn){
				if (value == true){
					Services.EventManager.Fire(new PowerReadyEvent());
				}

				activeParticleOn = value;
			}
		}
	}


	//initialize variables
	protected virtual void Start(){
		rb = GetComponent<Rigidbody>();
		scene = GameObject.Find(SCENE_ORGANIZER).transform;
		audioSource = GetComponent<AudioSource>();
		myStartPos = transform.position;
		awesomeCatchParticle = transform.Find(AWESOME_CATCH_PARTICLE).gameObject;
		awesomeCatchDistance = GameObject.Find(PLAYER_1).GetComponent<CatchBehavior>().GetAwesomeCatchDistance();
	}

	/// <summary>
	/// Players should call this function to pass between players.
	/// </summary>
	/// <param name="start">The ball's current position.</param>
	/// <param name="destination">The catching player.</param>
	public virtual void Pass(Vector3 start, Transform destination){
		transform.parent = scene; //stop being a child of the ball carrier, so that the ball can move between players
		IntendedReceiver = destination;
		co = StartCoroutine(PassBetweenPlayers(start, destination));

		//play a sound when the ball is thrown, unless the passes are happening fast enough that the sounds would overlap
		if (!audioSource.isPlaying){
			audioSource.Play();
		}

		//switch on the receiver's warning that the battery star is on the way
		destination.Find(CATCH_WARNING).gameObject.SetActive(true);
	}

	/// <summary>
	/// This coroutine moves the ball between the players.
	/// </summary>
	/// <param name="start">The ball's location as the throw begins.</param>
	/// <param name="destination">The catching player.</param>
	public virtual IEnumerator PassBetweenPlayers(Vector3 start, Transform destination){
		float totalFlightTime = Vector3.Distance(start, destination.position)/flightTimePerUnitDistance;
		float timer = 0.0f;

		while (timer <= totalFlightTime){
			//update totalFlightTime every frame, so that the ball moves faster or slower depending on the distance
			//this should also avoid a bug wherein the ball gets stuck at a high elevation
			totalFlightTime = Vector3.Distance(start, destination.position)/flightTimePerUnitDistance;
			timer += Time.deltaTime;

			Vector3 nextPoint = Vector3.Lerp(start,
											 destination.position,
											 normalTossCurve.Evaluate(timer/totalFlightTime));

			//make the ball arc between the elevated points where the players hold the ball
			//note that the players' colliders have to be tall enough to "catch" the ball up high
			nextPoint.y = Mathf.Lerp(start.y, start.y + verticalHeight, verticalCurve.Evaluate(timer/totalFlightTime));

			rb.MovePosition(nextPoint);

			if (Vector3.Distance(transform.position, destination.position) <= awesomeCatchDistance){
				awesomeCatchParticle.SetActive(true);
				ActiveParticleOn = true;
			}

			yield return null;
		}

		FallToGround(destination.position);

		Debug.Log("Coroutine stopped: y == " + transform.position.y);
		yield break;
	}


	/// <summary>
	/// This is a brute-force solution to the bug wherein the ball stops in the air. It forces the ball to the ground.
	/// </summary>
	/// <param name="destination">The world space point where the ball is meant to stop.</param>
	public virtual void FallToGround(Vector3 destination){
		if (destination.y != 0.0f){
			destination.y = 0.0f;
		}

		rb.MovePosition(destination);
	}

	/// <summary>
	/// Players call this when they catch or pick up the ball.
	/// </summary>
	/// <param name="catchingPlayer">The player interacting with the ball.</param>
	public void GetCaught(Transform catchingPlayer){
		if (Co != null){
			StopCoroutine(Co);
		}

		transform.parent = catchingPlayer.Find(CYCLE_OBJ);

		//changes to the player model may require changing the offset axis here
		transform.localPosition = new Vector3(0.0f, verticalOffset, 0.0f);

		IntendedReceiver = transform; //default assignment

		awesomeCatchParticle.SetActive(false);
		ActiveParticleOn = false;

		//switch off the catching player's warning that they're the receiver
		catchingPlayer.Find(CATCH_WARNING).gameObject.SetActive(false);
	}


	/// <summary>
	/// When the game restarts after the players die, call this.
	/// </summary>
	public void ResetBall(){
		GetCaught(GameObject.Find(PLAYER_1).transform);
		GameObject.Find(PLAYER_1).GetComponent<PlayerBallInteraction>().BallCarrier = true;
	}
}
