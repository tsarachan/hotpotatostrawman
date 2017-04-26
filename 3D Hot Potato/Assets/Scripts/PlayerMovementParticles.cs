using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementParticles : MonoBehaviour {

	private List<string> inputsThisFrame = new List<string>();
	private List<string> inputsLastFrame = new List<string>();


	private ParticleSystem brakeParticle;
	private const string BRAKE_PARTICLE_OBJ = "Brake particle";


	//possible inputs; these must match those in InputManager
	private const string DOWN = "down";
	private const string UP = "up";


	private void Start(){
		brakeParticle = transform.Find(BRAKE_PARTICLE_OBJ).GetComponent<ParticleSystem>();
	}


	private void LateUpdate(){
		foreach (string input in inputsThisFrame){
			if (!inputsLastFrame.Contains(input)){
				PlayMovementParticles(input);
			}
		}

		inputsLastFrame.Clear();

		foreach (string input in inputsThisFrame){
			inputsLastFrame.Add(input);
		}

		inputsThisFrame.Clear();
	}


	private void PlayMovementParticles(string input){
		switch (input){
			case UP:
				brakeParticle.Play();
				break;
		}
	}


	public void GetInput(string input){
		if (!inputsThisFrame.Contains(input)){
			inputsThisFrame.Add(input);
		}
	}
}
