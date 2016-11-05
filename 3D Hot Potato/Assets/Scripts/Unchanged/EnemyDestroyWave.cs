using UnityEngine;
using System.Collections;

public class EnemyDestroyWave : MonoBehaviour {

	private const string ENEMY_ORGANIZER = "Enemies";

	private void Start(){
		ClearEnemies();
		Destroy(gameObject);
	}

	private void ClearEnemies(){
		Transform enemies = GameObject.Find(ENEMY_ORGANIZER).transform;

		foreach (Transform enemy in enemies){
			if (enemy.GetComponent<EnemyBase>() != null){
				enemy.GetComponent<EnemyBase>().GetDestroyed();
			}
		}
	}
}
