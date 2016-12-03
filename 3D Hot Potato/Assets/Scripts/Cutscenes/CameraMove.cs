/*
 * 
 * This script handles all camera movement for the cutscene. The camera is on an invisible boom, which can be seen in the inspector hierarchy.
 * To reposition the camera, rotate the boom. To rotate the camera at the end of the boom, rotate the camera itself.
 * This script has functions to do both of those things.
 * 
 */

using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour {

	//transforms that will move to pan the camera
	private Transform mainCamera;
	private Transform cameraBoom;

	//values for when the camera will swing on the boom or rotate
	private float timer = 0.0f;
	private float startBoomSwingTime = 0.0f;
	private float endBoomSwingTime = 0.0f;
	private Vector3 startBoomSwingRotation = Vector3.zero;
	private Vector3 endBoomSwingRotation = Vector3.zero;
	private AnimationCurve boomSwingCurve;

	private float startCameraRotateTime = 0.0f;
	private float endCameraRotateTime = 0.0f;
	private Vector3 startCameraRotation = Vector3.zero;
	private Vector3 endCameraRotation = Vector3.zero;
	private AnimationCurve cameraRotateCurve;



	private void Start(){
		mainCamera = Camera.main.transform;
		cameraBoom = transform;
	}

	/// <summary>
	/// This function controls when the camera moves.
	/// </summary>
	private void Update(){
		timer += Time.deltaTime;

		if (timer >= startBoomSwingTime && timer <= endBoomSwingTime){
			cameraBoom.eulerAngles = RotateBoomTransform();
		}

		if (timer >= startCameraRotateTime && timer <= endCameraRotateTime){
			mainCamera.eulerAngles = RotateCameraTransform();
		}
	}


	/// <summary>
	/// This rotates the boom, which repositions the camera. The values it uses are set in SwingBoom(), below.
	/// </summary>
	/// <returns>The boom's new rotation, as a Vector3.</returns>
	private Vector3 RotateBoomTransform(){
		return Vector3.Lerp(startBoomSwingRotation,
							endBoomSwingRotation,
							boomSwingCurve.Evaluate((timer - startBoomSwingTime)/(endBoomSwingTime - startBoomSwingTime)));
	}


	/// <summary>
	/// The CutsceneManager calls this to swing the camera boom. This function sets all the necessary values; the actual work
	/// is done by RotateBoomTransform(), above.
	/// </summary>
	/// <param name="endRotation">The boom's rotation when finished, as a Vector3</param>.</param>
	/// <param name="swingTime">The amount of time to spend rotating, in seconds.</param>
	/// <param name="swingCurve">The animation curve to be used for the swing.</param>
	public void SwingBoom(Vector3 endRotation, float swingTime, AnimationCurve swingCurve){
		startBoomSwingTime = Time.time;
		endBoomSwingTime = Time.time + swingTime;
		startBoomSwingRotation = cameraBoom.eulerAngles;
		endBoomSwingRotation = endRotation;
		boomSwingCurve = swingCurve;
	}


	/// <summary>
	/// Rotates the camera transform. Like RotateBoomTransform(), this does the work, and RotateCamera(), below, sets the values.
	/// </summary>
	/// <returns>The camera's new rotation, as a Vector3.</returns>
	private Vector3 RotateCameraTransform(){
		return Vector3.Lerp(startCameraRotation,
							endCameraRotation,
							cameraRotateCurve.Evaluate((timer - startCameraRotateTime)/(endCameraRotateTime - startCameraRotateTime)));
	}


	/// <summary>
	/// CutsceneManager calls this to rotate the camera. This function sets the values used by RotateCameraTransform(), above.
	/// </summary>
	/// <param name="endRotation">The camera's rotation when finished, as a Vector3.</param>
	/// <param name="rotateTime">The time to spend rotating, in seconds.</param>
	/// <param name="rotateCurve">The animation curve to use for the rotation.</param>
	public void RotateCamera(Vector3 endRotation, float rotateTime, AnimationCurve rotateCurve){
		startCameraRotateTime = Time.time;
		endCameraRotateTime = Time.time + rotateTime;
		startCameraRotation = mainCamera.eulerAngles;
		endCameraRotation = endRotation;
		cameraRotateCurve = rotateCurve;
	}
}
