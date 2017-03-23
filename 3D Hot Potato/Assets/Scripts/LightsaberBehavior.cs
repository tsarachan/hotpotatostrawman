using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsaberBehavior : MonoBehaviour {


	//----------Tunable variables----------
	public float extendDuration = 1.0f;
	public float startRadius = 2.0f;
	public float activeDuration = 3.0f;
	public float shieldDist = 0.1f;
	public AnimationCurve shieldFadeCurve;


	//----------Internal variables----------


	private Transform player1;
	private Transform player2;
	private const string PLAYER_1 = "Player 1";
	private const string PLAYER_2 = "Player 2";


	private Transform start;
	private Transform end;
	private const string START_OBJ = "LightningStart";
	private const string END_OBJ = "LightningEnd";


	private float extendTimer = 0.0f;
	private float activeTimer = 0.0f;


	private LineRenderer lineRenderer;


	private int enemyLayer = 9;
	private int enemyLayerMask;
	private const string ENEMY_TAG = "Enemy";


	private float textureOffset = 0.0f;


	private LightsaberBehavior centerBeam;
	private const string CENTER_BEAM_OBJ = "Center beam";


	private ScoreManager scoreManager;
	private const string MANAGER_OBJ = "Managers";


	private Transform shield;
	private const string SHIELD_OBJ = "Shield";
	public Vector3 shieldStartScale = new Vector3(5.0f, 1.0f, 0.25f);
	private Vector3 shieldEndScale = new Vector3(0.0f, 0.0f, 0.0f);


	private void Update(){
		extendTimer += Time.deltaTime;
		activeTimer += Time.deltaTime;

		if (activeTimer > activeDuration){
			extendTimer = 0.0f;
			activeTimer = 0.0f;
			gameObject.SetActive(false);
		}

		start.position = player1.position;

		end.position = Vector3.Lerp(player1.position, player2.position, extendTimer/extendDuration);

		lineRenderer.SetPosition(0, start.position);
		lineRenderer.SetPosition(1, end.position);

		textureOffset -= Time.deltaTime;

		if (textureOffset < -10.0f){
			textureOffset += 10.0f;
		}

		//lineRenderer.sharedMaterials[1].SetTextureOffset("_MainTex", new Vector2(textureOffset, 0.0f));

		lineRenderer.startWidth = startRadius * (1 - (activeTimer/activeDuration));
		lineRenderer.endWidth = startRadius * (1 - (activeTimer/activeDuration));

		if (gameObject.name != CENTER_BEAM_OBJ){
			BlastEnemies();

			Vector3 shieldDir = (player2.position - player1.position).normalized;
			shield.transform.position = player1.position + shieldDir * shieldDist;
			shield.transform.rotation = Quaternion.LookRotation(shieldDir);
			shield.transform.localScale = Vector3.Lerp(shieldStartScale,
													   shieldEndScale,
													   shieldFadeCurve.Evaluate(activeTimer/activeDuration));
		}
	}


	public void ExtendConnection(){
		start.position = player1.position;
		end.position = player2.position;

		if (gameObject.name != CENTER_BEAM_OBJ){
			centerBeam.ExtendConnection();
		}

		lineRenderer.startWidth = startRadius;
		lineRenderer.endWidth = startRadius;
		gameObject.SetActive(true);
	}


	public void Setup(){
		player1 = GameObject.Find(PLAYER_1).transform;
		player2 = GameObject.Find(PLAYER_2).transform;

		start = transform.Find(START_OBJ);
		end = transform.Find(END_OBJ);

		lineRenderer = GetComponent<LineRenderer>();

		enemyLayerMask = 1 << enemyLayer;

		scoreManager = GameObject.Find(MANAGER_OBJ).GetComponent<ScoreManager>();

		if (gameObject.name != CENTER_BEAM_OBJ){
			centerBeam = transform.Find(CENTER_BEAM_OBJ).GetComponent<LightsaberBehavior>();
			centerBeam.Setup();

			shield = transform.Find(SHIELD_OBJ);
			shield.transform.localScale = shieldStartScale;
		}
	}


	private void BlastEnemies(){
		RaycastHit[] hitInfo = Physics.SphereCastAll(start.position, 
													 startRadius,
													 end.position - start.position,
													 Vector3.Distance(player1.position, player2.position),
													 enemyLayerMask,
													 QueryTriggerInteraction.Ignore);

		List<EnemyBase> hitEnemies = new List<EnemyBase>();

		foreach (RaycastHit hit in hitInfo){
			if (hit.collider.tag == ENEMY_TAG){
				hitEnemies.Add(hit.collider.GetComponent<EnemyBase>());
			}
		}

		for (int i = hitEnemies.Count - 1; i >= 0; i--){
			DestroyEnemy(hitEnemies[i]);
		}
	}


	public void DestroyEnemy(EnemyBase enemy){
		scoreManager.AddScore(enemy.ScoreValue);
		scoreManager.IncreaseCombo();
		enemy.GetDestroyed();
	}
}
