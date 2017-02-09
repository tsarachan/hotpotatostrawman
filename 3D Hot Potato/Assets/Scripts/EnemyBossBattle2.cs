namespace BossBattle2 {
	
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class EnemyBossBattle2 : MonoBehaviour {


		//----------Tunable variables----------

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

		private GameObject boss; //the object representing the boss
		private const string BOSS_OBJ = "Boss";

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
		private const string PROJECTILE_ENEMY = "ProjectileEnemy";
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


		private void Start(){
			boss = transform.Find(BOSS_OBJ).gameObject;
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
			lightningModule = lightningWeapon.transform.Find(LIGHTNING_EFFECT)
				.GetComponent<ParticleSystem>().main;
			sparklesModule = lightningWeapon.transform.Find(SPARKLES_EFFECT)
				.GetComponent<ParticleSystem>().main;
			ringModule = lightningWeapon.transform.Find(RING_EFFECT)
				.GetComponent<ParticleSystem>().main;
			rayModule = lightningWeapon.transform.Find(RAY_EFFECT)
				.GetComponent<ParticleSystem>().main;
		}


		private void Update(){
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
				GameObject projectile = ObjectPooling.ObjectPool.GetObj(PROJECTILE_ENEMY);
				projectile.transform.position = boss.transform.position;
				projectile.GetComponent<Rigidbody>().AddForce(boss.transform.forward * launchForce,
															  ForceMode.Impulse);

				enemySpawnTimer = 0.0f;
			}
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


		private int Fire(){
			health--;
			pixelScript.SetTemporaryResolution(feedbackResolution, feedbackResolution, resolutionChangeDuration);
			lightningStrike.Emit(numStrikes);

			if (health <= 0){
				Destroy(boss);
			}

			return health;
		}
	}

}
