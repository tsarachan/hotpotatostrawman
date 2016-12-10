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
		private PlayerSkid p1SkidScript;
		private const string P1_AXIS = "P1 axis";
		private PlayerMovement p2MovementScript;
		private PlayerBallInteraction p2BallScript;
		private const string P2 = "Player 2";
		private PlayerSkid p2SkidScript;
		private const string P2_AXIS = "P2 axis";

		//these variables are used to manage when each event occurs
		private float timer = 0.0f; //how long the cutscene has run; this is the master timer that controls when things happen
		private List<float> eventTimes = new List<float>(); //all the times when this script will send an instruction to another script
		public int index = 0; //which event are we on? Zero-indexed to work with eventTimes. Start the first cutscene at 0, the second at 5.

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

		[Header("P2 passes to P1")]
		public float p2PassTime = 0.0f;

		[Header("P1 moves off-screen")]
		public float p1LeaveStartTime = 0.0f;
		public float p1LeaveDuration = 1.0f;

		[Header("P2 moves off-screen")]
		public float p2LeaveStartTime = 0.0f;
		public float p2LeaveDuration = 1.0f;

		[Header("P1 skids to a stop")]
		public float p1YSkidStartTime = 0.0f;
		public float p1YSkidDuration = 1.5f;
		public float p1YSkidAngle = 90.0f;
		public AnimationCurve p1YSkidCurve;

		public float p1ZSkidSartTime = 0.0f;
		public float p1ZSkidDuration = 1.5f;
		public float p1ZSkidAngle = -45.0f;
		public AnimationCurve p1ZSkidCurve;

		[Header("P2 moves forward while stopping")]
		public float p2ForwardStartTime = 0.0f;
		public float p2ForwardDuration = 0.25f;
		private Transform p2Axis;
		public float p2BaseSpeed = 1.0f;
		private float p2CurrentSpeed = 1.0f;
		public AnimationCurve p2MoveCurve;

		[Header("P2 skids to a stop")]
		public float p2YSkidStartTime = 0.0f;
		public float p2YSkidDuration = 1.5f;
		public float p2YSkidAngle = -90.0f;
		public AnimationCurve p2YSkidCurve;

		public float p2ZSkidStartTime = 0.0f;
		public float p2ZSkidDuration = 1.5f;
		public float p2ZSkidAngle = 45.0f;
		public AnimationCurve p2ZSkidCurve;

		[Header("The camera pans up")]
		public float cameraSlerpStartTime = 0.0f;
		public float cameraSlerpDuration = 1.5f;
		public Vector3 cameraSlerpAngle = new Vector3(0.0f, 0.0f, 0.0f); //this is an amount of change, not the final angle
		public AnimationCurve slerpCurve;



		private void Start(){
			cameraMoveScript = GameObject.Find(CAMERA_BOOM).GetComponent<CameraMove>();
			enemyDirected1 = GameObject.Find(ENEMY_1).GetComponent<EnemyDirected>();
			p1MovementScript = GameObject.Find(P1).GetComponent<PlayerMovement>();
			p2MovementScript = GameObject.Find(P2).GetComponent<PlayerMovement>();
			p2BallScript = GameObject.Find(P2).GetComponent<PlayerBallInteraction>();
			p1SkidScript = GameObject.Find(P1_AXIS).GetComponent<PlayerSkid>();
			p2SkidScript = GameObject.Find(P2_AXIS).GetComponent<PlayerSkid>();
			p2Axis = GameObject.Find(P2_AXIS).transform;
			p2CurrentSpeed = p2BaseSpeed;
			eventTimes = GetEventTimes();
		}

		/// <summary>
		/// Create a list of all the times when this script will give an instruction.
		/// 
		/// This list is completely manual--to add or subtract something, do it here.
		/// </summary>
		/// <returns>A list of times, in seconds, when this script will tell another script to do something.</returns>
		private List<float> GetEventTimes(){
			List<float> temp = new List<float>();

			temp.Add(moveBackStartTime);
			temp.Add(enemyMoveStartTime);
			temp.Add(p2DodgeStartTime);
			temp.Add(p2PassTime);
			temp.Add(p1LeaveStartTime);
			temp.Add(p2LeaveStartTime);
			temp.Add(p1YSkidStartTime);
			temp.Add(p1ZSkidSartTime);
			temp.Add(p2ForwardStartTime);
			temp.Add(p2YSkidStartTime);
			temp.Add(p2ZSkidStartTime);
			temp.Add(cameraSlerpStartTime);

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
				case 1:
					enemyDirected1.MoveEnemy(enemyMoveToPos, enemyMoveDuration, enemyMoveCurve);
					break;
				case 2:
					StartCoroutine(MovePlayer(p2MovementScript, p2DodgeDuration, new List<string> { DOWN, RIGHT }));
					break;
				case 3:
					p2BallScript.Throw();
					break;
				case 4:
					StartCoroutine(MovePlayer(p1MovementScript, p1LeaveDuration, new List<string> {DOWN}));
					break;
				case 5:
					StartCoroutine(MovePlayer(p2MovementScript, p2LeaveDuration, new List<string> {DOWN}));
					break;
				case 6:
					p1SkidScript.RotateAlongY(p1YSkidAngle, p1YSkidDuration, p1YSkidCurve);
					break;
				case 7:
					p1SkidScript.RotateAlongZ(p1ZSkidAngle, p1ZSkidDuration, p1ZSkidCurve);
					break;
				case 8:
					StartCoroutine(MovePlayerAxis(p2Axis, p2ForwardDuration, UP));
					break;
				case 9:
					p2SkidScript.RotateAlongY(p2YSkidAngle, p2YSkidDuration, p2YSkidCurve);
					break;
				case 10:
					p2SkidScript.RotateAlongZ(p2ZSkidAngle, p2ZSkidDuration, p2ZSkidCurve);
					break;
				case 11:
					cameraMoveScript.SlerpCamera(Quaternion.Euler(cameraSlerpAngle), cameraSlerpDuration, slerpCurve);
					break;
				default:
					Debug.Log("Illegal index: " + index);
					break;
			}


			index ++;

			return index;
		}
			


		/// <summary>
		/// Make a player move by imitating inputs.
		/// </summary>
		/// <param name="moveScript">The player's PlayerMovement script.</param>
		/// <param name="duration">How long the input should be given.</param>
		/// <param name="directions">A list of the directions to be sent as inputs.</param>
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


		/// <summary>
		/// Move a player's entire axis, so that players can be moved around the scene and then still rotate
		/// reasonably.
		/// </summary>
		private IEnumerator MovePlayerAxis(Transform axis, float duration, string direction){
			float moveTimer = 0.0f;

			while (moveTimer < duration){
				p2CurrentSpeed = Mathf.Lerp(p2BaseSpeed, 0.0f, p2MoveCurve.Evaluate(timer/duration));

				switch (direction){
					case UP:
						axis.position += new Vector3(0.0f, 0.0f, p2CurrentSpeed);
						break;
					default:
						Debug.Log("Illegal direction: " + direction);
						break;
				}

				moveTimer += Time.deltaTime;

				yield return null;
			}

			yield break;
		}
	}
}
