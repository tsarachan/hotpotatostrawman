using UnityEngine;
using System.Collections;

public class EnemyDestroyWave : ObjectPooling.Poolable {

	private const string ENEMY_ORGANIZER = "Enemies";

	private void Start(){
		ClearEnemies();
		ObjectPooling.ObjectPool.AddObj(gameObject); //shut this off and return it to the pool
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
