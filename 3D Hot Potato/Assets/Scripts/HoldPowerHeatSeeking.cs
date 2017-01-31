using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldPowerHeatSeeking : HoldPower {


	//tunable variables
	public float deployDuration = 1.0f; //how long it takes the heat-seekers to arm
	public AnimationCurve deployCurve;
	public float retractDuration = 0.3f; //how long it takes the heat-seekers to retract after passing the battery star
	public AnimationCurve retractCurve;
	public float deployedScale = 1.0f; //the heat-seekers' scale when fully deployed


	//internal variables
	private Transform constructs;
	private const string CONSTRUCT_ORGANIZER = "Construct missiles";
	private float deployTimer = 0.0f;
	private float deployStartTime = 0.0f;
	private float retractTimer = 0.0f;
	private float calculatedRetractDuration = 0.0f;


	protected override void Start(){
		base.Start();

		GameObject constructRing = Instantiate(Resources.Load(CONSTRUCT_ORGANIZER),
											   transform.position,
											   Quaternion.identity,
											   transform) as GameObject;
		
		constructs = constructRing.transform;
	}

	protected override void Update(){
		holdTimer = RunHoldTimer();

		if (holdTimer > holdDuration){
			ActivateHoldPower();
		} else {
			constructs.localScale = Retract();
		}
	}


	protected override void ActivateHoldPower(){
		deployTimer += Time.deltaTime;

		float currentScale = Mathf.Lerp(0.0f, deployedScale, deployCurve.Evaluate(deployTimer/deployDuration));

		//keep track of how long it will take to retract from this position, in case the player passes in mid-deployment
		calculatedRetractDuration = retractDuration * (currentScale/deployedScale);

		constructs.localScale = new Vector3(currentScale, currentScale, currentScale);

		if (deployedScale - currentScale <= Mathf.Epsilon){
			FireMissiles();
			deployTimer = 0.0f;
		}
	}


	private void FireMissiles(){
		List<Transform> missiles = new List<Transform>();

		for (int i = 0; i < constructs.childCount; i++){
			missiles.Add(constructs.GetChild(i));
		}

		for (int i = 0; i < missiles.Count; i++){
			missiles[i].GetComponent<MissileBehavior>().Launch();
		}

		GameObject constructRing = Instantiate(Resources.Load(CONSTRUCT_ORGANIZER),
											   transform.position,
											   Quaternion.identity,
											   transform) as GameObject;

		constructs = constructRing.transform;
	}

	private Vector3 Retract(){
		retractTimer += Time.deltaTime;

		float currentScale = Mathf.Lerp(calculatedRetractDuration, 0.0f, retractCurve.Evaluate(retractTimer/retractDuration));

		return new Vector3(currentScale, currentScale, currentScale);
	}
}
