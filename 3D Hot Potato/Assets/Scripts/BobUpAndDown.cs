using UnityEngine;
using System.Collections;

public class BobUpAndDown : MonoBehaviour {

	public float speed = 1.0f;
	public float range = 1.0f;

	public float jumpHeight = 5.0f;
	public float jumpDuration = 1.0f;
	public AnimationCurve riseCurve;
	public AnimationCurve fallCurve;
	public float startHeightBoost = 1.0f;
	public float jumpLeanAngle = -60.0f;
	public float jumpLandAngle = -60.0f;

	Transform tf;
	Vector3 posVec;
	float startY;



	private bool jumping = false;


	private float bobTimer = 0.0f;

	private PlayerMovementLean playerMovementLean;


	private PlayerMovementParticles particleScript;


	void Awake()
	{
		tf = transform;
		posVec = tf.localPosition;
		startY = posVec.y;
		playerMovementLean = transform.parent.GetComponent<PlayerMovementLean>();
		particleScript = transform.parent.GetComponent<PlayerMovementParticles>();
	}

	void Update ()
	{
		bobTimer += Time.deltaTime;

		posVec.y = startY + range * Mathf.Sin(bobTimer * speed); //using Time.time means all bobbing things will
																	  //be at the same place in their bob
		if (!jumping){
			tf.localPosition = posVec;
		}

		if (Input.GetKeyDown(KeyCode.P)){
			StartCoroutine(Jump());
		}
	}


	public IEnumerator Jump(){
		//Debug.Log("Jump() called for " + transform.parent.name);

		jumping = true;
		particleScript.InAir = true;

		playerMovementLean.StartJumpRise(jumpLeanAngle);

		//Debug.Log("playerMovementLean.StartJumpRise() completed for " + transform.parent.name);

		yield return StartCoroutine(JumpUp());

		playerMovementLean.StartJumpFall(jumpLandAngle);

		//Debug.Log("playerMovementLean.StartJumpFall() for " + transform.parent.name);

		yield return StartCoroutine(FallDown());

		//Debug.Log("Jump() finishing for " + transform.parent.name);

		playerMovementLean.DoneJumping();

		//Debug.Log("playerMovementLean.DoneJumping() completed for " + transform.parent.name);

		jumping = false;

		bobTimer = 0.0f;

		particleScript.InAir = false;

		//Debug.Log("Jump() complete for " + transform.parent.name);

		yield break;
	}


	private IEnumerator JumpUp(){
		//Debug.Log("JumpUp() called for " + transform.parent.name);
		float riseDuration = jumpDuration/2;
		float timer = 0.0f;

		Vector3 jumpStart = tf.localPosition;

		while (timer <= riseDuration){
			//Debug.Log(transform.parent.name + " going up; timer == " + timer);

			timer += Time.deltaTime;
			tf.localPosition = new Vector3(tf.localPosition.x,
										   Mathf.Lerp(jumpStart.y, 
													  jumpStart.y + jumpHeight,
													  riseCurve.Evaluate(timer/riseDuration)),
										   tf.localPosition.z);
			

			yield return null;
		}

		//Debug.Log("JumpUp() ending for " + transform.parent.name);
		yield break;
	}


	private IEnumerator FallDown(){
		//Debug.Log("FallDown() called for " + transform.parent.name);
		float fallDuration = jumpDuration/2;
		float timer = 0.0f;

		Vector3 jumpStart = tf.localPosition;

		while (timer <= fallDuration){
			//Debug.Log(transform.parent.name + " going down; timer == " + timer);

			timer += Time.deltaTime;
			tf.localPosition = new Vector3(tf.localPosition.x,
										   Mathf.LerpUnclamped(jumpStart.y, 
													  startY,
													  fallCurve.Evaluate(timer/fallDuration)),
										   tf.localPosition.z);


			yield return null;
		}


		//Debug.Log("FallDown() ending for " + transform.parent.name);
		yield break;
	}
}
