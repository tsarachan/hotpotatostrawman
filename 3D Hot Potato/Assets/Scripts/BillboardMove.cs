using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BillboardMove : EnvironmentMove {

	public RandomPosterSelection posterSelection;

	private Image poster;
	private const string SIGN_OBJ = "sign";
	private const string CANVAS_OBJ = "Canvas";
	private const string IMAGE_OBJ = "Image";

	private void Start(){
		poster = transform.Find(SIGN_OBJ).Find(CANVAS_OBJ).Find(IMAGE_OBJ).GetComponent<Image>();
	}

	protected override void Update(){
		if (!frozen && GameHasStarted){
			currentSpeed = GetCurrentSpeed();

			transform.localPosition -= Vector3.forward * currentSpeed * Time.deltaTime;

			if (transform.localPosition.z <= resetZ) {

				transform.localPosition = startPos;
				poster.sprite = ChooseNewPoster();
			}
		} else {
			frozen = RunFreezeTimer();
		}
	}


	private Sprite ChooseNewPoster(){
		return posterSelection.posters[Random.Range(0, posterSelection.posters.Count)];
	}
}
