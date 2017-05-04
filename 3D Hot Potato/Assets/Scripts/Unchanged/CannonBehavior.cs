using UnityEngine;
using System.Collections;

public class CannonBehavior : MonoBehaviour {

	private Rigidbody rb;
	private AudioSource audioSource;

	private Vector3 posVec = new Vector3(0.0f, 0.0f, 0.0f);
	private float startZ;

	public float speed = 1.0f;
	public float range = 1.0f;

	private const string BALL_OBJ = "Ball";
	private const string CANNONBALL_OBJ = "Cannonball";
	private GameObject cannonball;
	private bool reloading = false;
	public float reloadTime = 1.0f; //how long the players have to wait between attacks
	private float timer = 0.0f;


	private void Start(){
		rb = GetComponent<Rigidbody>();
		audioSource = GetComponent<AudioSource>();
		startZ = transform.localPosition.z;
		cannonball = Resources.Load(CANNONBALL_OBJ) as GameObject;
	}

	//The boss moves left and right.
//	private void FixedUpdate(){
//		posVec.z = ForwardAndBack();
//
//		rb.MovePosition(posVec);
//	}


	/// <summary>
	/// Move the cannon forward and back along the Z axis.
	/// 
	/// The formula uses:
	/// 
	/// transform.parent.position.z: enables the cannon to scroll in with the whole setpiece
	/// startZ: keeps the center of the cannon's movement ahead of the players, rather than snapping it to (0, 0, 0)
	/// range: the maximum extent of movement.
	/// Mathf.Sin(Time.time): makes the cannon go back and forth.
	/// speed: how fast the cannon moves.
	/// 
	/// </summary>
	/// <returns>The position the cannon should move to this frame.</returns>
	private float ForwardAndBack(){
		float z = transform.parent.position.z + startZ + range * Mathf.Sin(Time.time * speed);

		return z;
	}

	/// <summary>
	/// Shoot the cannon when the players hit it with the ball, if it's not reloading.
	/// </summary>
	/// <param name="other">The collider that's hit the cannon.</param>
	private void OnTriggerEnter(Collider other){
		if (other.gameObject.name.Contains(BALL_OBJ) && !reloading){
			reloading = Shoot();
		}
	}

	/// <summary>
	/// Create a cannonball and set the cannon to reloading.
	/// 
	/// This doesn't tell the cannonball what to do; that's the cannonball's responsibility.
	/// </summary>
	private bool Shoot(){
		Instantiate(cannonball, transform.position, Quaternion.identity);
		audioSource.Play();

		return true;
	}

	/// <summary>
	/// If the cannon is reloading, run the timer until it's reloaded.
	/// </summary>
	private void Update(){
		if (reloading){
			reloading = RunReloadTimer();
		}
	}


	/// <summary>
	/// Increments the reload timer until the cannon has reloaded.
	/// </summary>
	/// <returns><c>true</c> if the reload time is complete <c>false</c> otherwise.</returns>
	private bool RunReloadTimer(){
		timer += Time.deltaTime;

		if (timer >= reloadTime){
			timer = 0.0f;
			return false;
		} else {
			return true;
		}
	}
}
