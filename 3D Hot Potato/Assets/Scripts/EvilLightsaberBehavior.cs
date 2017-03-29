using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvilLightsaberBehavior : MonoBehaviour {

	//----------Tunable variables----------
	public float extendDuration = 1.0f;
	public float startRadius = 2.0f;
	public float activeDuration = 3.0f;


	//----------Internal variables----------


	private Transform player1;
	private Transform player2;
	private const string PLAYER_1 = "Player 1";
	private const string PLAYER_2 = "Player 2";


	private Transform end;
	private const string END_OBJ = "LightningEnd";
	private const string BOSS_OBJ = "Lightning rod boss(Clone)";


	private float extendTimer = 0.0f;
	private float activeTimer = 0.0f;


	private LineRenderer lineRenderer;


	private int playerLayer = 11;
	private int playerLayerMask;
	private const string PLAYER_TAG = "Player";


	private EvilLightsaberBehavior centerBeam;
	private const string CENTER_BEAM_OBJ = "Center beam";

	private Transform boss;


	//used to alternatingly choose player 1 and player 2 in ChoosePlayersAlternatingly() below
	private int choosePlayer = 0;


	public void Tick(){
		extendTimer += Time.deltaTime;
		activeTimer += Time.deltaTime;

		if (activeTimer > activeDuration){
			if (gameObject.name != CENTER_BEAM_OBJ){
				BlastPlayers();
				activeTimer = 0.0f;

			}
		}
			
		lineRenderer.SetPosition(0, boss.position);
		lineRenderer.SetPosition(1, end.position);

		lineRenderer.startWidth = startRadius * (1 - (activeTimer/activeDuration));
		lineRenderer.endWidth = startRadius * (1 - (activeTimer/activeDuration));
	}


	public void Setup(){
		player1 = GameObject.Find(PLAYER_1).transform;
		player2 = GameObject.Find(PLAYER_2).transform;

		end = transform.Find(END_OBJ);

		boss = GameObject.Find(BOSS_OBJ).transform;
		 
		end.position = ChoosePlayersAlternatingly();

		lineRenderer = GetComponent<LineRenderer>();

		playerLayerMask = 1 << playerLayer;

		//scoreManager = GameObject.Find(MANAGER_OBJ).GetComponent<ScoreManager>();

		if (gameObject.name != CENTER_BEAM_OBJ){
			centerBeam = transform.Find(CENTER_BEAM_OBJ).GetComponent<EvilLightsaberBehavior>();
			centerBeam.Setup();
		}
	}


	private Vector3 ChoosePlayersAlternatingly(){
		choosePlayer++;

		if (choosePlayer%2 == 1){
			return player1.position;
		} else {
			return player2.position;
		}
	}


	private void BlastPlayers(){
		RaycastHit[] hitInfo = Physics.SphereCastAll(boss.position, 
			startRadius,
			end.position - boss.position,
			Vector3.Distance(boss.position, end.position),
			playerLayerMask,
			QueryTriggerInteraction.Ignore);

		List<PlayerEnemyInteraction> hitPlayers = new List<PlayerEnemyInteraction>();

		foreach (RaycastHit hit in hitInfo){
			if (hit.collider.tag == PLAYER_TAG){
				hitPlayers.Add(hit.collider.GetComponent<PlayerEnemyInteraction>());
			}
		}

		for (int i = hitPlayers.Count - 1; i >= 0; i--){
			hitPlayers[i].LoseTheGame();
			return;
		}

		//reposition the beam
		end.position = ChoosePlayersAlternatingly();
		lineRenderer.SetPosition(1, end.position);
	}
}
