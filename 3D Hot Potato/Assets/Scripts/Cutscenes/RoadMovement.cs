/*
 * 
 * This script moves the road like the treadmill, but slows it to a halt rather than treadmilling.
 * 
 */

namespace Cutscene
{
	using UnityEngine;
	using System.Collections;

	public class RoadMovement : MonoBehaviour {

		public float startSpeed = 50.0f;
		public float slowDuration = 1.5f;
		public AnimationCurve slowCurve;

		private float currentSpeed = 0.0f;
		private float timer = 0.0f;


		private void Start(){
			currentSpeed = startSpeed;
		}


		private void Update(){
			timer += Time.deltaTime;

			currentSpeed = SlowDown();

			transform.localPosition -= Vector3.forward * currentSpeed * Time.deltaTime;
		}


		private float SlowDown(){
			return Mathf.Lerp(startSpeed, 0.0f, slowCurve.Evaluate(timer/slowDuration));
		}
	}
}
