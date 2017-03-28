namespace LightningRodBoss
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;


	public class LightningRodBossBehavior : EnemyBase {


		//----------Tunable variables----------

		public int startingHealth = 3;
		public float introDuration = 5.0f;
		public float vulnerableDuration = 3.0f;
		public float shakeSpeed = 2.0f;
		public float shakeMagnitude = 2.0f;
		public float shakeDuration = 2.0f;


		//these variables are used to bring the boss battle setpiece--the boss, platforms, and the lightning weapon--
		//onto the screen
		public float enterDistance = 5.0f; //how far the setpiece will travel on the z-axis. It should move far enough to reach (0, 0, 0).
		public float enterDuration = 2.0f;
		public AnimationCurve enterCurve;



		//----------Internal variables----------


		//the instructions that will be given to the player, with supporting variables
		private const string STEP_1 = "Use your powers";
		private const string STEP_2 = "Player 2\r\nBlast the lightning rod";
		private const string STEP_3 = "Player 1\r\nCut the boss with your beam";
		private Text instructionText;
		private const string TEXT_OBJ = "Instruction text";


		//the lightsaber, and supporting variables
		private GameObject lightsaber;
		private const string LIGHTSABER_OBJ = "Lightsaber prefab(Clone)";
		private const string PARTICLES_ORGANIZER = "Particles";
		private const string PLAYER_1_OBJ = "Player 1";


		//is the boss' shield down?
		private bool vulnerable = false;


		//the boss' current health
		private int currentHealth = 0;


		//don't shake while already shaking!
		private bool gettingHit = false;


		//this boss' parent object
		private const string ENEMY_ORGANIZER = "Enemies";


		//timers
		private float vulnerableTimer = 0.0f;


		//variables for bringing the boss onto the screen
		private bool enteringScreen = true;
		private float enterTimer = 0.0f;
		private Vector3 start;
		private Vector3 end;


		//the level manager
		private LevelManager levelManager;
		private const string MANAGER_OBJ = "Managers";


		//initialize variables
		private void Start(){
			transform.parent = GameObject.Find(ENEMY_ORGANIZER).transform;

			instructionText = GameObject.Find(TEXT_OBJ).GetComponent<Text>();
			instructionText.text = STEP_1;

			//trigger the lightsaber, to guarantee that there is one in the scene to find
			GameObject.Find(PLAYER_1_OBJ).GetComponent<CatchSandbox>().Tether();
			Debug.Log(transform.root);
			Debug.Log(transform.root.Find(PARTICLES_ORGANIZER));
			Debug.Log(transform.root.Find(PARTICLES_ORGANIZER).Find(LIGHTSABER_OBJ));
			lightsaber = transform.root.Find(PARTICLES_ORGANIZER).Find(LIGHTSABER_OBJ).gameObject;
		
			currentHealth = startingHealth;

			start = transform.position;
			end = new Vector3(transform.position.x,
				transform.position.y,
				transform.position.z - enterDistance);

			StartCoroutine(NewText(STEP_1, introDuration));

			levelManager = GameObject.Find(MANAGER_OBJ).GetComponent<LevelManager>();
		}


		private void Update(){
			if (enteringScreen){
				transform.position = MoveOntoScreen();
			} else {
				if (vulnerable){
					vulnerableTimer += Time.deltaTime;

					if (vulnerableTimer >= vulnerableDuration){
						BecomeInvulnerable();
					}
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


		public void BecomeVulnerable(){
			vulnerable = true;
			vulnerableTimer = 0.0f;
			instructionText.text = STEP_3;
		}

		private void BecomeInvulnerable(){
			vulnerable = false;
			instructionText.text = STEP_2;
		}


		/// <summary>
		/// The boss cannot be destroyed by normal means. It can only be hit when the vulnerable and the lightsaber
		/// is active (a bodge for getting hit by the lightsaber).
		/// </summary>
		public override void GetDestroyed(){
			if (vulnerable){
				if (lightsaber.activeInHierarchy){
					StartCoroutine(HitEffects());
				}
			}
		}


		private IEnumerator HitEffects(){
			if (gettingHit){
				yield break;
			} else {
				gettingHit = true;
			}

			Vector3 startPos = transform.position;
			Vector3 temp = startPos;

			BecomeInvulnerable();

			for (float i = 0; i < shakeDuration; i += Time.deltaTime){
				temp.x = startPos.x + shakeMagnitude * Mathf.Sin(shakeSpeed * Time.time);
				temp.y = startPos.y + shakeMagnitude * Mathf.Sin(shakeSpeed * Time.time);


				transform.position = temp;

				yield return null;
			}

			transform.position = startPos;

			currentHealth--;

			if (currentHealth <= 0){
				ZeroHealthEffects();
			}

			yield break;
		}


		private void ZeroHealthEffects(){
			levelManager.Hold = false;
			Debug.Log("At zero health");
		}


		private IEnumerator NewText(string text, float duration){
			float timer = 0.0f;

			for (float i = 0; i < duration; i += Time.deltaTime){
				instructionText.text = text;

				yield return null;
			}

			instructionText.text = STEP_2;

			yield break;
		}

		public override void Reset(){
			enteringScreen = true;
			enterTimer = 0.0f;
			start = transform.position;
			end = new Vector3(transform.position.x,
				transform.position.y,
				transform.position.z - enterDistance);
			levelManager = GameObject.Find(MANAGER_OBJ).GetComponent<LevelManager>();
			levelManager.Hold = true;
		}
	}
}
