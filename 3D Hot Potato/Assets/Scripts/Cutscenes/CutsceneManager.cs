/*
 * 
 * This script is the director and cinematographer--it tells all the other scripts involved in the cutscene when to act.
 * 
 */


namespace Cutscene
{
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;

	public class CutsceneManager : MonoBehaviour {


		//these are the various scripts used in the cutscene
		private CameraMove cameraMoveScript;
		private const string CAMERA_BOOM = "Camera boom";

		//these variables are used to manage when each event occurs
		private float timer = 0.0f; //how long the cutscene has run; this is the master timer that controls when things happen
		private List<float> eventTimes = new List<float>(); //all the times when this script will send an instruction to another script
		private int index = 0; //which event are we on? Zero-indexed to work with eventTimes.


		//everything below this is "direction" from the director

		//pull the camera back to show the players
		[Header("Move the camera back to show players")]
		public float moveBackStartTime = 0.0f;
		public float moveBackDuration = 2.0f;
		public Vector3 moveBackToPos = new Vector3(0.0f, 0.0f, 0.0f);
		public AnimationCurve moveBackCurve;


		private void Start(){
			cameraMoveScript = GameObject.Find(CAMERA_BOOM).GetComponent<CameraMove>();
			eventTimes = GetEventTimes();
		}

		private List<float> GetEventTimes(){
			List<float> temp = new List<float>();

			temp.Add(moveBackStartTime);

			return temp;
		}

		private void Update(){
			if (index < eventTimes.Count){ //stop trying to do things once the cutscene is complete
				timer += Time.deltaTime;

				if (timer >= eventTimes[index]){
					index = PerformAction();
				}
			}
		}

		private int PerformAction(){
			switch (index){
				case 0:
					cameraMoveScript.MoveBoom(moveBackToPos, moveBackDuration, moveBackCurve);
					break;

			}


			index ++;

			return index;
		}
	}
}
