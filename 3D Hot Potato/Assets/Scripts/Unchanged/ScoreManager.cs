using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreManager : MonoBehaviour {

	//----------Tunable variables----------
	public float nextComboTime = 0.5f; //how long you have to destroy an enemy before the combo resets


	//----------Internal variables----------

	private float comboTimer = 0.0f;



	private float score = 0.0f;
	private int combo = 0;
	private int comboStart = 0;

	private const string COMBO_LABEL = "x Combo";


	private Text scoreText;
	private Text comboText;
	private const string SIGN_OBJ = "Score sign";
	private const string CANVAS_OBJ = "Score canvas";
	private const string SCORE_TEXT_OBJ = "Score text";
	private const string COMBO_TEXT_OBJ = "Combo text";


	private void Start(){
		scoreText = GameObject.Find(SIGN_OBJ).transform.Find(CANVAS_OBJ).Find(SCORE_TEXT_OBJ)
			.GetComponent<Text>();
		comboText = GameObject.Find(SIGN_OBJ).transform.Find(CANVAS_OBJ).Find(COMBO_TEXT_OBJ)
			.GetComponent<Text>();

		scoreText.text = score.ToString();
		comboText.text = combo.ToString() + COMBO_LABEL;
	}


	private void Update(){
		comboTimer += Time.deltaTime;

		if (comboTimer >= nextComboTime){
			comboText.text = ResetCombo();

			comboTimer = 0.0f;
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
	}


	private string ResetCombo(){
		combo = comboStart;
		return combo.ToString() + COMBO_LABEL;
	}
}
