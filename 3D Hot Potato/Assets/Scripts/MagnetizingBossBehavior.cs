namespace MagnetizingBoss
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class MagnetizingBossBehavior : MonoBehaviour {


		//----------Tunable variables----------
		public float pullStrength = 10.0f; //the strength of the magnetizing force
		public float armRotSpeed = 10.0f;

		//how many degrees off the arms can be before they snap to a player and start magnetizing
		public float quaternionMatchTolerance = 10.0f;


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
			currentAction = PointTowardPlayer1;
		}


		private void Update(){
			switch (currentStage){
				case Stage.GrabP1:
					currentAction = GrabPlayer1;
					break;
				case Stage.GrabP2:
					currentAction = GrabPlayer2;
					break;
				case Stage.SwitchToP1:
					currentAction = PointTowardPlayer1;
					break;
				case Stage.SwitchToP2:
					currentAction = PointTowardPlayer2;
					break;
			}

			currentAction();
		}


		private void GrabPlayer1(){
			Vector3 forceTowardBoss = (transform.position - player1.transform.position).normalized;
			p1Body.AddForce(forceTowardBoss * pullStrength, ForceMode.Force);

			Vector3 dirTowardP1 = (player1.transform.position - transform.position).normalized * -1;
	
			armAxis.rotation = Quaternion.LookRotation(dirTowardP1);
		}


		private void PointTowardPlayer1(){
			Vector3 dirTowardP1 = (player1.transform.position - transform.position).normalized * -1;
			rotationTarget.rotation = Quaternion.LookRotation(dirTowardP1);
			armAxis.rotation = Quaternion.RotateTowards(armAxis.rotation,
														rotationTarget.rotation,
														armRotSpeed * Time.deltaTime);

			if (Quaternion.Angle(armAxis.rotation, rotationTarget.rotation) < quaternionMatchTolerance){
				currentStage = Stage.GrabP1;
			}
		}


		private void GrabPlayer2(){
			Vector3 forceTowardBoss = (transform.position - player2.transform.position).normalized;
			p2Body.AddForce(forceTowardBoss * pullStrength, ForceMode.Force);

			Vector3 dirTowardP2 = Vector3.Cross((player2.transform.position - transform.position).normalized, Vector3.up);
			armAxis.rotation = Quaternion.LookRotation(dirTowardP2);
		}


		private void PointTowardPlayer2(){
			Vector3 dirTowardP2 = Vector3.Cross((player2.transform.position - transform.position).normalized, Vector3.up);
			rotationTarget.rotation = Quaternion.LookRotation(dirTowardP2);
			armAxis.rotation = Quaternion.RotateTowards(armAxis.rotation,
														rotationTarget.rotation,
														armRotSpeed * Time.deltaTime);

			if (Quaternion.Angle(armAxis.rotation, rotationTarget.rotation) < quaternionMatchTolerance){
				currentStage = Stage.GrabP2;
			}
		}


		public void PlayerBlock(char arm){
			if (currentStage == Stage.GrabP1 && arm == '1'){
				currentStage = Stage.SwitchToP2;
			} else if (currentStage == Stage.GrabP2 && arm == '2'){
				currentStage = Stage.SwitchToP1;
			}
		}
	}
}
