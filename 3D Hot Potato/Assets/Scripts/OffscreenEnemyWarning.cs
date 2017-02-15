using System.Collections;
using UnityEngine;

public class OffscreenEnemyWarning : MonoBehaviour {

	public float maxDistance = 100.0f;

	private int enemyLayer = 9;
	private int enemyLayerMask;

	private Transform arrow;
	private const string ARROW_SPRITE = "Arrow";


	private void Start(){
		enemyLayerMask = 1 << enemyLayer;
		arrow = transform.Find(ARROW_SPRITE);
	}


	private void Update(){
		if (Physics.Raycast(transform.position, transform.forward, maxDistance, enemyLayerMask)){
			arrow.gameObject.SetActive(true);
		} else {
			arrow.gameObject.SetActive(false);
		}
	}
}
