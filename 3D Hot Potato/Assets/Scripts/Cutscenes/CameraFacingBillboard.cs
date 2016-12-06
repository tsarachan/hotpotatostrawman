/*
 * 
 * Makes the object this script is attached to face the camera.
 * 
 * Original from http://wiki.unity3d.com/index.php?title=CameraFacingBillboard
 * 
 */

namespace Cutscene
{
	using UnityEngine;
	using System.Collections;

	public class CameraFacingBillboard : MonoBehaviour
	{
		private Camera mainCamera;


		private void Start(){
			mainCamera = Camera.main;
		}

		private void Update()
		{
			transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
				mainCamera.transform.rotation * Vector3.up);
		}
	}
}
