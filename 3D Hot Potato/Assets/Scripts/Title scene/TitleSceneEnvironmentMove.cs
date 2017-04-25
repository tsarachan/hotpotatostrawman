namespace TitleScene
{
	using UnityEngine;
	using System.Collections;

	public class TitleSceneEnvironmentMove : MonoBehaviour {

		public float speed = 50.0f;
		public Vector3 startPos = new Vector3(-34.9f, 9.04f, 74.8f);
		public float resetZ = -25.0f; //the z-position where this object will cycle back to the front


		protected float currentSpeed = 0.0f;
		public AnimationCurve startMovingCurve;
		protected float timer = 0.0f;
		public float timeToFullSpeed = 1.0f;


		protected virtual void Update(){
			currentSpeed = GetCurrentSpeed();

			transform.localPosition -= Vector3.forward * currentSpeed * Time.deltaTime;

			if (transform.localPosition.z <= resetZ) {

				transform.localPosition = startPos;
			}
		}


		protected float GetCurrentSpeed(){
			if (timer >= timeToFullSpeed){
				return speed;
			} else {
				timer += Time.deltaTime;

				return Mathf.Lerp(0.0f, speed, startMovingCurve.Evaluate(timer/timeToFullSpeed));
			}
		}
	}
}