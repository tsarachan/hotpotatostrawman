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
		private EnemyDirected enemyDirected1;
		private const string ENEMY_1 = "CutsceneEnemy 1";
		private PlayerMovement p1MovementScript;
		private const string P1 = "Player 1";
		private PlayerMovement p2MovementScript;
		private const string P2 = "Player 2";

		//these variables are used to manage when each event occurs
		private float timer = 0.0f; //how long the cutscene has run; this is the master timer that controls when things happen
		private List<float> eventTimes = new List<float>(); //all the times when this script will send an instruction to another script
		private int index = 0; //which event are we on? Zero-indexed to work with eventTimes.

		//these match the directions in InputManager; they're used to move the players during the cutscene
		private const string UP = "up";
		private const string DOWN = "down";
		private const string LEFT = "left";
		private const string RIGHT = "right";


		//everything below this is "direction" from the director

		[Header("Move the camera back to show players")]
		public float moveBackStartTime = 0.0f;
		public float moveBackDuration = 2.0f;
		public Vector3 moveBackToPos = new Vector3(0.0f, 0.0f, 0.0f);
		public AnimationCurve moveBackCurve;

		[Header("An enemy chases P2 from behind")]
		public float enemyMoveStartTime = 0.0f;
		public float enemyMoveDuration = 2.0f;
		public Vector3 enemyMoveToPos = new Vector3(-11.9f, 0.0f, 35.62f);
		public AnimationCurve enemyMoveCurve;

		[Header("P2 dodges")]
		public float p2DodgeStartTime = 0.0f;
		public float p2DodgeDuration = 1.0f;

		[Header("P1 moves off-screen")]
		public float p1LeaveStartTime = 0.0f;
		public float p1LeaveDuration = 1.0f;

		[Header("P2 moves off-screen")]
		public float p2LeaveStartTime = 0.0f;
		public float p2LeaveDuration = 1.0f;



		private void Start(){
			cameraMoveScript = GameObject.Find(CAMERA_BOOM).GetComponent<CameraMove>();
			enemyDirected1 = GameObject.Find(ENEMY_1).GetComponent<EnemyDirected>();
			p1MovementScript = GameObject.Find(P1).GetComponent<PlayerMovement>();
			p2MovementScript = GameObject.Find(P2).GetComponent<PlayerMovement>();
			eventTimes = GetEventTimes();
		}

		private List<float> GetEventTimes(){
			List<float> temp = new List<float>();

			temp.Add(moveBackStartTime);
			temp.Add(enemyMoveStartTime);
			temp.Add(p2DodgeStartTime);
			temp.Add(p1LeaveStartTime);
			temp.Add(p2LeaveStartTime);

			return temp;
		}

		private void Update(){
			Debug.DrawLine(enemyDirected1.transform.position, enemyMoveToPos, Color.red, 2.0f);

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
				case 1:
					enemyDirected1.MoveEnemy(enemyMoveToPos, enemyMoveDuration, enemyMoveCurve);
					break;
				case 2:
					StartCoroutine(MovePlayer(p2MovementScript, p2DodgeDuration, new List<string> { DOWN, RIGHT }));
					break;
				case 3:
					StartCoroutine(MovePlayer(p1MovementScript, p1LeaveDuration, new List<string> {DOWN}));
					break;
				case 4:
					StartCoroutine(MovePlayer(p2MovementScript, p2LeaveDuration, new List<string> {DOWN}));
					break;
				default:
					Debug.Log("Illegal index: " + index);
					break;
			}


			index ++;

			return index;
		}

		private IEnumerator MovePlayer(PlayerMovement moveScript, float duration, List<string> directions){
			float moveTimer = 0.0f;
			while(moveTimer <= duration){
				foreach (string direction in directions){
					moveScript.Move(direction);
				}

				moveTimer += Time.deltaTime;

				yield return null;
			}

			yield break;
		}
	}
}
