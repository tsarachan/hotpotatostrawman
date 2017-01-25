using System.Collections;
using UnityEngine;

public class HoldPowerConstructs : HoldPower {

	//tunable variables
	public float timeBetweenConstructs = 0.2f;
	public float offsetAngle = 20.0f; //affects new constructs' direction


	//the gameobject that this player is going to send out 
	private GameObject construct;
	private const string CONSTRUCT_NAME = "Hold power construct";


	//internal variables
	private float constructTimer = 0.0f;
	private float lastAngle = 0.0f;


	protected override void Update(){
		holdTimer = RunHoldTimer();

		if (holdTimer > holdDuration){
			constructTimer += Time.deltaTime;

			if (constructTimer >= timeBetweenConstructs){
				ActivateHoldPower();
				constructTimer = 0.0f;
			}
		}
	}


	protected override void ActivateHoldPower(){
		if (lastAngle > 0){
			lastAngle = lastAngle - 180.0f + offsetAngle;
		} else {
			lastAngle = lastAngle + 180.0f + offsetAngle;
		}


		GameObject construct = ObjectPooling.ObjectPool.GetObj(CONSTRUCT_NAME);
		construct.transform.position = transform.position;
		construct.GetComponent<ParticleCubeBehavior>().SetDirection(new Vector3(0.0f, lastAngle, 0.0f));
	}
}
