using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeAndScroll : ObjectPooling.Poolable {

	private const string PARTICLE_ORGANIZER = "Particles";
	public float lifetime = 2.0f;
	private float timer = 0.0f;


	private const string PLAYER_TAG = "Player";
	private const string DRIVE_THRU_PARTICLE = "Go through explosion particle";
	private bool driveThruable = true;


	private void Start(){
		transform.parent = GameObject.Find(PARTICLE_ORGANIZER).transform;
	}


	private void Update(){
		transform.position += new Vector3 (0, 0, -0.85f);

		timer += Time.deltaTime;

		if (timer >= lifetime){
			ObjectPooling.ObjectPool.AddObj(gameObject);
		}
	}


	public override void Reset(){
		timer = 0.0f; //sanity check; make sure the timer is at zero, so the object doesn't instantly disappear
		driveThruable = true;
		gameObject.SetActive(true);
		//Explode();
	}


	public override void ShutOff(){
		timer = 0.0f;
		gameObject.SetActive(false);
	}

	void Explode(){
		GetComponent<ParticleSystem> ().Stop ();
		GetComponent<ParticleSystem> ().Clear ();
		GetComponent<ParticleSystem> ().Play ();
	}


	private void OnTriggerEnter(Collider other){
		if (other.tag == PLAYER_TAG && driveThruable){
			ParticleSystem driveThruParticle = 
				other.transform.Find(DRIVE_THRU_PARTICLE).GetComponent<ParticleSystem>();

			driveThruParticle.Stop();
			driveThruParticle.Play();

			driveThruable = false;
		}
	}
}
