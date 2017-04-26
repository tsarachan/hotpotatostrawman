using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSeek : MonoBehaviour {


	public float force = 10.0f;


	public Transform target;
	private ParticleSystem ps;



	private void Start(){
		ps = GetComponent<ParticleSystem>();
	}


	//wait for the particle system to be updated before applying new forces
	private void LateUpdate(){
		ParticleSystem.Particle[] particles = new ParticleSystem.Particle[ps.particleCount];
		ps.GetParticles(particles);

		for (int i = 0; i < particles.Length; i++){
			ParticleSystem.Particle currentParticle = particles[i];

			Vector3 direction = (target.position - currentParticle.position).normalized;

			Vector3 seekForce = direction * force * Time.deltaTime;

			currentParticle.velocity += seekForce;

			particles[i] = currentParticle;
		}

		ps.SetParticles(particles, particles.Length);
	}
}
