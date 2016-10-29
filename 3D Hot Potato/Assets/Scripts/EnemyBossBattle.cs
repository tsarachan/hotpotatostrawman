using UnityEngine;
using System.Collections;

public class EnemyBossBattle : MonoBehaviour {

	private const string ENEMY_ORGANIZER = "Enemies";

	//these variables are used to bring the boss battle setpiece--the boss and the cannon--onto the screen
	private bool enteringScreen = true;
	public float enterDistance = 5.0f; //how far the setpiece will travel on the z-axis
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

	private void Update(){
		if (enteringScreen){
			transform.position = MoveOntoScreen();
		}
	}

	private Vector3 MoveOntoScreen(){
		timer += Time.deltaTime;

		Vector3 pos = Vector3.LerpUnclamped(start, end, enterCurve.Evaluate(timer/enterTime));

		if (Vector3.Distance(pos, end) <= Mathf.Epsilon) { enteringScreen = false; }

		return pos;
	}
}
