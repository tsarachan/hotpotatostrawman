using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticlePlexus : MonoBehaviour {

	public float maxDistance = 1.0f;
	new ParticleSystem myParticleSystem;
	ParticleSystem.Particle[] particles;

	ParticleSystem.MainModule particleSystemModule;

	public LineRenderer lineRendererTemplate;
	private List<LineRenderer> lineRenderers = new List<LineRenderer>();

	private Transform focalTransform;


	private void Start () {
		myParticleSystem = GetComponent<ParticleSystem>();
		particleSystemModule = myParticleSystem.main;
		
	}
	
	// Update is called once per frame
	void LateUpdate () {
		int maxParticles = particleSystemModule.maxParticles;

		if (particles == null || particles.Length < maxParticles){
			particles = new ParticleSystem.Particle[maxParticles];
		}

		myParticleSystem.GetParticles(particles);

		int particleCount = myParticleSystem.particleCount;

		float maxDistanceSqr = maxDistance * maxDistance;

		int lineRendererIndex = 0;
		int lineRendererCount = lineRenderers.Count;

		switch(particleSystemModule.simulationSpace){
		case ParticleSystemSimulationSpace.Local:
			focalTransform = transform;
			lineRendererTemplate.useWorldSpace = false;
			break;
		case ParticleSystemSimulationSpace.Custom:
			focalTransform = particleSystemModule.customSimulationSpace;
			lineRendererTemplate.useWorldSpace = false;
			break;
		case ParticleSystemSimulationSpace.World:
			focalTransform = transform;
			lineRendererTemplate.useWorldSpace = true;
			break;
		default:
			Debug.Log("Unsupported simulation space " + particleSystemModule.simulationSpace);
			break;
		}

		//check each particle to see if it's connected to the other particles
		//this is not the ideal algorithm!
		for (int i = 0; i < particleCount; i++){
			Vector3 p1Position = particles[i].position;

			for (int j = i + 1; j < particleCount; j++){
				Vector3 p2Position = particles[j].position;

				//using SqrMagnitude is more efficient, since it avoids a square root call
				float distanceSqr = Vector3.SqrMagnitude(p1Position - p2Position);

				if (distanceSqr <= maxDistanceSqr){
					LineRenderer lr;

					if (lineRendererIndex == lineRendererCount){
						lr = Instantiate(lineRendererTemplate, focalTransform, false);
						lineRenderers.Add(lr);

						lineRendererCount++;
					}

					//get a reference to the renderer to minimize expensive lookups
					lr = lineRenderers[lineRendererIndex];

					lr.enabled = true;

					lr.SetPosition(0, p1Position);
					lr.SetPosition(1, p2Position);

					lineRendererIndex++;
				}
			}
		}

		for (int i = lineRendererIndex; i < lineRendererCount; i++){
			lineRenderers[i].enabled = false;
		}
	}
}
