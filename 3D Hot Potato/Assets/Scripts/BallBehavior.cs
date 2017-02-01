﻿/*
 * 
 * This script handles the ball's movement betwen players
 * 
 */

using UnityEngine;
using System.Collections;

public class BallBehavior : MonoBehaviour {

	protected Rigidbody rb;

	public AnimationCurve normalTossCurve; //can be used to make the ball move faster at start then lose speed over time, etc.
	public float flightTimePerUnitDistance = 0.1f; //effectively speed
	public AnimationCurve verticalCurve; //used to give the ball a "lob" effect
	public float verticalHeight = 2.0f; //height the ball reaches at the apex of the lob

	public float verticalOffset = 2.0f; //how high above a player the ball is held

	protected const string PLAYER_OBJ = "Player";

	protected const string SCENE_ORGANIZER = "Scene";
	protected Transform scene;

	protected Coroutine co;
	public Coroutine Co{
		get { return co; }
	}

	protected const string SUPER_METER = "Super meter";
	protected PowerUp powerUpScript;

	protected AudioSource audioSource;


	//used to reset the ball when the game starts
	private Vector3 myStartPos = new Vector3(0.0f, 0.0f, 0.0f);

	protected virtual void Start(){
		rb = GetComponent<Rigidbody>();
		scene = GameObject.Find(SCENE_ORGANIZER).transform;
		audioSource = GetComponent<AudioSource>();
		myStartPos = transform.position;
	}

	/// <summary>
	/// Players should call this function to pass between players.
	/// </summary>
	/// <param name="start">The ball's current position.</param>
	/// <param name="destination">The catching player.</param>
	public virtual void Pass(Vector3 start, Transform destination){
		transform.parent = scene; //stop being a child of the ball carrier, so that the ball can move between players
		co = StartCoroutine(PassBetweenPlayers(start, destination));

		//play a sound when the ball is thrown, unless the passes are happening fast enough that the sounds would overlap
		if (!audioSource.isPlaying){
			audioSource.Play();
		}
	}

	/// <summary>
	/// This coroutine moves the ball between the players.
	/// </summary>
	/// <param name="start">The ball's location as the throw begins.</param>
	/// <param name="destination">The catching player.</param>
	public virtual IEnumerator PassBetweenPlayers(Vector3 start, Transform destination){
		float totalFlightTime = Vector3.Distance(start, destination.position)/flightTimePerUnitDistance;
		float timer = 0.0f;

		while (timer <= totalFlightTime){
			//update totalFlightTime every frame, so that the ball moves faster or slower depending on the distance
			//this should also avoid a bug wherein the ball gets stuck at a high elevation
			totalFlightTime = Vector3.Distance(start, destination.position)/flightTimePerUnitDistance;
			timer += Time.deltaTime;

			Vector3 nextPoint = Vector3.Lerp(start,
											 destination.position,
											 normalTossCurve.Evaluate(timer/totalFlightTime));

			//make the ball arc between the elevated points where the players hold the ball
			//note that the players' colliders have to be tall enough to "catch" the ball up high
			nextPoint.y = Mathf.Lerp(start.y, start.y + verticalHeight, verticalCurve.Evaluate(timer/totalFlightTime));

			rb.MovePosition(nextPoint);

			yield return null;
		}

		FallToGround(destination.position);

		Debug.Log("Coroutine stopped: y == " + transform.position.y);
		yield break;
	}


	/// <summary>
	/// This is a brute-force solution to the bug wherein the ball stops in the air. It forces the ball to the ground.
	/// </summary>
	/// <param name="destination">The world space point where the ball is meant to stop.</param>
	public virtual void FallToGround(Vector3 destination){
		if (destination.y != 0.0f){
			destination.y = 0.0f;
		}

		rb.MovePosition(destination);
	}

	/// <summary>
	/// Players call this when they catch or pick up the ball.
	/// </summary>
	/// <param name="catchingPlayer">The player interacting with the ball.</param>
	public void GetCaught(Transform catchingPlayer){
		if (Co != null){
			StopCoroutine(Co);
		}

		transform.parent = catchingPlayer;

		//changes to the player model may require changing the offset axis here
		transform.localPosition = new Vector3(0.0f, verticalOffset, 0.0f);
	}


	public void ResetBall(){
		transform.position = myStartPos;
	}
}
