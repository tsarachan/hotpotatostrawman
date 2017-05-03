namespace CannonBoss{
	using System.Collections;
	using System.Collections.Generic;
	using TMPro;
	using UnityEngine;

	public class CannonBossBattle : EnemyBase {


		///////////////////////////////////////////////////////////////////////
		/// Tunable variables
		///////////////////////////////////////////////////////////////////////


		//gameplay-related
		[SerializeField] private float bossMoveDist = 5.0f; //how far left and right the boss moves
		[SerializeField] private float bossMoveSpeed = 1.0f; //how quickly the boss moves
		[SerializeField] private float spawnDelay = 5.0f; //how long between waves of enemies
		[SerializeField] private float bossSpeedMult = 1.5f; //how much faster the boss gets after 2 hits
		[SerializeField] private float cannonMoveDist = 5.0f; //how far forward and back the cannon moves
		[SerializeField] private float cannonMoveSpeed = 1.0f; //how quickly the cannon moves
		[SerializeField] private float bossAccelDuration = 0.5f; //how long it takes the boss to speed up after 2 hits


		//entering onto the playfield
		[SerializeField] private float enterDist = 130.0f;
		[SerializeField] private float enterDuration = 1.5f;
		[SerializeField] private AnimationCurve enterCurve;


		//how long to wait before going to the high score scene after victory
		[SerializeField] private float victoryDelay = 3.0f;


		///////////////////////////////////////////////////////////////////////
		/// Non-tunable variables
		///////////////////////////////////////////////////////////////////////


		//the steps in the boss fight
		private enum Stages { Entering, Tutorial, Health4, Health3, Health2, Health1, Health0 };
		private Stages currentStage;


		//the boss, which will move back and forth
		private Rigidbody bossBody;
		private const string BOSS_OBJ = "Boss";


		//the boss' starting location; used to help move the boss back and forth
		private Vector3 bossStartPos = new Vector3(0.0f, 0.0f, 0.0f);


		//the cannon, which also moves back and forth
		private Rigidbody cannonBody;
		private const string CANNON_OBJ = "Cannon";


		//the cannon's starting location; used to move the cannon back and forth
		private Vector3 cannonStartPos = new Vector3(0.0f, 0.0f, 0.0f);


		//variables for moving back and forth
		private float moveTimer = 0.0f;
		private float cannonTimer = 0.0f;
		private float currentBossMoveSpeed = 0.0f;
		private float currentCannonMoveSpeed = 0.0f;
		private float acceleratedBossMoveSpeed = 0.0f;
		private float bossAccelTimer = 0.0f;


		//the enemies spawned during this boss battle
		private const string BOSS_SUPPORTER_OBJ = "PlayerTrackerEnemy";
		private List<Transform> supporterSpawners = new List<Transform>();
		private List<string> easySpawners = new List<string> { "4", "12" };
		private List<string> difficultSpawners = new List<string>() { "4", "6", "8", "10", "12" };
		private const string SPAWNER_OBJ = "Spawner ";
		private float spawnTimer = 0.0f;


		//this transform's parent
		private const string ENEMY_ORGANIZER = "Enemies";


		//timer for entering the play area
		private float enterTimer = 0.0f;
		private Vector3 startPos = new Vector3(0.0f, 0.0f, 0.0f);
		private Vector3 endPos = new Vector3(0.0f, 0.0f, 0.0f);


		//the LevelManager, so that it can be shut off
		private LevelManager levelManager;
		private const string MANAGER_OBJ = "Managers";


		//particle when the boss is destroyed
		private const string BOSS_DESTROYED_PARTICLE = "Boss destroyed particle";


		//the GamEndSystem, to go to the high score screen after winning, with associated variables
		private GameEndSystem gameEndSystem;
		private const string GAME_END_FUNC = "EndGame";
		private TextMeshProUGUI tutorialText1;
		private const string TUTORIAL_TEXT_OBJ = "Tutorial text 1";
		private const string CONGRATULATIONS_MESSAGE = "Congratulations!\nThanks for playing the demo!";



		///////////////////////////////////////////////////////////////////////
		/// Setup
		///////////////////////////////////////////////////////////////////////

		//initialize variables
		private void Start(){
			currentStage = Stages.Entering;
			bossBody = transform.Find(BOSS_OBJ).GetComponent<Rigidbody>();
			cannonBody = transform.Find(CANNON_OBJ).GetComponent<Rigidbody>();
			supporterSpawners = GetSpawners();
			transform.parent = GameObject.Find(ENEMY_ORGANIZER).transform;
			acceleratedBossMoveSpeed = bossMoveSpeed * bossSpeedMult;
			startPos = transform.position;
			endPos = startPos;
			endPos.z -= enterDist;
			levelManager = GameObject.Find(MANAGER_OBJ).GetComponent<LevelManager>();
			gameEndSystem = GameObject.Find(MANAGER_OBJ).GetComponent<GameEndSystem>();
		}


		private List<Transform> GetSpawners(){
			List<Transform> temp = new List<Transform>();
			List<string> currentSpawners = GetCurrentSpawnList();


			foreach (string spawner in currentSpawners){
				temp.Add(GameObject.Find(SPAWNER_OBJ + spawner).transform);
			}

			Debug.Assert(temp.Count == currentSpawners.Count);

			return temp;
		}


		private List<string> GetCurrentSpawnList(){
			if (currentStage == Stages.Entering){
				return easySpawners;
			} else {
				return difficultSpawners;
			}
		}


		///////////////////////////////////////////////////////////////////////
		/// State machine that controls the boss battle
		///////////////////////////////////////////////////////////////////////


		//handle spawning other enemies
		private void Update(){
			switch(currentStage){
				case Stages.Entering:
				case Stages.Tutorial:
					//do nothing; the boss doesn't spawn enemies in these stages
					break;
				case Stages.Health4:
				case Stages.Health3:
					spawnTimer = HandleSpawning();
					break;
				case Stages.Health2:
				case Stages.Health1:
					spawnTimer = HandleSpawning();
					break;
				case Stages.Health0:
					break;
			}
		}


		//handle the boss' and cannon's movement
		private void FixedUpdate(){
			switch(currentStage){
				case Stages.Entering:
					transform.position = EnterOnScreen();

					if (enterTimer >= enterDuration){
						currentStage++;
					}
					break;
				case Stages.Tutorial:
					//do nothing; during the tutorial, the cannon boss just sits there
					break;
				case Stages.Health4:
				case Stages.Health3:
					moveTimer = HandleBossMovement();
					break;
				case Stages.Health2:
				case Stages.Health1:
					currentBossMoveSpeed = GetBossMoveSpeed();
					moveTimer = HandleBossMovement();
					cannonTimer = HandleCannonMovement();
					break;
				case Stages.Health0:
					break;
			}
		}


		///////////////////////////////////////////////////////////////////////
		/// Movement
		///////////////////////////////////////////////////////////////////////


		private Vector3 EnterOnScreen(){
			enterTimer += Time.deltaTime;

			return Vector3.Lerp(startPos, endPos, enterCurve.Evaluate(enterTimer/enterDuration));
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


		/// <summary>
		/// Move the boss left and right
		/// </summary>
		/// <returns>The boss' new position.</returns>
		private Vector3 LeftAndRight(float time){
			Vector3 temp = bossStartPos;
			temp.x += Mathf.Sin(time * currentBossMoveSpeed) * bossMoveDist;

			return temp;
		}


		private float GetBossMoveSpeed(){
			if (bossAccelTimer < bossAccelDuration){
				bossAccelTimer += Time.deltaTime;

				return Mathf.Lerp(bossMoveSpeed, acceleratedBossMoveSpeed, bossAccelTimer/bossAccelDuration);
			} else {
				return acceleratedBossMoveSpeed;
			}
		}


		private float HandleCannonMovement(){
			float temp = cannonTimer;
			temp += Time.deltaTime;

			cannonBody.MovePosition(ForwardAndBack(temp));

			return temp;
		}


		private Vector3 ForwardAndBack(float time){
			Vector3 temp = cannonStartPos;
			temp.z += Mathf.Sin(time * cannonMoveSpeed) * cannonMoveDist;

			return temp;
		}


		///////////////////////////////////////////////////////////////////////
		/// Taking damage
		///////////////////////////////////////////////////////////////////////


		public void Hit(){
			if (currentStage != Stages.Health0){
				currentStage++;

				if (currentStage == Stages.Health4){ //moving out of the tutorial; set variables
					bossStartPos = bossBody.transform.position;
				} else if (currentStage == Stages.Health2){ //moving into the more difficult part of the battle
					cannonStartPos = cannonBody.transform.position;
					bossMoveSpeed *= bossSpeedMult; //the boss speeds up
					supporterSpawners = GetSpawners(); //spawn more enemies
				} else if (currentStage == Stages.Health0){
					levelManager.StopGame();
				}
			}
		}


		///////////////////////////////////////////////////////////////////////
		/// Object pooling
		///////////////////////////////////////////////////////////////////////


		public override void GetDestroyed(){
			if (currentStage == Stages.Health0){
				GameObject destroyParticle = ObjectPooling.ObjectPool.GetObj(BOSS_DESTROYED_PARTICLE);
				destroyParticle.transform.position = bossBody.transform.position;
				tutorialText1.text = CONGRATULATIONS_MESSAGE;
				Invoke(GAME_END_FUNC, victoryDelay);
			}

			ObjectPooling.ObjectPool.AddObj(gameObject);
		}


		private void EndGame(){
			gameEndSystem.VoluntaryStop();
		}


		public override void Reset(){
			currentStage = Stages.Entering;
			bossBody = transform.Find(BOSS_OBJ).GetComponent<Rigidbody>();
			cannonBody = transform.Find(CANNON_OBJ).GetComponent<Rigidbody>();
			supporterSpawners = GetSpawners();

			bossBody.transform.localPosition = new Vector3(0.0f, 0.0f, 28.3f);
			cannonBody.transform.localPosition = new Vector3(0.0f, 0.0f, 5.0f);

			enterTimer = 0.0f;
			startPos = transform.position;
			endPos = startPos;
			endPos.z -= enterDist;

			currentBossMoveSpeed = bossMoveSpeed;
			currentCannonMoveSpeed = cannonMoveSpeed;

			levelManager = GameObject.Find(MANAGER_OBJ).GetComponent<LevelManager>();
			levelManager.Hold = true;

			gameEndSystem = GameObject.Find(MANAGER_OBJ).GetComponent<GameEndSystem>();

			tutorialText1 = GameObject.Find(TUTORIAL_TEXT_OBJ).GetComponent<TextMeshProUGUI>();

			gameObject.SetActive(true);
		}


		public override void ShutOff(){
			gameObject.SetActive(false);
		}
	}
}
