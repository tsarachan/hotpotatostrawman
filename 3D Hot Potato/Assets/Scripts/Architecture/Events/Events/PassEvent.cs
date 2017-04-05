using UnityEngine;

public class PassEvent : Event {

	public readonly GameObject throwingPlayer;
	public readonly GameObject receivingPlayer;

	public PassEvent(GameObject throwingPlayer, GameObject receivingPlayer){
		this.throwingPlayer = throwingPlayer;
		this.receivingPlayer = receivingPlayer;
	}
}
