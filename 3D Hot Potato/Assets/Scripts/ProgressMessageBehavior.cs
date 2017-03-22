using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressMessageBehavior : MonoBehaviour {

	public Vector3 startLoc = new Vector3(0.0f, 0.0f, 140.0f);
	public Vector3 endLoc = new Vector3(0.0f, 0.0f, -16.0f);
	public float speed = 10.0f;


	private Vector3 movement = new Vector3(0.0f, 0.0f, 0.0f);

	private Text text;
	private const string CANVAS_OBJ = "Canvas";
	private const string TEXT_OBJ = "Text";


	private const string WORLD = "World ";
	private const string ACT = "Act ";
	private const string NEWLINE = "\r\n";


	private void Start(){
		text = transform.Find(CANVAS_OBJ).Find(TEXT_OBJ).GetComponent<Text>();
		movement.z = -speed;
		transform.position = startLoc;
	}


	public void NewMessage(int worldNum, int actNum, string message){
		text.text = WORLD + worldNum.ToString() + NEWLINE + ACT + actNum.ToString() + NEWLINE + message;

		StartCoroutine(ScrollMessage());
	}


	private IEnumerator ScrollMessage(){
		transform.position = startLoc;

		while (transform.position.z > endLoc.z){
			transform.Translate(movement);

			yield return null;
		}

		yield break;
	}
}
