/*
 * 
 * This script moves a parent object of the players to imitate skidding.
 * 
 */

namespace Cutscene
{
	using UnityEngine;
	using System.Collections;

	public class PlayerSkid : MonoBehaviour {

		private float timer = 0.0f;

		//variables for rotating along the Y-axis (yaw, but reversed since we're kicking the back out)
		private float startYRotation = 0.0f;
		private float finalYRotation = 0.0f;
		private float yRotationStartTime = 0.0f;
		private float yRotationEndTime = 0.0f;
		private AnimationCurve yRotationCurve;

		//variables for rotating along the Z-axis (roll)
		private float startZRotation = 0.0f;
		private float finalZRotation = 0.0f;
		private float zRotationStartTime = 0.0f;
		private float zRotationEndTime = 0.0f;
		private AnimationCurve zRotationCurve;

		private void Update(){
			timer += Time.deltaTime;

			Vector3 temp = transform.eulerAngles;

			if (timer >= yRotationStartTime && timer <= yRotationEndTime){
				temp.y = GetYRotation();
			}

			if (timer >= zRotationStartTime && timer <= zRotationEndTime){
				temp.z = GetZRotation();
			}

			transform.eulerAngles = temp;
		}


		/// <summary>
		/// Rotate around the Y-axis (yaw, reversed so that the back wheel kick out). Values are set by RotateAlongY(), below.
		/// </summary>
		/// <returns>The new Y rotation.</returns>
		private float GetYRotation(){
			return Mathf.LerpUnclamped(startYRotation,
							  finalYRotation,
							  yRotationCurve.Evaluate((timer - yRotationStartTime)/(yRotationEndTime - yRotationStartTime)));
		}


		/// <summary>
		/// CutsceneManager calls this to start a Y-axis rotation.
		/// </summary>
		/// <param name="yRotation">The intended final Y rotation, in degrees.</param>
		/// <param name="yRotateTime">The time to spend rotating, in seconds.</param>
		/// <param name="yCurve">An animation curve to fine-tune the rotation.</param>
		public void RotateAlongY(float yRotation, float yRotateTime, AnimationCurve yCurve){
			startYRotation = transform.eulerAngles.y;
			finalYRotation = yRotation;
			yRotationStartTime = Time.time;
			yRotationEndTime = Time.time + yRotateTime;
			yRotationCurve = yCurve;
		}


		/// <summary>
		/// Rotate around the Z-axis (roll). Values are set by RotateAlongZ(), below.
		/// </summary>
		/// <returns>The new Z rotation.</returns>
		private float GetZRotation(){
			return Mathf.LerpUnclamped(startZRotation,
							  finalZRotation,
							  zRotationCurve.Evaluate((timer - zRotationStartTime)/(zRotationEndTime - zRotationStartTime)));
		}


		/// <summary>
		/// CutsceneManager calls this to start a Z-axis rotation.
		/// </summary>
		/// <param name="zRotation">The intended final Z rotation, in degrees.</param>
		/// <param name="zRotateTime">The time to spend rotating, in seconds.</param>
		/// <param name="zCurve">An animation curve to fine-tune the rotation.</param>
		public void RotateAlongZ(float zRotation, float zRotateTime, AnimationCurve zCurve){
			startZRotation = transform.eulerAngles.z;
			finalZRotation = zRotation;
			zRotationStartTime = Time.time;
			zRotationEndTime = Time.time + zRotateTime;
			zRotationCurve = zCurve;
		}
	}
}
