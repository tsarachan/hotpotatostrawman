using UnityEngine;

public class PassEvent : Event {

	public readonly GameObject throwingPlayer;

	public PassEvent(GameObject throwingPlayer){
		this.throwingPlayer = throwingPlayer;
	}
}
