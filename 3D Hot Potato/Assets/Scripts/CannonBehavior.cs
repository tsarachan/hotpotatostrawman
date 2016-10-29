using UnityEngine;
using System.Collections;

public class CannonBehavior : MonoBehaviour {

	private Rigidbody rb;

	private Vector3 posVec = new Vector3(0.0f, 0.0f, 0.0f);
	private float startZ;

	public float speed = 1.0f;
	public float range = 1.0f;

	private void Start(){
		rb = GetComponent<Rigidbody>();
		startZ = transform.localPosition.z;
	}

	//The boss moves left and right.
	private void FixedUpdate(){
		posVec.z = ForwardAndBack();

		rb.MovePosition(posVec);
	}


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
}
