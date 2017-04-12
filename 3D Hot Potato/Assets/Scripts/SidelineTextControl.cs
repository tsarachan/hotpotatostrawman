using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SidelineTextControl : MonoBehaviour {

	//----------Tunable variables----------

	//where text stops
	public Vector3 displayLoc = new Vector3(7.35f, 0.0f, -77.8f);


	//how long it takes text to slide onscreen
	public float enterDuration = 0.5f;
	public AnimationCurve enterCurve;


	//how long it takes text to leave the screen
	public float leaveDuration = 0.5f;
	public AnimationCurve leaveCurve;


	//how long messages are displayed
	public float displayTime = 1.0f;


	//where text waits at the top of the screen
	public Vector3 startLoc = new Vector3(-23.36f, 11.45f, 46.9f);


	//where text goes as it scrolls off the screen
	public Vector3 endLoc = new Vector3(7.35f, 0.0f, -96.25f);


	//----------Internal variables----------

	//UI text variables
	private TextMeshProUGUI text;
	private const string TEXT_OBJ = "Right side text";


	//used to check whether the text is already moving on screen
	private enum TextPosition { OFFTOP, ENTERING, DISPLAY, LEAVING };
	private TextPosition currentTextPosition;


	//used to differentiate teh current combo from other messages
	private const string COMBO_LABEL = "Combo";


	private void Start(){
		text = GameObject.Find(TEXT_OBJ).GetComponent<TextMeshProUGUI>();
		currentTextPosition = TextPosition.OFFTOP;
	}


	public IEnumerator ShowText(string message){
		if (currentTextPosition != TextPosition.OFFTOP){

			//if the message is an updated combo amount, change the number but otherwise don't interrupt the process
			if (text.text.Contains(COMBO_LABEL) && message.Contains(COMBO_LABEL)){
				text.text = message;
			}
			yield break;
		} else {
			currentTextPosition = TextPosition.ENTERING;
		}

		text.text = message;

		for (float timer = 0.0f; timer <= enterDuration; timer += Time.deltaTime){
			text.transform.position = Vector3.Lerp(startLoc, displayLoc, enterCurve.Evaluate(timer/enterDuration));

			yield return null;
		}

		currentTextPosition = TextPosition.DISPLAY;

		yield return new WaitForSeconds(displayTime);

		yield return StartCoroutine(LeaveText());

		yield break;
	}


	private IEnumerator LeaveText(){
		if (currentTextPosition != TextPosition.DISPLAY){
			yield break;
		} else {
			currentTextPosition = TextPosition.LEAVING;
		}

		for (float timer = 0.0f; timer <= leaveDuration; timer += Time.deltaTime){
			text.transform.position = Vector3.Lerp(displayLoc, endLoc, leaveCurve.Evaluate(timer/enterDuration));

			yield return null;
		}

		currentTextPosition = TextPosition.OFFTOP;

		yield break;
	}
}
