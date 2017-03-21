using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsaberBehavior : MonoBehaviour {


	//----------Tunable variables----------
	public float extendDuration = 1.0f;
	public float radius = 2.0f;
	public float activeDuration = 3.0f;


	//----------Internal variables----------
	private bool active = false;

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


	private LineRenderer colorLineRenderer;
	private LineRenderer centerLineRenderer;
	private const string COLOR_LINE_OBJ = "Color line";
	private const string CENTER_LINE_OBJ = "Center line";


	private int enemyLayer = 9;
	private int enemyLayerMask;
	private const string ENEMY_TAG = "Enemy";


	private void Update(){
		if (active){
			extendTimer += Time.deltaTime;
			activeTimer += Time.deltaTime;

			if (activeTimer > activeDuration){
				extendTimer = 0.0f;
				activeTimer = 0.0f;
				active = false;
			}

			start.position = player1.position;

			end.position = Vector3.Lerp(player1.position, player2.position, extendTimer/extendDuration);

			colorLineRenderer.SetPosition(0, start.position);
			colorLineRenderer.SetPosition(1, end.position);

			centerLineRenderer.SetPosition(0, start.position);
			centerLineRenderer.SetPosition(1, end.position);

			BlastEnemies();
		}
	}


	public void ExtendConnection(){
		active = true;
		start.position = player1.position;
		end.position = player2.position;
	}


	public void Setup(){
		player1 = GameObject.Find(PLAYER_1).transform;
		player2 = GameObject.Find(PLAYER_2).transform;

		start = transform.Find(START_OBJ);
		end = transform.Find(END_OBJ);

		colorLineRenderer = transform.Find(COLOR_LINE_OBJ).GetComponent<LineRenderer>();
		centerLineRenderer = transform.Find(CENTER_LINE_OBJ).GetComponent<LineRenderer>();

		enemyLayerMask = 1 << enemyLayer;
	}


	private void BlastEnemies(){
		RaycastHit[] hitInfo = Physics.SphereCastAll(start.position, 
													 radius,
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
			hitEnemies[i].GetDestroyed();
		}
	}
}
