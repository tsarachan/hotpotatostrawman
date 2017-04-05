using UnityEngine;

public class PowerTriggeredEvent : Event {

	public readonly GameObject player;

	public PowerTriggeredEvent(GameObject player){
		this.player = player;
	}
}
