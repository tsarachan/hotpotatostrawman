using System.Collections;
using UnityEngine;

public class HoldPowerConstructs : HoldPower {

	//tunable variables
	public float rotationSpeed = 1.0f; //how quickly the constructs rotate around their center
	public float deployDuration = 1.0f; //time needed for the construct ring's x, y, and z scales to reach deployedScale, below
	public AnimationCurve deployCurve;
	public float retractDuration = 0.3f; //how long the construct ring takes to retract after a pass
	public AnimationCurve retractCurve;
	public float deployedScale = 1.0f; //the construct's x, y, and z scale when fully deployed



	//internal variables
	private Transform constructs;
	private const string CONSTRUCT_ORGANIZER = "Construct ring";
	private float deployTimer = 0.0f;
	private float retractTimer = 0.0f;


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

		constructs.localScale = new Vector3(currentScale, currentScale, currentScale);

		constructs.Rotate(new Vector3(0.0f, rotationSpeed, 0.0f), Space.Self);
	}


	private Vector3 Retract(){
		retractTimer += Time.deltaTime;

		float currentScale = Mathf.Lerp(deployedScale, 0.0f, retractCurve.Evaluate(retractTimer/retractDuration));

		return new Vector3(currentScale, currentScale, currentScale);
	}

}
