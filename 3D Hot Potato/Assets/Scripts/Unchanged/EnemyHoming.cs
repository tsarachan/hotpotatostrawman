using UnityEngine;
using System.Collections;

public class EnemyHoming : EnemyBase {

	public float chanceOfGoForBall = 0.8f; //between 1.0 and 0.0
	protected const string BALL_OBJ = "Ball";
	public Transform target;
	private Rigidbody rb;
	public float speed = 0.3f;
	public float Speed{
		get { return speed; }
		set { speed = value; }
	}
	public float[] maxSpeeds = { 2.0f, 5.0f, 7.0f };
	private float myMaxSpeed = 0.0f;
	private GameObject destroyParticle;
	public float speedBoost = 2f; //speed multiplier for when the enemy stops homing

	private const string PLAYER_ORGANIZER = "Players";
	private Transform playerOrganizer;

	private const string ENEMY_ORGANIZER = "Enemies";

	public float onScreenTime = 10.0f;
	private float stayTimer = 0.0f;

	private Vector3 direction;
	private Light myPointLight;
	public Color leavingScreenColor;
	public Color preparingToChargeColor; //Zach added this. Feel free to remove it.
	private Color startColor;
	private float startIntensity;


	//these variables are used to bring enemies onto the screen, allowing players to see them before they start attacking
	private bool enteringScreen = true;
	public float enterDistance = 5.0f; //how far the homing enemy will move before it starts attacking
	public float enterTime = 2.0f;
	private float timer = 0.0f;
	public AnimationCurve enterCurve;
	private Vector3 start;
	private Vector3 end;

	private void Start(){
		transform.parent = GameObject.Find(ENEMY_ORGANIZER).transform;
		playerOrganizer = GameObject.Find(PLAYER_ORGANIZER).transform;
		target = ChooseTarget();
		rb = GetComponent<Rigidbody>();
		//destroyParticle = Resources.Load("DestroyParticle") as GameObject;
		myMaxSpeed = maxSpeeds[Random.Range(0, maxSpeeds.Length)];
		start = transform.position;
		end = new Vector3(transform.position.x,
			transform.position.y,
			transform.position.z - enterDistance);
		myPointLight = transform.GetChild(0).GetComponent<Light>();
		startColor = myPointLight.color;
		startIntensity = myPointLight.intensity;
	}




	/// <summary>
	/// Decide which player to chase.
	/// </summary>
	/// <returns>The target.</returns>
	private Transform ChooseTarget(){
		float randomValue = Random.Range(0.0f, 1.0f); //if this is <= chanceOfGoForBall, the enemy will want to chase the ball


		if (randomValue <= chanceOfGoForBall){ //enemy wants to go for the ball
			return GameObject.Find(BALL_OBJ).transform;
		} else { //didn't want to go for the ball; chase a random player
			return playerOrganizer.GetChild(Random.Range(0, playerOrganizer.childCount));
		}
	}

	private void FixedUpdate(){
		//if frozen, this enemy is immobilized. It retains its old force. 
//		if (frozen){
//			frozen = RunFreezeTimer();
//			rb.isKinematic = true;
//			return; //don't gain force, change direction, or build toward charging forward while frozen
//		} else if (!frozen){
//			rb.isKinematic = false; //when not frozen, the enemy can move
//		}

		if (enteringScreen){
			rb.MovePosition(MoveOntoScreen());
		} else {
			if (stayTimer <= onScreenTime){
				direction = GetDirection();
				stayTimer += Time.deltaTime;
				rb.AddForce(direction * Speed, ForceMode.Force);
				if(stayTimer >= onScreenTime - 1.5f){
					myPointLight.color = preparingToChargeColor;
					myPointLight.intensity = 8.0f; //set the light to maximum intensity to signify danger
				}
			} else {
				//stop changing direction; the enemy goes off-screen
				rb.AddForce(direction * Speed * speedBoost, ForceMode.Force);
				myPointLight.color = leavingScreenColor;
				myPointLight.intensity = 8.0f; //set the light to maximum intensity to signify danger
			}

			//This is a bodge to limit maximum speed. The better way would be to impose a countervailing force.
			//Directly manipulating rigidbody velocity could lead to physics problems.
			if (rb.velocity.magnitude > myMaxSpeed) { rb.velocity = rb.velocity.normalized * myMaxSpeed; }
		}
	}

	private Vector3 MoveOntoScreen(){
		timer += Time.deltaTime;

		Vector3 pos = Vector3.LerpUnclamped(start, end, enterCurve.Evaluate(timer/enterTime));

		if (Vector3.Distance(pos, end) <= Mathf.Epsilon) { enteringScreen = false; }

		return pos;
	}

	private Vector3 GetDirection(){
		if (target != null){
			return (target.position - transform.position).normalized;
		} else {
			return transform.position; //stop in place if the player this enemy is searching for has been destroyed
		}
	}

	public override void GetDestroyed(){
		GetComponent<ParticleBurst>().MakeBurst();
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
		stayTimer = 0.0f;
		enteringScreen = true;


		//reset the enemy's light
		//the if-statement prevents this from running the first time this enters the scene,
		//when myPointLight has yet to be initialized. It's unnecessary at that point.
		if (myPointLight != null){
			myPointLight.color = startColor;
			myPointLight.intensity = startIntensity;
		}

		GetComponent<Rigidbody>().velocity = startVelocity; //sanity check: make absolutely sure the velocity is zero
	}
}