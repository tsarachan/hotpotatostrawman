/*
 * 
 * This is the primary script controlling one of the boss battles.
 * 
 * In this battle, players must attack the boss by staying on a platform of their color. When both players are on
 * their platform at the same time, an attack charges. At full charge, a blast of energy strikes the boss. When the
 * boss is struck three times, it is destroyed.
 * 
 * Throughout the battle the boss sends out enemies. These enemies are shot out in a straight line, and are fired
 * in a spinning pattern.
 * 
 */
namespace BossBattle2 {
	
	using System.Collections;
	using UnityEngine;

	public class EnemyBossBattle2 : EnemyBase {


		//----------Tunable variables----------

		//these variables are used to bring the boss battle setpiece--the boss, platforms, and the lightning weapon--
		//onto the screen
		public float enterDistance = 5.0f; //how far the setpiece will travel on the z-axis. It should move far enough to reach (0, 0, 0).
		public float enterDuration = 2.0f;
		public AnimationCurve enterCurve;


		//variables relating to the lightning weapon
		public float chargeRequired = 2.0f; //how long the players have to stay in the charge areas to fire a shot at the boss
		public int numStrikes = 10; //the number of lightning particles emitted during a strike
		public float weaponOnDelay = 2.5f; //how long after the lightning blasts the weapon switches on

		//all of these variables are for the particle that grows to show that the weapon is powering up
		public float startEmission = 0.0f;
		public float maxEmission = 100.0f;


		//how quickly the charging platforms rotate
		public float axisRotationSpeed = 10.0f;


		//the number of times players have to hit the boss with the lightning
		public int health = 3;


		//variables for lowering pixel resolution as a form of feedback
		public int feedbackResolution = 75;
		public float resolutionChangeDuration = 0.5f;


		//variables relating to the enemies the boss spawns
		public float spawnRate = 1.0f; //how long between spawns
		public float launchRotation = 10.0f; //the number of degrees between spawns
		public float launchForce = 3.0f;


		//----------Internal variables----------

		//the boss, with the points where it launches projectiles
		private GameObject boss;
		private Transform launchForward;
		private Transform launchRear;
		private const string BOSS_OBJ = "Boss";
		private const string LAUNCH_POINT_FORWARD = "Launch point forward";
		private const string LAUNCH_POINT_REAR = "Launch point rear";

		//used to find the axis of the charge-up points so that they can rotate
		private Transform axis;
		private const string AXIS = "Charge-up point axis";


		//the lightning weapon's power
		private ParticleSystem lightningStrike1; //p1's bolt of lightning
		private ParticleSystem lightningStrike2; //p2's bolt of lightning
		private const string LIGHTNING_STRIKE_1 = "Lightning strike 1";
		private const string LIGHTNING_STRIKE_2 = "Lightning strike 2";
		private const string SPLASH = "SubEmitter0";


		//the particles that depict the weapon's charge
		private ParticleSystem p1ChargeParticle;
		private ParticleSystem p2ChargeParticle;
		private const string CHARGE_PARTICLE = "Charge-up particles";
		private const string CHARGE_POINT_1 = "Charge-up point 1";
		private const string CHARGE_POINT_2 = "Charge-up point 2";


		//keeps track of how long the players have been charging
		public float chargeTimer = 0.0f;


		//feedback for a successful lightning strike
		private AlpacaSound.RetroPixelPro.RetroPixelPro pixelScript;


		//the enemies that serve as projectiles
		private GameObject projectileEnemy;
		private GameObject projectileHuntEnemy;
		private const string PROJECTILE_ENEMY = "ProjectileEnemy";
		private const string PROJECTILE_HUNT_ENEMY = "ProjectileHuntEnemy";
		private float enemySpawnTimer = 0.0f;


		//booleans set to true when the players are in the correct positions
		public bool P1Charging { get; set; }
		public bool P2Charging { get; set; }


		//variables for the effects that show the weapon charging up
		private const string LIGHTNING_EFFECT = "Lightning";
		private const string SPARKLES_EFFECT = "Sparkles";
		private const string RING_EFFECT = "Ring";
		private const string RAY_EFFECT = "Ray";
		private ParticleSystem.MainModule lightningModule;
		private ParticleSystem.MainModule sparklesModule;
		private ParticleSystem.MainModule ringModule;
		private ParticleSystem.MainModule rayModule;


		//variables for bringing the boss onto the screen
		private bool enteringScreen = true;
		private float enterTimer = 0.0f;
		private Vector3 start;
		private Vector3 end;
		private Vector3 bossStartRotation = new Vector3(45.0f, 45.0f, 0.0f);
		private Vector3 axisStartRotation = new Vector3(0.0f, 0.0f, 0.0f);


		//used to parent the boss battle
		private const string ENEMY_ORGANIZER = "Enemies";


		//used to pause the level manager
		private LevelManager levelManager;
		private const string MANAGER_OBJ = "Managers";


