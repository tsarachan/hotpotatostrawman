using UnityEngine;
using System.Collections;

public class CatchParticleBehavior : ObjectPooling.Poolable {


	//how long the particle system lasts
	public float existDuration = 1.0f;
	private float existTimer = 0.0f;

	private const string PARTICLES_ORGANIZER = "Particles";


	private void Start(){
		transform.parent = GameObject.Find(PARTICLES_ORGANIZER).transform;
	}


	private void Update(){
		MeasureLifetime();
	}


	private void MeasureLifetime(){
		existTimer += Time.deltaTime;

		if (existTimer >= existDuration){
			ShutOff();
		}
	}


	//override functions for Poolable that omit the rigidbodies
	public override void Reset(){
		gameObject.SetActive(true);

	}


	public override void ShutOff(){
		existTimer = 0.0f;
		gameObject.SetActive(false);
	}
}
