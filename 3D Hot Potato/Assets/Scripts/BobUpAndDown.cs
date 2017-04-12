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


	void Awake()
	{
		tf = transform;
		posVec = tf.localPosition;
		startY = posVec.y;
		playerMovementLean = transform.parent.GetComponent<PlayerMovementLean>();
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
		jumping = true;

		playerMovementLean.StartJumpRise(jumpLeanAngle);


		yield return StartCoroutine(JumpUp());

		playerMovementLean.StartJumpFall(jumpLandAngle);

		yield return StartCoroutine(FallDown());

		playerMovementLean.DoneJumping();

		jumping = false;

		bobTimer = 0.0f;

		yield break;
	}


	private IEnumerator JumpUp(){
		float riseDuration = jumpDuration/2;
		float timer = 0.0f;

		Vector3 jumpStart = tf.localPosition;

		while (timer <= riseDuration){
			timer += Time.deltaTime;
			tf.localPosition = new Vector3(tf.localPosition.x,
										   Mathf.Lerp(jumpStart.y, 
													  jumpStart.y + jumpHeight,
													  riseCurve.Evaluate(timer/riseDuration)),
										   tf.localPosition.z);
			

			yield return null;
		}


		yield break;
	}


	private IEnumerator FallDown(){
		float fallDuration = jumpDuration/2;
		float timer = 0.0f;

		Vector3 jumpStart = tf.localPosition;

		while (timer <= fallDuration){
			timer += Time.deltaTime;
			tf.localPosition = new Vector3(tf.localPosition.x,
										   Mathf.LerpUnclamped(jumpStart.y, 
													  startY,
													  fallCurve.Evaluate(timer/fallDuration)),
										   tf.localPosition.z);


			yield return null;
		}


		yield break;
	}
}
