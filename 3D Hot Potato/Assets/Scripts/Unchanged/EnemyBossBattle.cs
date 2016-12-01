using UnityEngine;
using System;
using System.Collections;

public class EnemyBossBattle : EnemyBase {

	private const string ENEMY_ORGANIZER = "Enemies";

	//these variables are used to bring the boss battle setpiece--the boss and the cannon--onto the screen
	private bool enteringScreen = true;
	public float enterDistance = 5.0f; //how far the setpiece will travel on the z-axis. It should move far enough to reach (0, 0, 0).
	public float enterTime = 2.0f;
	private float timer = 0.0f;
	public AnimationCurve enterCurve;
	private Vector3 start;
	private Vector3 end;

	private void Start(){
		transform.parent = GameObject.Find(ENEMY_ORGANIZER).transform;
		start = transform.position;
		end = new Vector3(transform.position.x,
						  transform.position.y,
						  transform.position.z - enterDistance);
	}

	//bring the setpiece onto the screen; stop when it's fully on screen
	private void Update(){
		if (enteringScreen){
			transform.position = MoveOntoScreen();
		}
	}

	/// <summary>
	/// Get the location of the setpiece each frame.
	/// 
	/// When the setpiece reaches its destination, flip the [enteringScreen] bool so that the boss battle stops moving.
	/// </summary>
	/// <returns>The position the setpiece should move to this frame.</returns>
	private Vector3 MoveOntoScreen(){
		timer += Time.deltaTime;

		Vector3 pos = Vector3.LerpUnclamped(start, end, enterCurve.Evaluate(timer/enterTime));

		if (Vector3.Distance(pos, end) <= Mathf.Epsilon) { enteringScreen = false; }

		return pos;
	}

	/// <summary>
	/// When the boss battle ends, the level is over. Destroy all existing enemies, and clear all enemies still to be spawned.
	/// </summary>
	public override void GetDestroyed(){
		Instantiate(Resources.Load("ClearEnemies"), transform.position, Quaternion.identity); //get rid of all existing enemies

		//clear the arrays used to spawn enemies, so that no more spawn
		GameObject.Find("Enemy spawners").GetComponent<SpawnEnemies>().StopSpawning();

		Destroy(gameObject); //get rid of the boss battle setpiece--the boss and the cannon

	}


	/// <summary>
	/// Special Reset function that doesn't reference a rigidbody, since this is the setpiece rather than the enemies themselves
	/// </summary>
	public override void Reset(){
		gameObject.SetActive(true);
	}
}
