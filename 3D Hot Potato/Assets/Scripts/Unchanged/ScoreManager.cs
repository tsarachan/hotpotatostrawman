using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreManager : MonoBehaviour {

	//----------Tunable variables----------

	//how long you have to destroy an enemy before the combo resets
	public float nextComboTime = 0.5f;


	//where the sign is when it's on screen and in view
	public Vector3 displayLoc = new Vector3(-23.36f, 11.45f, 22.2f);


	//how long it takes the sign to enter the screen
	public float enterDuration = 0.5f;
	public AnimationCurve enterCurve;


	//how long it takes the sign to leave the screen
	public float leaveDuration = 0.5f;
	public AnimationCurve leaveCurve;


	//the sign's location when it starts to enter the screen
	public Vector3 startLoc = new Vector3(-23.36f, 11.45f, 104.0f);


	//where the sign goes as it leaves the screen
	public Vector3 endLoc = new Vector3(-23.36f, 11.45f, -18.51f);


	//----------Internal variables----------

	private float comboTimer = 0.0f;


	//these keep track of the score and current combo
	private float score = 0.0f;
	private int combo = 0;
	private int comboStart = 0;


	//this is added to the current combo value ot display the combo to the player
	private const string COMBO_LABEL = "x Combo";


	//UI text variables
	private Text scoreText;
	private Text comboText;
	private const string SIGN_OBJ = "Score sign";
	private const string CANVAS_OBJ = "Score canvas";
	private const string SCORE_TEXT_OBJ = "Score text";
	private const string COMBO_TEXT_OBJ = "Combo text";


	//used to move the score information in and out of view
	private Transform sign;


	//used to prevent the sign from trying to enter and leave at the same time
	private bool signIsInView = false;


	private void Start(){
		sign = GameObject.Find(SIGN_OBJ).transform;
		sign.position = startLoc;

		scoreText = sign.Find(CANVAS_OBJ).Find(SCORE_TEXT_OBJ)
			.GetComponent<Text>();
		comboText = sign.Find(CANVAS_OBJ).Find(COMBO_TEXT_OBJ)
			.GetComponent<Text>();

		scoreText.text = score.ToString();
		comboText.text = combo.ToString() + COMBO_LABEL;
	}


	private void Update(){
		if (combo > 0){
			comboTimer += Time.deltaTime;

			if (comboTimer >= nextComboTime){
				comboText.text = ResetCombo();

				comboTimer = 0.0f;
			}
		}
	}


	public void AddScore(int value){
		score += value + value * combo * 0.5f;
		scoreText.text = score.ToString();
	}


	public void IncreaseCombo(){
		combo++;
		comboText.text = combo.ToString() + COMBO_LABEL;

		comboTimer = 0.0f;

		StartCoroutine(MoveIntoView());
	}


	private string ResetCombo(){
		if (Mathf.Abs(sign.position.z - displayLoc.z) <= Mathf.Epsilon){
			StartCoroutine(LeaveView());
		}

		combo = comboStart;
		return combo.ToString() + COMBO_LABEL;
	}


	private IEnumerator MoveIntoView(){
		Debug.Log("MoveIntoView() called");
		if(signIsInView){
			yield break;
		}

		Debug.Log("sign not in view");

		signIsInView = true;

		sign.position = startLoc;

		float timer = 0.0f;

		while (timer <= enterDuration){
			Debug.Log("Moving into view; timer == " + timer);
			timer += Time.deltaTime;

			sign.position = Vector3.Lerp(startLoc, displayLoc, enterCurve.Evaluate(timer/enterDuration));

			yield return null;
		}

		yield break;
	}


	private IEnumerator LeaveView(){
		for (float timer = 0.0f; timer <= leaveDuration; timer += Time.deltaTime){
			sign.position = Vector3.Lerp(displayLoc, endLoc, enterCurve.Evaluate(timer/leaveDuration));

			yield return null;
		}

		signIsInView = false;

		yield break;
	}
}
