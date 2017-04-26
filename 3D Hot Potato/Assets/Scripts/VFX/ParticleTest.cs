using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleTest : MonoBehaviour {

	private Transform spinner;
	private const string SPINNER = "Spinner axis";

	private Transform lightning;
	private const string LIGHTNING = "Lightning emitter";

	private Transform swoosh;
	private const string SWOOSH = "Swoosh emitter";


	private void Start(){
		spinner = transform.Find(SPINNER);
		lightning = transform.Find(LIGHTNING);
		swoosh = GameObject.Find(SWOOSH).transform;
	}


	private void Update(){
		if (Input.GetKeyDown(KeyCode.T)){
			spinner.GetChild(0).GetComponent<ParticleSystem>().Play();
		} else if (Input.GetKeyDown(KeyCode.Y)){
			lightning.GetComponent<ParticleSystem>().Play();
		} else if (Input.GetKeyDown(KeyCode.U)){
			swoosh.GetComponent<ParticleSystem>().Play();
		}
	}
}
