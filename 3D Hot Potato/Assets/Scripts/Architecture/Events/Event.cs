//minimal base class for events

public abstract class Event {
	public delegate void Handler(Event e);
}