		//initialize variables
		private void Start(){
			boss = transform.Find(BOSS_OBJ).gameObject;
			launchForward = transform.Find(BOSS_OBJ).Find(LAUNCH_POINT_FORWARD);
			launchRear = transform.Find(BOSS_OBJ).Find(LAUNCH_POINT_REAR);
			axis = transform.Find(AXIS);
			lightningStrike1 = transform.Find(AXIS).Find(CHARGE_POINT_1).Find(LIGHTNING_STRIKE_1).GetComponent<ParticleSystem>();
			lightningStrike2 = transform.Find(AXIS).Find(CHARGE_POINT_2).Find(LIGHTNING_STRIKE_2).GetComponent<ParticleSystem>();
			pixelScript = Camera.main.GetComponent<AlpacaSound.RetroPixelPro.RetroPixelPro>();
			p1ChargeParticle = transform.Find(AXIS).Find(CHARGE_POINT_1)
				.Find(CHARGE_PARTICLE).GetComponent<ParticleSystem>();
			p2ChargeParticle = transform.Find(AXIS).Find(CHARGE_POINT_2)
				.Find(CHARGE_PARTICLE).GetComponent<ParticleSystem>();
			P1Charging = false;
			P2Charging = false;
			projectileEnemy = Resources.Load(PROJECTILE_ENEMY) as GameObject;
			projectileHuntEnemy = Resources.Load(PROJECTILE_HUNT_ENEMY) as GameObject;
			start = transform.position;
			end = new Vector3(transform.position.x,
				transform.position.y,
				transform.position.z - enterDistance);
			transform.parent = GameObject.Find(ENEMY_ORGANIZER).transform;
			levelManager = GameObject.Find(MANAGER_OBJ).GetComponent<LevelManager>();
			levelManager.Hold = true;
		}


		/// <summary>
		/// Bring the battle into view, and then run the battle.
		/// </summary>
		private void Update(){
			if (enteringScreen){
				transform.position = MoveOntoScreen();

			//if the boss is active, do all the things involved in the battle
			} else if (boss.activeInHierarchy){
				axis.Rotate(Vector3.up * axisRotationSpeed * Time.deltaTime);

				//division is expensive and this battle has lots of particles, so
				//just do this operation once
				float progress = chargeTimer/chargeRequired;
		
				//change the size of the particles that depict the lightning weapon charging
				ChargeUpParticles(progress);


				//this if-statement handles charging and firing the lightning weapon
				if (P1Charging && P2Charging){
					chargeTimer += Time.deltaTime;

					if (chargeTimer >= chargeRequired){
						health = Fire();
						chargeTimer = 0.0f;
					}
				}

				//handle enemy spawns
				enemySpawnTimer += Time.deltaTime;

				boss.transform.Rotate(Vector3.up * launchRotation * Time.deltaTime);

				if (enemySpawnTimer >= spawnRate){
					LaunchProjectiles();
					enemySpawnTimer = 0.0f;
				}
			}
		}


		/// <summary>
		/// Get the location of the setpiece each frame.
		/// 
		/// When the setpiece reaches its destination, flip the [enteringScreen] bool so that the boss battle 
		/// stops moving.
		/// </summary>
		/// <returns>The position the setpiece should move to this frame.</returns>
		private Vector3 MoveOntoScreen(){
			enterTimer += Time.deltaTime;

			Vector3 pos = Vector3.LerpUnclamped(start, end, enterCurve.Evaluate(enterTimer/enterDuration));

			if (Vector3.Distance(pos, end) <= Mathf.Epsilon) { enteringScreen = false; }

			return pos;
		}


		private void ChargeUpParticles(float product){
			//here again, Unity wants these modules assigned to a variable to interact with them
			//not sure why that's necessary, but it works OK
			ParticleSystem.EmissionModule p1Module = p1ChargeParticle.emission;
			p1Module.rateOverTime = Mathf.Lerp(startEmission, maxEmission, chargeTimer/chargeRequired);

			ParticleSystem.EmissionModule p2Module = p2ChargeParticle.emission;
			p2Module.rateOverTime = Mathf.Lerp(startEmission, maxEmission, chargeTimer/chargeRequired);
		}


		private void LaunchProjectiles(){
			GameObject projectile = ObjectPooling.ObjectPool.GetObj(PROJECTILE_ENEMY);
			projectile.transform.position = launchForward.position;
			projectile.GetComponent<Rigidbody>().AddForce(boss.transform.forward * launchForce,
				ForceMode.Impulse);

			projectile = ObjectPooling.ObjectPool.GetObj(PROJECTILE_HUNT_ENEMY);
			projectile.transform.position = launchRear.position;
			projectile.GetComponent<Rigidbody>().AddForce(-boss.transform.forward * launchForce,
				ForceMode.Impulse);
		}


		private int Fire(){
			health--;
			pixelScript.SetTemporaryResolution(feedbackResolution, feedbackResolution, resolutionChangeDuration);
			lightningStrike1.Emit(numStrikes);
			lightningStrike2.Emit(numStrikes);

			if (health <= 0){
				levelManager.Hold = false;
				gameObject.SetActive(false);
			}

			return health;
		}


		public override void GetDestroyed(){
			//set the charge-up particles to stop emitting; otherwise they'll be active as the boss battle
			//comes back on scene
			ParticleSystem.EmissionModule p1Module = p1ChargeParticle.emission;
			p1Module.rateOverTime = 0.0f;

			ParticleSystem.EmissionModule p2Module = p2ChargeParticle.emission;
			p2Module.rateOverTime = 0.0f;

			levelManager.Hold = false;

			ObjectPooling.ObjectPool.AddObj(gameObject);
		}


		public override void Reset(){
			enteringScreen = true;
			enterTimer = 0.0f;
			start = transform.position;
			end = new Vector3(transform.position.x,
				transform.position.y,
				transform.position.z - enterDistance);
			P1Charging = false;
			P2Charging = false;
			enemySpawnTimer = 0.0f;
			boss = transform.Find(BOSS_OBJ).gameObject;
			boss.transform.rotation = Quaternion.Euler(bossStartRotation);
			axis = transform.Find(AXIS);
			axis.rotation = Quaternion.Euler(axisStartRotation);
			health = 3;
			chargeTimer = 0.0f;
			levelManager = GameObject.Find(MANAGER_OBJ).GetComponent<LevelManager>();
			levelManager.Hold = true;


			gameObject.SetActive(true);
		}


		public override void ShutOff(){
			gameObject.SetActive(false);
		}
	}

}
