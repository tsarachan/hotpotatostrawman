namespace CannonBoss{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class CannonBossBattle : EnemyBase {



		//----------Internal variables----------


		//the steps in the boss fight
		private enum Stages { Tutorial, Health3, Health2, Health1, Health0 };
		private Stages currentStage;


		//the boss, which will move back and forth
		private Rigidbody bossBody;
		private const string BOSS_OBJ = "Boss";

		//the cannon, which also moves back and forth
		private Rigidbody cannonBody;
		private const string CANNON_OBJ = "Cannon";


		private void Start(){
			currentStage = Stages.Tutorial;
			bossBody = transform.Find(BOSS_OBJ).GetComponent<Rigidbody>();
		}


		private void Update(){
			switch(currentStage){
				case Stages.Tutorial:
					//do nothing; during the tutorial, the cannon boss just sits there
					break;
				case Stages.Health3:
				case Stages.Health2:
					//enemies spawn
					break;
				case Stages.Health1:
					break;
				case Stages.Health0:
					break;
			}
		}


		private void FixedUpdate(){
			switch(currentStage){
			case Stages.Tutorial:
				//do nothing; during the tutorial, the cannon boss just sits there
				break;
			case Stages.Health3:
			case Stages.Health2:
				//move back and forth
				break;
			case Stages.Health1:
				break;
			case Stages.Health0:
				break;
			}
		}


//		private Vector3 BackAndForth(){
//
//		}


		public void Hit(){
			if (currentStage != Stages.Health0){
				currentStage++;
				Debug.Log(currentStage);
			}
		}
	}
}
