using UnityEngine;

public class EnemyDestroyedEvent : Event {

	public readonly GameObject enemy;
	public readonly GameObject destroyer;

	public EnemyDestroyedEvent(GameObject enemy, GameObject destroyer){
		this.enemy = enemy;
		this.destroyer = destroyer;
	}
}
