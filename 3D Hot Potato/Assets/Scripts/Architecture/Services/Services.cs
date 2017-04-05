using UnityEngine;

public abstract class Services {

	private static EventManager eventManager;
	public static EventManager EventManager{
		get {
			Debug.Assert(eventManager != null);
			return eventManager;
		} set {
			eventManager = value;
		}
	}
}
