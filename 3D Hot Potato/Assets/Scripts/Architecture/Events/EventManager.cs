/*
 * 
 * Synchronous event system
 * 
 */

using System;
using System.Collections.Generic;

public class EventManager {

	//dictionary that maps Event types to their handlers
	private Dictionary<Type, Event.Handler> registeredHandlers = new Dictionary<Type, Event.Handler>();


	public void Register<T>(Event.Handler handler) where T : Event {
		Type type = typeof(T);
		if (registeredHandlers.ContainsKey(type)){
			registeredHandlers[type] += handler;
		} else {
			registeredHandlers[type] = handler;
		}
	}


	public void Unregister<T>(Event.Handler handler) where T : Event {
		Type type = typeof(T);
		Event.Handler handlers;

		if (registeredHandlers.TryGetValue(type, out handlers)){
			handlers -= handler;
			if (handlers == null){
				registeredHandlers.Remove(type);
			} else {
				registeredHandlers[type] = handlers;
			}
		}
	}


	public void Fire(Event e){
		Type type = e.GetType();
		Event.Handler handlers;

		if (registeredHandlers.TryGetValue(type, out handlers)){
			handlers(e);
		}
	}
}
