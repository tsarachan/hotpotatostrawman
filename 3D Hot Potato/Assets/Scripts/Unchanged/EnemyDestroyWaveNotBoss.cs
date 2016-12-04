using UnityEngine;
using System.Collections;

public class EnemyDestroyWaveNotBoss : ObjectPooling.Poolable {

	private const string ENEMY_ORGANIZER = "Enemies";
	private const string BOSS_OBJ = "Boss";

	private void Start(){
		ClearNonBossEnemies();
		ObjectPooling.ObjectPool.AddObj(gameObject); //shut this off and return it to the pool
	}

	private void ClearNonBossEnemies(){
		Transform enemies = GameObject.Find(ENEMY_ORGANIZER).transform;

		foreach (Transform enemy in enemies){
			if (enemy.GetComponent<EnemyBase>() != null && !enemy.name.Contains(BOSS_OBJ)){
				enemy.GetComponent<EnemyBase>().GetDestroyed();
			}
		}
	}
}
