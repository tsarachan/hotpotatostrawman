using UnityEngine;
using System.Collections;

public class EnemyDestroyWaveNotBoss : MonoBehaviour {

	private const string ENEMY_ORGANIZER = "Enemies";
	private const string BOSS_OBJ = "Boss";

	private void Start(){
		ClearNonBossEnemies();
		Destroy(gameObject);
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
