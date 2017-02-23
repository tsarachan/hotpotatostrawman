using System.Collections;
using UnityEngine;

public class OffscreenEnemyWarning : MonoBehaviour {

	public float maxDistance = 100.0f;
	public float warningTime = 1.0f; //how long the arrow should stay visible when it detects something
	public float detectMultiplier = 2.0f;

	private float timer = 0.0f;

	private int enemyLayer = 9;
	private int enemyLayerMask;

	private Transform arrow;
	private const string ARROW_SPRITE = "Arrow";

	public Vector3 distToSpawner = new Vector3(68.5f, 0.0f, 0.0f);


	private void Start(){
		enemyLayerMask = 1 << enemyLayer;
		arrow = transform.Find(ARROW_SPRITE);
	}


	private void Update(){
		if (Physics.Raycast(transform.position + transform.forward * detectMultiplier, transform.forward, maxDistance, enemyLayerMask)){
			timer = warningTime;	
		}


		//run the timer to limit how long the arrow appears
		timer -= Time.deltaTime;

		if (timer > 0.0f){
			arrow.gameObject.SetActive(true);
		} else {
			arrow.gameObject.SetActive(false);
		}
	}
}
