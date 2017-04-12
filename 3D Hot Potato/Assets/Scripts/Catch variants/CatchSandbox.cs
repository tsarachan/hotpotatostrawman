/*
 * 
 * This script contains all the special catches. If you want to experiment with a new special catch, add it here.
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchSandbox : MonoBehaviour {


	//----------Tunable variables----------

	[Header("These are only relevant to p2's multiburst")]
	public Vector3[] multiBurstPositions;
	public float timeBetweenMultibursts = 0.3f;


	//----------Internal variables----------


	//variables relating to the AOE burst
	protected const string BURST_OBJ = "Burst prefab";


	//variables for a smaller burst
	protected const string MINI_BURST_OBJ = "Mini burst prefab";


	//variables relating to the tether
	protected GameObject tether;
	protected const string TETHER_OBJ = "Lightsaber prefab";
	protected const string PARTICLE_ORGANIZER = "Particles";
	protected GameObject lightPillar;
	protected const string PILLAR_OBJ = "Light pillar prefab";


	//variables for a directable lightning bolt coming from the player
	protected const string DIRECTIONAL_LIGHTNING = "Directional lightning prefab";


	//variables for the beam weapon that both players aim--a replacement for the tether
	protected const string DEATH_RAY = "Two-player death ray";
	protected GameObject deathRay;


	//used to prevent powers from activating while already in effect
	protected bool powerRunning = false;


	protected virtual void Start(){

		//nonsense initializations for determining which powers are in use
		deathRay = gameObject;
		tether = gameObject;
		lightPillar = gameObject;
	}


	/// <summary>
	/// Call this to activate the tether.
	/// 
	/// This method handles loading the tether and switching it on. The tether handles its own lifespan, and will
	/// switch itself off.
	/// </summary>
	/// <returns>The tether's gameobject.</returns>
	public void Tether(){
		StartCoroutine(CallDownTether());
	}


	protected GameObject SetUpTether(){
		if (tether != gameObject){
			return tether;
		} else {
			GameObject newTether = Instantiate(Resources.Load(TETHER_OBJ) as GameObject,
							   				   Vector3.zero,
							   				   Quaternion.identity,
							   				   transform.root.Find(PARTICLE_ORGANIZER));
			newTether.GetComponent<LightsaberBehavior>().Setup();

			return newTether;
		}
	}


	protected IEnumerator CallDownTether(){
		lightPillar = SetUpLightPillar();
		tether = SetUpTether();

		yield return StartCoroutine(lightPillar.GetComponent<LightPillarBehavior>().ShineDown());

		tether.GetComponent<LightsaberBehavior>().ExtendConnection();

		yield break;
	}


	protected GameObject SetUpLightPillar(){
		if (lightPillar != gameObject){
			return lightPillar;
		} else {
			GameObject newPillar = Instantiate(Resources.Load(PILLAR_OBJ) as GameObject,
											   Vector3.zero,
											   Quaternion.identity,
											   transform.root.Find(PARTICLE_ORGANIZER));
			newPillar.GetComponent<LightPillarBehavior>().Setup(transform);

			return newPillar;
		}
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


	protected IEnumerator MultiBurst(Vector3 center){
		float timer = 0.0f;
		int burstsSoFar = 0;

		while (burstsSoFar < multiBurstPositions.Length){
			timer += Time.deltaTime;

			if (timer >= timeBetweenMultibursts){
				GameObject burst = ObjectPooling.ObjectPool.GetObj(BURST_OBJ);
				burst.transform.position = center + multiBurstPositions[burstsSoFar];

				burstsSoFar++;

				timer = 0.0f;
			}

			yield return null;
		}

		yield break;
	}


	protected IEnumerator MultiBurstAroundPlayer(Vector3 playerPosition){
		if (powerRunning){
			yield break;
		} else {
			powerRunning = true;
			Debug.Log("starting new MultiBurstAroundPlayer");

			float timer = 0.0f;
			int burstsSoFar = 0;
			int totalBursts = 4;
			float distFromPlayer = 8.0f;
			float stopTime = Time.time + timeBetweenMultibursts * totalBursts;
			float stopTimer = Time.time;

			while (stopTimer <= stopTime ){
				//Debug.Log("totalTimer == " + totalTimer + ", totalDuration == " + totalDuration);
				timer += Time.deltaTime;
				stopTimer = Time.time;

				if (timer >= timeBetweenMultibursts && burstsSoFar < totalBursts){
					GameObject burst = ObjectPooling.ObjectPool.GetObj(BURST_OBJ);
					stopTime = Time.time + Mathf.Max(timeBetweenMultibursts * totalBursts,
													 burst.GetComponent<BurstBehavior>().existDuration);

					switch (burstsSoFar){
						case 0:
							burst.transform.position = playerPosition + new Vector3(0.0f, 0.0f, distFromPlayer);
							Debug.Log("Making first burst");
							break;
						case 1:
							burst.transform.position = playerPosition + new Vector3(-distFromPlayer, 0.0f, 0.0f);
							Debug.Log("Making second burst");
							break;
						case 2:
							burst.transform.position = playerPosition + new Vector3(distFromPlayer, 0.0f, 0.0f);
							Debug.Log("Making third burst");
							break;
						case 3:
							burst.transform.position = playerPosition + new Vector3(0.0f, 0.0f, -distFromPlayer);
							Debug.Log("Making fourth burst");
							break;
						default:
							Debug.Log("Illegal burstsSoFar: " + burstsSoFar);
							break;
					}

					burstsSoFar++;
				
					timer = 0.0f;
				}

				yield return null;
			}
			powerRunning = false;
			Debug.Log("powerRunning == " + powerRunning);

			yield break;
		}
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
		//note that until InputManager is refactored, this will create the object but it won't be able receive inputs
		//for now, the lightning stream MUST be in the hierarchy at the start of the game
		if (directionalLightning == null){
			directionalLightning = Instantiate(Resources.Load(DIRECTIONAL_LIGHTNING),
											   transform.position,
											   Quaternion.identity,
											   transform) as GameObject;
		}

		//there should now definitely be directable lightning ready to go; if not, send an error message
		Debug.Assert(directionalLightning != null);

		directionalLightning.GetComponent<DirectionalLightning>().Activate();

		return directionalLightning;
	}


	/// <summary>
	/// Activates a reverse tether, which extends out from player 1 instead of extending back toward player 2.
	/// </summary>
	/// <returns>The ray object.</returns>
	protected GameObject TwoPlayerDeathRay(){
		deathRay = SetUpDeathRay();

		Debug.Assert(deathRay != null);
		Debug.Assert(deathRay != gameObject);

		deathRay.GetComponent<DeathRayBehavior>().Activate();

		return deathRay;
	}


	protected GameObject SetUpDeathRay(){
		if (deathRay != gameObject){
			return deathRay;
		} else {
			return Instantiate(Resources.Load(DEATH_RAY),
							   transform.position,
							   Quaternion.identity,
							   transform) as GameObject;
		}
	}
}
