/*
 * 
 * This script handles the ball's movement betwen players
 * 
 */

using UnityEngine;
using System.Collections;

public class BallBehavior : MonoBehaviour {


	//----------Tunable variables----------


	//speed with which the ball moves between the players
	public float speed = 2.0f;


	//the maximum height the ball will achieve as it arcs
	public float maxHeight = 5.0f;


	//height above the player where the ball rests
	public float holdHeight = 2.0f;


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



	private void Start(){
		rb = GetComponent<Rigidbody>();
		player1 = GameObject.Find(PLAYER_1_OBJ).transform;
		player2 = GameObject.Find(PLAYER_2_OBJ).transform;
		IntendedReceiver = transform; //nonsense initialization for error-checking
		ResetBall();
	}


	public void Throw(Transform throwingPlayer, Transform receivingPlayer){
		transform.parent = transform.root;

		throwingPlayer.GetComponent<PlayerBallInteraction>().BallCarrier = false;

		receivingPlayer.Find(CATCH_WARNING_OBJ).gameObject.SetActive(true);

		IntendedReceiver = receivingPlayer;

		Services.EventManager.Fire(new PassEvent(throwingPlayer.gameObject, receivingPlayer.gameObject));

		StartCoroutine(MoveBetweenPlayers(throwingPlayer, receivingPlayer));
	}


	public IEnumerator MoveBetweenPlayers(Transform throwingPlayer, Transform receivingPlayer){
		bool arrived = false;

		while (!arrived){
			if (Vector3.Distance(receivingPlayer.position, transform.position) <= speed){
				rb.MovePosition(receivingPlayer.position);
				arrived = true;
				GetCaught(receivingPlayer);
				yield break;
			}


			//calculate the height of the ball at this point in the arc
			//the ball should be at max height when it is halfway between the players
			float distBetweenPlayers = Vector3.Distance(throwingPlayer.position, receivingPlayer.position);
//			Debug.Log("distBetweenPlayers == " + distBetweenPlayers);


			Vector3 groundLoc = transform.position;
			groundLoc.y = 0.0f;
			float distToReceiver = Vector3.Distance(groundLoc, receivingPlayer.position);
//			Debug.Log("distToReceiver == " + distToReceiver);

			float currentHeight = Mathf.Sin((1 - (distToReceiver/distBetweenPlayers)) * Mathf.PI) * maxHeight;
//			Debug.Log("currentHeight == " + currentHeight);

			Vector3 nextPos = groundLoc + 
							  (receivingPlayer.position - groundLoc).normalized * speed;

			nextPos.y = currentHeight;

			rb.MovePosition(nextPos);

			yield return null;
		}

		//this coroutine shoudl always end in the if-statement above; this is a sanity check
		yield break;
	}


	public void GetCaught(Transform receivingPlayer){
		transform.parent = receivingPlayer.Find(CYCLE_OBJ);

		Debug.Log(transform.parent);

		receivingPlayer.GetComponent<PlayerBallInteraction>().BallCarrier = true;

		receivingPlayer.Find(CATCH_WARNING_OBJ).gameObject.SetActive(false);

		transform.localPosition = new Vector3(0.0f, holdHeight, 0.0f);

		IntendedReceiver = transform;
	}


	public void ResetBall(){
		GetCaught(player1);
		player2.GetComponent<PlayerBallInteraction>().BallCarrier = false;
	}
}
