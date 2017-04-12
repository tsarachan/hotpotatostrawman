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
		Score += value + value * combo * 0.5f;
		scoreText.text = SCORE_LABEL + Score.ToString();
	}


	public void IncreaseCombo(){
		combo++;

		comboTimer = 0.0f;

		StartCoroutine(sidelineTextControl.ShowText(combo.ToString() + COMBO_LABEL));
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
