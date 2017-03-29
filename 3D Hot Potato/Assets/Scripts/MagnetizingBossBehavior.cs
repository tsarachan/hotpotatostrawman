namespace MagnetizingBoss
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class MagnetizingBossBehavior : MonoBehaviour {


		//----------Tunable variables----------
		public float pullStrength = 10.0f; //the strength of the magnetizing force
		public float pushStrength = 20.0f;
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
		private enum Stage { GrabP1, SwitchToP2, GrabP2, SwitchToP1, Vulnerable };
		private Stage currentStage = Stage.SwitchToP1;


		private void Start(){
			player1 = GameObject.Find(PLAYER_1_OBJ);
			player2 = GameObject.Find(PLAYER_2_OBJ);
			p1Body = player1.GetComponent<Rigidbody>();
			p2Body = player2.GetComponent<Rigidbody>();
			armAxis = transform.Find(ARM_AXIS_OBJ);
			rotationTarget = transform.Find(ROTATION_TARGET_OBJ);
			currentStage = Stage.Vulnerable;
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
					PushPlayers();
					break;
				case Stage.SwitchToP2:
					currentAction = PointTowardPlayer2;
					PushPlayers();
					break;
				case Stage.Vulnerable:
					currentAction = HoldArmsNeutral;
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


		private void PushPlayers(){
			Vector3 dirAwayFromP1 = (player1.transform.position - transform.position).normalized;
			Vector3 dirAwayFromP2 = (player2.transform.position - transform.position).normalized;

			p1Body.AddForce(dirAwayFromP1 * pushStrength, ForceMode.Force);
			p2Body.AddForce(dirAwayFromP2 * pushStrength, ForceMode.Force);
		}


		private void HoldArmsNeutral(){
			Vector3 armsAkimbo = Quaternion.Euler(0.0f, 45.0f, 0.0f) * Vector3.forward;
			rotationTarget.rotation = Quaternion.LookRotation(armsAkimbo);
			armAxis.rotation = Quaternion.RotateTowards(armAxis.rotation,
														rotationTarget.rotation,
														armRotSpeed * Time.deltaTime);
		}


		public void PlayerBlock(char contactPoint){
			if (currentStage == Stage.GrabP1 && contactPoint == '1'){
				currentStage = Stage.SwitchToP2;
			} else if (currentStage == Stage.GrabP2 && contactPoint == '2'){
				currentStage = Stage.Vulnerable;
			} else if (currentStage == Stage.Vulnerable && contactPoint == 'n'){
				currentStage = Stage.SwitchToP1;
			}
		}
	}
}
