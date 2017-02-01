using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIElementMove : MonoBehaviour {


	//tunable variables for moving the UI element
	public float startXPos = -600.0f; //starting position on the X axis
	public float endXPos = 600.0f; //ending position on the X axis
	public float moveDuration = 1.0f;
	public float timer = 0.0f;
	public AnimationCurve moveCurve;


	//internal variables
	private RectTransform rectTransform;
	private bool isMoving = false;


	private void Start(){
		rectTransform = GetComponent<RectTransform>();
	}


	private void Update(){
		if (isMoving){
			timer += Time.deltaTime;

			rectTransform.anchoredPosition = Move();

			if (timer >= moveDuration){
				isMoving = false;
			}
		}
	}


	private Vector2 Move(){
		Vector2 temp = new Vector2(0.0f, 0.0f);

		temp.x = Mathf.Lerp(startXPos, endXPos, moveCurve.Evaluate(timer/moveDuration));

		return temp;
	}


	public void StartUIElementMoving(){
		isMoving = true;
	}
}
