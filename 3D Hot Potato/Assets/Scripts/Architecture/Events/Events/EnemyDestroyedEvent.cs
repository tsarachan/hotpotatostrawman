using UnityEngine;

public class EnemyDestroyedEvent : Event {

	public readonly GameObject enemy;

	public EnemyDestroyedEvent(GameObject enemy){
		this.enemy = enemy;
	}
}
