using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class ScoreManager : MonoBehaviour {

	//----------Tunable variables----------

	//how long you have to destroy an enemy before the combo resets
	public float nextComboTime = 0.5f;


	//where combo notifications stop
	public Vector3 displayLoc = new Vector3(7.35f, 0.0f, -77.8f);


	//how long it takes combo notifications to slide onscreen
	public float enterDuration = 0.5f;
	public AnimationCurve enterCurve;


	//how long it takes combo notifications to leave the screen
	public float leaveDuration = 0.5f;
	public AnimationCurve leaveCurve;


	//where combo notifications wait at the top of the screen
	public Vector3 startLoc = new Vector3(-23.36f, 11.45f, 46.9f);


	//where combo notifications go as they scroll off the screen
	public Vector3 endLoc = new Vector3(7.35f, 0.0f, -96.25f);


	//----------Internal variables----------

	private float comboTimer = 0.0f;


	//these keep track of the score and current combo
	public float Score { get; private set; }
	private int combo = 0;
	private const int comboStart = 0;


	//this is added to the current combo value ot display the combo to the player
	private const string COMBO_LABEL = "x Combo";


	//this is added to the score as a label
	private const string SCORE_LABEL = "Score: ";


	//UI text variables
	private TextMeshProUGUI scoreText;
	private TextMeshProUGUI comboText;
	private const string SCORE_TEXT_OBJ = "Score text";
	private const string COMBO_TEXT_OBJ = "Combo text";


	//used to check whether the combo text is already moving on screen
	private enum SignPosition { OFFTOP, ENTERING, DISPLAY, LEAVING };
	private SignPosition currentSignPosition;


	private void Start(){

		scoreText = GameObject.Find(SCORE_TEXT_OBJ)
			.GetComponent<TextMeshProUGUI>();
		comboText = GameObject.Find(COMBO_TEXT_OBJ)
			.GetComponent<TextMeshProUGUI>();

		scoreText.text = SCORE_LABEL + Score.ToString();
		comboText.text = combo.ToString() + COMBO_LABEL;
		currentSignPosition = SignPosition.OFFTOP;
	}


	private void Update(){
		if (combo > 0){
			comboTimer += Time.deltaTime;

			if (comboTimer >= nextComboTime){
				comboText.text = ResetCombo();
				StartCoroutine(LeaveComboText());

				comboTimer = 0.0f;
			}
		}
	}


	public void AddScore(int value){
		Score += value + value * combo * 0.5f;
		scoreText.text = SCORE_LABEL + Score.ToString();
	}


	public void IncreaseCombo(){
		combo++;
		comboText.text = combo.ToString() + COMBO_LABEL;

		comboTimer = 0.0f;

		StartCoroutine(EnterComboText());
	}
		

	private IEnumerator EnterComboText(){
		if (currentSignPosition != SignPosition.OFFTOP){
			yield break;
		} else {
			currentSignPosition = SignPosition.ENTERING;
		}

		for (float timer = 0.0f; timer <= enterDuration; timer += Time.deltaTime){
			comboText.transform.position = Vector3.Lerp(startLoc, displayLoc, enterCurve.Evaluate(timer/enterDuration));

			yield return null;
		}

		currentSignPosition = SignPosition.DISPLAY;

		yield break;
	}


	private IEnumerator LeaveComboText(){
		if (currentSignPosition != SignPosition.DISPLAY){
			yield break;
		} else {
			currentSignPosition = SignPosition.LEAVING;
		}

		for (float timer = 0.0f; timer <= leaveDuration; timer += Time.deltaTime){
			comboText.transform.position = Vector3.Lerp(displayLoc, endLoc, leaveCurve.Evaluate(timer/enterDuration));

			yield return null;
		}

		currentSignPosition = SignPosition.OFFTOP;

		yield break;
	}


	private string ResetCombo(){
		combo = comboStart;
		return combo.ToString() + COMBO_LABEL;
	}


	public void ResetAfterLoss(float score){
		Score = score;
		scoreText.text = Score.ToString();
		ResetCombo();
	}
}
