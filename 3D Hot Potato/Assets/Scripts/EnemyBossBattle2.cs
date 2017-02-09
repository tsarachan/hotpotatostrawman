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

		//all of these variables are for the particle that grows to show that the weapon is powering up
		public float startLightningSize = 0.5f;
		public float maxLightningSize = 3.0f;
		public float startSparklesSize = 0.5f;
		public float maxSparklesSize = 3.0f;
		public float startRingSize = 0.35f;
		public float maxRingSize = 1.5f;
		public float startRaySize = 0.05f;
		public float maxRaySize = 1.0f;


		//variables relating to the bolts that connect the platforms to the lightning weapon
		public float startLineWidth = 0.05f;
		public float maxLineWidth = 1.0f;


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
		private GameObject lightningWeapon; //the ball that charges up
		private ParticleSystem lightningStrike; //the bolt of lightning
		private ParticleSystem lightningSplash; //the subemitter that produces "splashes" of particles on collision
		private const string LIGHTNING_WEAPON = "Lightning weapon";
		private const string LIGHTNING_STRIKE = "Lightning strike";
		private const string SPLASH = "SubEmitter0";


		//the beams that charge the lightning weapon
		private LineRenderer p1Line;
		private LineRenderer p2Line;
		private const string P1_BOLT = "Connected bolt 1";
		private const string P2_BOLT = "Connected bolt 2";


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


		//used to parent the boss battle
		private const string ENEMY_ORGANIZER = "Enemies";


		//initialize variables
		private void Start(){
			boss = transform.Find(BOSS_OBJ).gameObject;
			launchForward = transform.Find(BOSS_OBJ).Find(LAUNCH_POINT_FORWARD);
			launchRear = transform.Find(BOSS_OBJ).Find(LAUNCH_POINT_REAR);
			axis = transform.Find(AXIS);
			lightningWeapon = transform.Find(LIGHTNING_WEAPON).gameObject;
			lightningStrike = transform.Find(LIGHTNING_STRIKE).GetComponent<ParticleSystem>();
			lightningSplash = transform.Find(LIGHTNING_STRIKE).Find(SPLASH).GetComponent<ParticleSystem>();
			pixelScript = Camera.main.GetComponent<AlpacaSound.RetroPixelPro.RetroPixelPro>();
			p1Line = transform.Find(P1_BOLT).GetComponent<LineRenderer>();
			p2Line = transform.Find(P2_BOLT).GetComponent<LineRenderer>();
			P1Charging = false;
			P2Charging = false;
			projectileEnemy = Resources.Load(PROJECTILE_ENEMY) as GameObject;
			projectileHuntEnemy = Resources.Load(PROJECTILE_HUNT_ENEMY) as GameObject;
			lightningModule = lightningWeapon.transform.Find(LIGHTNING_EFFECT)
				.GetComponent<ParticleSystem>().main;
			sparklesModule = lightningWeapon.transform.Find(SPARKLES_EFFECT)
				.GetComponent<ParticleSystem>().main;
			ringModule = lightningWeapon.transform.Find(RING_EFFECT)
				.GetComponent<ParticleSystem>().main;
			rayModule = lightningWeapon.transform.Find(RAY_EFFECT)
				.GetComponent<ParticleSystem>().main;
			start = transform.position;
			end = new Vector3(transform.position.x,
				transform.position.y,
				transform.position.z - enterDistance);
			transform.parent = GameObject.Find(ENEMY_ORGANIZER).transform;
		}


		/// <summary>
		/// Update this instance.
		/// </summary>
		private void Update(){
			if (enteringScreen){
				transform.position = MoveOntoScreen();
			} else {
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
			p1Line.startWidth = Mathf.Lerp(startLineWidth, maxLineWidth, product);
			p1Line.endWidth = Mathf.Lerp(startLineWidth, maxLineWidth, product);
			p2Line.startWidth = Mathf.Lerp(startLineWidth, maxLineWidth, product);
			p2Line.endWidth = Mathf.Lerp(startLineWidth, maxLineWidth, product);

			lightningModule.startSize = Mathf.Lerp(startLightningSize, maxLightningSize, product);
			sparklesModule.startSize = Mathf.Lerp(startSparklesSize, maxSparklesSize, product);
			ringModule.startSize = Mathf.Lerp(startRingSize, maxRingSize, product);
			rayModule.startSize = Mathf.Lerp(startRaySize, maxRaySize, product);
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
			lightningStrike.Emit(numStrikes);

			if (health <= 0){
				Destroy(boss);
			}

			return health;
		}


		public override void GetDestroyed(){
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


			gameObject.SetActive(true);
		}


		public override void ShutOff(){
			gameObject.SetActive(false);
		}
	}

}
