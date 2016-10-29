using UnityEngine;
using System.Collections;

public class EnemyBoss : EnemyBase {

	private Rigidbody rb;

	private Vector3 posVec = new Vector3(0.0f, 0.0f, 0.0f);
	private float startZ;
	public int health = 3;

	public float speed = 1.0f;
	public float range = 1.0f;

	private void Start(){
		rb = GetComponent<Rigidbody>();
		//posVec = transform.localPosition; //move relative to the whole boss fight, which will slide into position as a set
		startZ = transform.localPosition.z;
	}

	//The boss moves left and right.
	private void FixedUpdate(){
		posVec.z = transform.parent.position.z + startZ;
		posVec.x = BackAndForth();

		rb.MovePosition(posVec);
	}


	/// <summary>
	/// Move the boss left and right along the x-axis.
	/// 
	/// The formula uses:
	/// 
	/// range: the maximum extent of movement.
	/// Mathf.Sin(Time.time): makes the boss go back and forth.
	/// speed: how fast the boss moves.
	/// 
	/// </summary>
	/// <returns>The position the boss should move to this frame.</returns>
	private float BackAndForth(){
		float x = range * Mathf.Sin(Time.time * speed);

		return x;
	}

	public void GetHit(){
		health--;
		GetComponent<ParticleBurst>().MakeBurst();
		if (health <= 0){
			GetDestroyed();
		}
	}


	public override void GetDestroyed(){
		GetComponent<ParticleBurst>().MakeBurst(); //make more particles, just for good measure
		Destroy(transform.parent.gameObject); //get rid of all the parts of the boss fight
	}
}
