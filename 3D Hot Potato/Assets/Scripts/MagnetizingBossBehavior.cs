namespace MagnetizingBoss
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class MagnetizingBossBehavior : MonoBehaviour {


		//----------Tunable variables----------
		public float pullStrength = 10.0f; //the strength of the magnetizing force
		public float armRotSpeed = 10.0f;


		//----------Internal variables----------


		//the players, and associated variables
		private Rigidbody p1Body;
		private Rigidbody p2Body;
		private GameObject player1;
		private GameObject player2;
		private const string PLAYER_1_OBJ = "Player 1";
		private const string PLAYER_2_OBJ = "Player 2";


		//the boss' "arms"
		private Transform armAxis;
		private const string ARM_AXIS_OBJ = "Arm axis";
		private Transform rotationTarget;
		private const string ROTATION_TARGET_OBJ = "Rotation target";


		//delegate for who to aim at
		private delegate void CurrentAction();
		private CurrentAction currentAction;


		//which step are we on in the boss fight
		private enum Stage { GrabP1, SwitchToP2, GrabP2, SwitchToP1 };
		private Stage currentStage = Stage.SwitchToP1;


		private void Start(){
			player1 = GameObject.Find(PLAYER_1_OBJ);
			player2 = GameObject.Find(PLAYER_2_OBJ);
			p1Body = player1.GetComponent<Rigidbody>();
			p2Body = player2.GetComponent<Rigidbody>();
			armAxis = transform.Find(ARM_AXIS_OBJ);
			rotationTarget = transform.Find(ROTATION_TARGET_OBJ);
			currentAction = GrabPlayer1;
		}


		private void Update(){
			currentAction();
		}


		private void GrabPlayer1(){
			Vector3 forceTowardBoss = (transform.position - player1.transform.position).normalized;
			p1Body.AddForce(forceTowardBoss * pullStrength, ForceMode.Force);

			Vector3 dirTowardP1 = (player1.transform.position - transform.position).normalized * -1;
			rotationTarget.rotation = Quaternion.LookRotation(dirTowardP1);
			armAxis.rotation = Quaternion.RotateTowards(armAxis.rotation,
														rotationTarget.rotation,
														armRotSpeed * Time.deltaTime);
		}


		private void GrabPlayer2(){
			Vector3 forceTowardBoss = (transform.position - player2.transform.position).normalized;
			p2Body.AddForce(forceTowardBoss * pullStrength, ForceMode.Force);

			Vector3 dirTowardP2 = (transform.position - player2.transform.position).normalized;
			armAxis.rotation = Quaternion.RotateTowards(armAxis.rotation,
														Quaternion.Euler(dirTowardP2),
														armRotSpeed);
		}
	}
}
