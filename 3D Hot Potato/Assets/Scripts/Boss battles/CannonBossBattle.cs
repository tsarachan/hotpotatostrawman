namespace CannonBoss{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class CannonBossBattle : EnemyBase {


		///////////////////////////////////////////////////////////////////////
		/// Tunable variables
		///////////////////////////////////////////////////////////////////////

		[SerializeField] private float bossMoveDist = 5.0f; //how far left and right the boss moves
		[SerializeField] private float spawnDelay = 5.0f; //how long between waves of enemies


		///////////////////////////////////////////////////////////////////////
		/// Non-tunable variables
		///////////////////////////////////////////////////////////////////////


		//the steps in the boss fight
		private enum Stages { Tutorial, Health3, Health2, Health1, Health0 };
		private Stages currentStage;


		//the boss, which will move back and forth
		private Rigidbody bossBody;
		private const string BOSS_OBJ = "Boss";


		//the boss' starting location; used to help move the boss back and forth
		private Vector3 bossStartPos = new Vector3(0.0f, 0.0f, 0.0f);

		//the cannon, which also moves back and forth
		private Rigidbody cannonBody;
		private const string CANNON_OBJ = "Cannon";


		//timer for moving back and forth
		private float moveTimer = 0.0f;


		//the enemies spawned during this boss battle
		private const string BOSS_SUPPORTER_OBJ = "PlayerTrackerEnemy";
		private List<Transform> supporterSpawners = new List<Transform>();
		private List<string> spawners = new List<string>() { "4", "6", "8", "10", "12" };
		private const string SPAWNER_OBJ = "Spawner ";
		private float spawnTimer = 0.0f;


		///////////////////////////////////////////////////////////////////////
		/// Setup
		///////////////////////////////////////////////////////////////////////

		//initialize variables
		private void Start(){
			currentStage = Stages.Tutorial;
			bossBody = transform.Find(BOSS_OBJ).GetComponent<Rigidbody>();
			supporterSpawners = GetSpawners();
		}


		private List<Transform> GetSpawners(){
			List<Transform> temp = new List<Transform>();

			foreach (string spawner in spawners){
				temp.Add(GameObject.Find(SPAWNER_OBJ + spawner).transform);
			}

			Debug.Assert(temp.Count == spawners.Count);

			return temp;
		}


		///////////////////////////////////////////////////////////////////////
		/// State machine that controls the boss battle
		///////////////////////////////////////////////////////////////////////


		private void Update(){
			switch(currentStage){
				case Stages.Tutorial:
					//do nothing; during the tutorial, the cannon boss just sits there
					break;
				case Stages.Health3:
				case Stages.Health2:
					spawnTimer = HandleSpawning();
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
					moveTimer = HandleBossMovement();
					break;
				case Stages.Health1:
					break;
				case Stages.Health0:
					break;
			}
		}


		///////////////////////////////////////////////////////////////////////
		/// Other functions
		///////////////////////////////////////////////////////////////////////


		/// <summary>
		/// Move the boss left and right
		/// </summary>
		/// <returns>The boss' new position.</returns>
		private Vector3 LeftAndRight(float time){
			Vector3 temp = bossStartPos;
			temp.x += Mathf.Sin(time) * bossMoveDist;

			return temp;
		}


		private float HandleSpawning(){
			float temp = spawnTimer;
			temp += Time.deltaTime;

			if (temp >= spawnDelay){
				foreach (Transform spawner in supporterSpawners){
					GameObject obj = ObjectPooling.ObjectPool.GetObj(BOSS_SUPPORTER_OBJ);
					obj.transform.position = spawner.position;
					obj.GetComponent<ObjectPooling.Poolable>().Reset();
				}

				temp = 0.0f;
			}

			return temp;
		}


		private float HandleBossMovement(){
			float temp = moveTimer;
			temp += Time.deltaTime;

			bossBody.MovePosition(LeftAndRight(temp));

			return temp;
		}


		public void Hit(){
			if (currentStage != Stages.Health0){
				currentStage++;

				if (currentStage == Stages.Health3){ //moving out of the tutorial; set variables
					bossStartPos = bossBody.transform.position;
				}
				Debug.Log(currentStage);
			}
		}
	}
}
