using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class ScoreManager : MonoBehaviour {

	//----------Tunable variables----------

	//how long you have to destroy an enemy before the combo resets
	public float nextComboTime = 0.5f;


	//----------Internal variables----------

	private float comboTimer = 0.0f;


	//these keep track of the score and current combo
	public float Score { get; private set; }
	private int combo = 0;
	private const int comboStart = 0;


	//this is added to the current combo value ot display the combo to the player
	private const string COMBO_LABEL = "x Combo";


	//this is added to the score as a label
	//currently this is intentionally blank
	//whatever is closest to the camera shows up the most clearly, so it seemed like it shoudl be numbers
	//surely everyone knows it's a score anyway . . . .
	private const string SCORE_LABEL = "";


	//UI text variables
	private TextMeshProUGUI scoreText;
	private const string SCORE_TEXT_OBJ = "Score text";
	private SidelineTextControl sidelineTextControl;
	private const string TEXT_CONTROL_OBJ = "Sideline text";


	//used to check whether the combo text is already moving on screen
	private enum SignPosition { OFFTOP, ENTERING, DISPLAY, LEAVING };
	private SignPosition currentSignPosition;


	private void Start(){

		scoreText = GameObject.Find(SCORE_TEXT_OBJ)
			.GetComponent<TextMeshProUGUI>();
		sidelineTextControl = GameObject.Find(TEXT_CONTROL_OBJ).GetComponent<SidelineTextControl>();

		scoreText.text = SCORE_LABEL + Score.ToString();
		currentSignPosition = SignPosition.OFFTOP;
	}


	private void Update(){
		if (combo > 0){
			comboTimer += Time.deltaTime;

			if (comboTimer >= nextComboTime){
				StartCoroutine(sidelineTextControl.ShowText(ResetCombo()));

				comboTimer = 0.0f;
			}
		}
	}


	public void AddScore(int value){
		Score += value * GetComboMultiplier();
		scoreText.text = SCORE_LABEL + Score.ToString();
	}


	/// <summary>
	/// Determines the score multiplier for ranges of combo values.
	/// 
	/// This assumes that the combo always increments before the multiplier is calculated. In other words,
	/// after the first enemy is destroyed the combo should be 1 (not 0) when this function is called.
	/// After 5 enemies have been destroyed the combo should already be be 5 (not 4), etc.
	/// </summary>
	/// <returns>The score multiplier.</returns>
	private int GetComboMultiplier(){
		if (combo <= 1){
			return 1;
		} else if (combo >= 2 && combo <= 7){
			return 2;
		} else if (combo >= 8 && combo <= 19){
			return 5;
		} else {
			return 10;
		}
	}


	public void IncreaseCombo(){
		combo++;

		comboTimer = 0.0f;
		nextComboTime = GetNextComboTime();

		StartCoroutine(sidelineTextControl.ShowText(combo.ToString() + COMBO_LABEL));
	}


	/// <summary>
	/// Calculates how long the players have to destroy another enemy in order to keep the combo going, as a function
	/// of the current combo total.
	/// 
	/// This assumes that the combo always increments before the time is calculated. In other words,
	/// after the first enemy is destroyed the combo should be 1 (not 0) when this function is called.
	/// After 5 enemies have been destroyed the combo should already be be 5 (not 4), etc.
	/// </summary>
	/// <returns>The next combo time.</returns>
	private float GetNextComboTime(){
		if (combo <= 1){
			return 2.0f;
		} else if (combo >= 2 && combo <= 7){
			return 1.0f;
		} else if (combo >= 8 && combo <= 19){
			return 0.5f;
		} else {
			return 0.3f;
		}
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


	/// <summary>
	/// Called when the game ends to record the score.
	/// </summary>
	public void RecordScore(){
		ScoreRepository.Score = (int)Score;
	}
}
