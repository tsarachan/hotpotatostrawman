using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementParticles : MonoBehaviour {


	private List<string> inputsThisFrame = new List<string>();
	private List<string> inputsLastFrame = new List<string>();



	//various particles associated with different inputs

	//the puff of dust raised by braking
	private ParticleSystem brakeParticle;
	private const string BRAKE_PARTICLE_OBJ = "Brake particle";

	//the line left by accelerating
	private GameObject accelParticle;
	private const string ACCEL_PARTICLE_OBJ = "Acceleration particle";
	private ParticleSystem accelParticleVert;
	private ParticleSystem accelParticleLeft;
	private ParticleSystem accelParticleRight;
	private const string VERTICAL_OBJ = "Vertical";
	private const string LEFT_OBJ = "Left";
	private const string RIGHT_OBJ = "Right";


	//possible inputs; these must match those in InputManager
	private const string DOWN = "down";
	private const string UP = "up";


	//variables associated with the wind trails
	public bool windActive = true; //are we using wind at all?
	private ContrailBehavior frontWind;
	private ContrailBehavior rearWind;
	private ContrailBehavior leftWind;
	private ContrailBehavior rightWind;
	private const string WIND_SOURCE = " wind source";
	private const string FRONT_OBJ = "Forward";
	private const string REAR_OBJ = "Rear";
	private const string CYCLE_OBJ = "Cycle and rider";


	private void Start(){
		brakeParticle = transform.Find(BRAKE_PARTICLE_OBJ).GetComponent<ParticleSystem>();

		ParticleSystem.MainModule mainBrakeParticle= brakeParticle.main;
		mainBrakeParticle.startColor = RenderSettings.ambientLight;

		Color particleColor = RenderSettings.ambientLight;
		particleColor.a = 0.0f;

		brakeParticle.GetComponent<Renderer>().material.SetColor("_Color", particleColor);
		brakeParticle.GetComponent<Renderer>().material.SetColor("_Ambient", particleColor);



		accelParticle = transform.Find(ACCEL_PARTICLE_OBJ).gameObject;
		accelParticleVert = accelParticle.transform.Find(VERTICAL_OBJ).GetComponent<ParticleSystem>();
		accelParticleLeft = accelParticle.transform.Find(LEFT_OBJ).GetComponent<ParticleSystem>();
		accelParticleRight = accelParticle.transform.Find(RIGHT_OBJ).GetComponent<ParticleSystem>();

		if (windActive){
			frontWind = transform.Find(CYCLE_OBJ).Find(FRONT_OBJ + WIND_SOURCE).GetComponent<ContrailBehavior>();
			rearWind = transform.Find(CYCLE_OBJ).Find(REAR_OBJ + WIND_SOURCE).GetComponent<ContrailBehavior>();
			leftWind = transform.Find(CYCLE_OBJ).Find(LEFT_OBJ + WIND_SOURCE).GetComponent<ContrailBehavior>();
			rightWind = transform.Find(CYCLE_OBJ).Find(RIGHT_OBJ + WIND_SOURCE).GetComponent<ContrailBehavior>();

			frontWind.Active = false;
			rearWind.Active = false;
			leftWind.ChangeState(true);
			rightWind.ChangeState(true);
		}
	}


	private void LateUpdate(){
		//start effects that should begin this frame
		foreach (string input in inputsThisFrame){
			if (!inputsLastFrame.Contains(input)){
				PlayMovementParticles(input);
			}
		}


		//end effects where inputs are missing
		foreach (string input in inputsLastFrame){
			if (!inputsThisFrame.Contains(input)){
				StopMovementParticles(input);
			}
		}


		if (windActive){
			HandleContrails();
		}

		inputsLastFrame.Clear();

		foreach (string input in inputsThisFrame){
			inputsLastFrame.Add(input);
		}

		inputsThisFrame.Clear();
	}


	private void HandleContrails(){
		if (inputsThisFrame.Count == 0 ||
			inputsThisFrame.Count == 1 && inputsThisFrame.Contains(DOWN)){
			//Debug.Log("Left and right; inputsThisFrame.Count == " + inputsThisFrame.Count);
			frontWind.ChangeState(false);
			rearWind.ChangeState(false);
			leftWind.ChangeState(true);
			rightWind.ChangeState(true);
		} else {
			//Debug.Log("Front and back; inputsThisFrame.Count == " + inputsThisFrame.Count);
			frontWind.ChangeState(true);
			rearWind.ChangeState(true);
			leftWind.ChangeState(false);
			rightWind.ChangeState(false);
		}
	}


	private void PlayMovementParticles(string input){
		switch (input){
			case UP:
				brakeParticle.Play();
				break;
			case DOWN:
				accelParticleVert.Play();
				accelParticleLeft.Play();
				accelParticleRight.Play();
				break;

		}
	}


	private void StopMovementParticles(string input){
		switch(input){
		case UP:
				brakeParticle.Stop();
				break;
			case DOWN:
				accelParticleVert.Stop();
				accelParticleLeft.Stop();
				accelParticleRight.Stop();
				break;
		}
	}


	public void GetInput(string input){
		if (!inputsThisFrame.Contains(input)){
			inputsThisFrame.Add(input);
		}
	}
}
