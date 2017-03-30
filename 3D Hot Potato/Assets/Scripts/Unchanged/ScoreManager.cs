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
	public float Score { get; private set; }
	private int combo = 0;
	private const int comboStart = 0;


	//this is added to the current combo value ot display the combo to the player
	private const string COMBO_LABEL = "x\r\nCombo";


	//UI text variables
	private Text scoreText;
	private Text comboText;
	private const string SCORE_TEXT_OBJ = "Score text";
	private const string COMBO_TEXT_OBJ = "Combo text";


	private void Start(){

		scoreText = GameObject.Find(SCORE_TEXT_OBJ)
			.GetComponent<Text>();
		comboText = GameObject.Find(COMBO_TEXT_OBJ)
			.GetComponent<Text>();

		scoreText.text = Score.ToString();
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
		Score += value + value * combo * 0.5f;
		scoreText.text = Score.ToString();
	}


	public void IncreaseCombo(){
		combo++;
		comboText.text = combo.ToString() + COMBO_LABEL;

		comboTimer = 0.0f;
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
