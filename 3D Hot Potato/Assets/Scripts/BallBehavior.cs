/*
 * 
 * This script handles the ball's movement betwen players
 * 
 */

using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class BallBehavior : MonoBehaviour {


	//----------Tunable variables----------


	//speed with which the ball moves between the players
	public float speed = 2.0f;


	//the maximum height the ball will achieve as it arcs
	public float maxHeight = 5.0f;


	//height above the player where the ball rests
	public float holdHeight = 2.0f;


	//the distance between ball and player where a special catch becomes possible; must match value in CatchBehavior
	public float awesomeCatchDist = 10.0f;


	//----------Internal variables----------


	//components
	private Rigidbody rb;


	//used to re-parent the ball after a catch
	private const string CYCLE_OBJ = "Cycle and rider";


	//other scripts use this to determine who will catch a ball in the air
	public Transform IntendedReceiver { get; set; }


	//the players
	private Transform player1;
	private Transform player2;
	private const string PLAYER_1_OBJ = "Player 1";
	private const string PLAYER_2_OBJ = "Player 2";


	//the notifications above the players
	private const string CATCH_WARNING_OBJ = "Catch warning";


	//the spark that shows when the ball is in range for an awesome catch
	private GameObject awesomeParticle;
	private const string AWESOME_PARTICLE_OBJ = "Ball particle";
	private bool awesomeCatchReady = false;
	public bool AwesomeCatchReady{
		get { return awesomeCatchReady; }
		set {
			if (value != awesomeCatchReady){
				awesomeCatchReady = value;

				if (awesomeCatchReady == true){
					Services.EventManager.Fire(new PowerReadyEvent());
				}
			}
		}
	}


	//the special catch indicators for each player
	private Text p1CatchText;
	private Text p2CatchText;
	private GameObject p1CatchObj;
	private GameObject p2CatchObj;
	private const string TEXT_CANVAS = "Catch warning";
	private const string TEXT_OBJ = "Text";
	private const string MISS = "X";
	private const string CATCH = "OK";
	private const string INCOMING = "!";


	//BallEnemyInteraction uses this to decide if the ball can destroy enemies
	public bool InAir { get; private set; }



	private void Start(){
		rb = GetComponent<Rigidbody>();
		player1 = GameObject.Find(PLAYER_1_OBJ).transform;
		player2 = GameObject.Find(PLAYER_2_OBJ).transform;
		IntendedReceiver = transform; //nonsense initialization for error-checking
		awesomeParticle = transform.Find(AWESOME_PARTICLE_OBJ).gameObject;
		p1CatchObj = player1.transform.Find(TEXT_CANVAS).gameObject;
		p2CatchObj = player2.transform.Find(TEXT_CANVAS).gameObject;
		p1CatchText = p1CatchObj.transform.Find(TEXT_OBJ).GetComponent<Text>();
		p2CatchText = p2CatchObj.transform.Find(TEXT_OBJ).GetComponent<Text>();
		ResetBall();
	}


	public void Throw(Transform throwingPlayer, Transform receivingPlayer){
		transform.parent = transform.root;

		throwingPlayer.GetComponent<PlayerBallInteraction>().BallCarrier = false;

		receivingPlayer.Find(CATCH_WARNING_OBJ).gameObject.SetActive(true);

		IntendedReceiver = receivingPlayer;

		InAir = true;

		Services.EventManager.Fire(new PassEvent(throwingPlayer.gameObject, receivingPlayer.gameObject));

		StartCoroutine(MoveBetweenPlayers(throwingPlayer, receivingPlayer));
	}


	public IEnumerator MoveBetweenPlayers(Transform throwingPlayer, Transform receivingPlayer){
		bool arrived = false;

		while (!arrived){
			//calculate the 2D distance between the ball and the receiver
			Vector3 groundLoc = transform.position;
			groundLoc.y = 0.0f;
			float distToReceiver = Vector3.Distance(groundLoc, receivingPlayer.position);


			//if the 2D distance is less than the speed of the ball, the ball will arrive this frame
			//give the ball to the receiver, so the ball doesn't flicker around the receiver
			if (distToReceiver <= speed){
				rb.MovePosition(receivingPlayer.position);
				arrived = true;
				GetCaught(receivingPlayer);
				yield break;
			}


			//the ball isn't getting caught this frame; should the awesome catch particle turn on?
			if (Vector3.Distance(receivingPlayer.position, transform.position) <= awesomeCatchDist){
				AwesomeCatchReady = true;
				awesomeParticle.SetActive(AwesomeCatchReady);
			}


			//calculate the height of the ball at this point in the arc
			//the ball should be at max height when it is halfway between the players
			float distBetweenPlayers = Vector3.Distance(throwingPlayer.position, receivingPlayer.position);


			float currentHeight = Mathf.Sin((1 - (distToReceiver/distBetweenPlayers)) * Mathf.PI) * maxHeight;


			//place the ball
			Vector3 nextPos = groundLoc + 
							  (receivingPlayer.position - groundLoc).normalized * speed;

			nextPos.y = currentHeight;

			rb.MovePosition(nextPos);

			yield return null;
		}

		//this coroutine should always end in the if-statement above; this is a sanity check
		yield break;
	}


	public void GetCaught(Transform receivingPlayer){
		transform.parent = receivingPlayer.Find(CYCLE_OBJ);

		receivingPlayer.GetComponent<PlayerBallInteraction>().BallCarrier = true;

		transform.localPosition = new Vector3(0.0f, holdHeight, 0.0f);

		IntendedReceiver = transform;

		InAir = false;

		receivingPlayer.GetComponent<CatchBehavior>().CheckIfAwesomeCatch();

		ResetCatchTexts();

		AwesomeCatchReady = false;
		awesomeParticle.SetActive(AwesomeCatchReady);
	}


	public void ResetBall(){
		GetCaught(player1);
		player2.GetComponent<PlayerBallInteraction>().BallCarrier = false;
		awesomeParticle.SetActive(false);
		InAir = false;

		ResetCatchTexts();
	}


	private void ResetCatchTexts(){
		p1CatchText.text = INCOMING;
		p1CatchText.color = Color.white;
		p2CatchText.text = INCOMING;
		p2CatchText.color = Color.white;

		p1CatchObj.SetActive(false);
		p2CatchObj.SetActive(false);
	}
}
