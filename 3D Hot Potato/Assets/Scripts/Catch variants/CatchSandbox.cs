/*
 * 
 * This script contains all the special catches. If you want to experiment with a new special catch, add it here.
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchSandbox : MonoBehaviour {


	//variables relating to the AOE burst
	protected const string BURST_OBJ = "Burst prefab";


	//variables for a smaller burst
	protected const string MINI_BURST_OBJ = "Mini burst prefab";


	//variables relating to the tether
	protected const string TETHER_OBJ = "Lightning prefab";
	protected const string PARTICLE_ORGANIZER = "Particles";


	//variables for a directable lightning bolt coming from the player
	protected const string DIRECTIONAL_LIGHTNING = "Directional lightning prefab";


	/// <summary>
	/// Call this to activate the tether.
	/// 
	/// This method handles loading the tether and switching it on. The tether handles its own lifespan, and will
	/// switch itself off.
	/// </summary>
	/// <returns>The tether's gameobject.</returns>
	protected GameObject Tether(){
		//get a reference to the tether
		GameObject tether = transform.root.Find(PARTICLE_ORGANIZER).Find(TETHER_OBJ).gameObject;

		//if there's no tether yet, make one
		if (tether == null){
			tether = Instantiate(Resources.Load(TETHER_OBJ),
								 new Vector3(0.0f, 0.0f, 0.0f),
								 Quaternion.identity,
								 GameObject.Find(PARTICLE_ORGANIZER).transform) as GameObject;
		}

		//at this point, there should definitely be a reference to the tether. If not, send an error message.
		Debug.Assert(tether != null);

		tether.SetActive(true);

		return tether;
	}


	/// <summary>
	/// Creates a burst at a specified location.
	/// </summary>
	/// <returns>The burst object.</returns>
	/// <param name="location">The location where the burst should appear.</param>
	protected GameObject SingleBurst(Vector3 location){
		GameObject burst = ObjectPooling.ObjectPool.GetObj(BURST_OBJ);
		burst.transform.position = location;

		return burst;
	}


	/// <summary>
	/// The same as SingleBurst, above, but the burst is smaller.
	/// </summary>
	/// <returns>The mini burst object.</returns>
	/// <param name="location">The location where the burst should appear.</param>
	protected GameObject SingleMiniBurst(Vector3 location){
		GameObject miniBurst = ObjectPooling.ObjectPool.GetObj(MINI_BURST_OBJ);
		miniBurst.transform.position = location;

		return miniBurst;
	}


	/// <summary>
	/// Switches on a stream of lightning that the player can direct with the directional controller,
	/// whether it's a keyboard or a joystick. The stream is responsible for switching itself off.
	/// 
	/// If necessary, this will handle loading the stream of lightning into the scene.
	/// </summary>
	/// <returns>The lightning object.</returns>
	protected GameObject DirectionalLightning(){
		//get a reference to the directable lightning
		GameObject directionalLightning = transform.Find(DIRECTIONAL_LIGHTNING).gameObject;

		//if the directable lightning isn't already there, create it
		if (directionalLightning == null){
			directionalLightning = Instantiate(Resources.Load(DIRECTIONAL_LIGHTNING),
											   transform.position,
											   Quaternion.identity,
											   transform) as GameObject;
		}

		//there should now definitely be directable lightning ready to go; if not, send an error message
		Debug.Assert(directionalLightning != null);

		directionalLightning.SetActive(true);

		return directionalLightning;
	}
}
