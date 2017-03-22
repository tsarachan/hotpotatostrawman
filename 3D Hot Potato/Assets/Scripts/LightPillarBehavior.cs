using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightPillarBehavior : MonoBehaviour {

	//----------Tunable variables----------
	public float extendDuration = 1.0f;
	public float startHeight = 10.0f;


	//----------Internal variables----------

	private Transform player;

	private Transform start;
	private Transform end;
	private const string START_OBJ = "Start";
	private const string END_OBJ = "End";

	private Vector3 origin = new Vector3(0.0f, 0.0f, 0.0f);

	private float timer = 0.0f;

	private LineRenderer lineRenderer;


	private const string CENTER_OBJ = "Center beam";


	public void Setup(Transform player){
		start = transform.Find(START_OBJ);
		end = transform.Find(END_OBJ);
		this.player = player;
		start.position = player.position + new Vector3(0.0f, startHeight, 0.0f);
		end.position = start.position;
		origin = start.position;

		timer = 0.0f;

		lineRenderer = GetComponent<LineRenderer>();

		if (gameObject.name != CENTER_OBJ){
			transform.Find(CENTER_OBJ).GetComponent<LightPillarBehavior>().Setup(player);
			StartCoroutine(transform.Find(CENTER_OBJ).GetComponent<LightPillarBehavior>().ShineDown());
		}
	}


	public IEnumerator ShineDown(){
		gameObject.SetActive(true);

		while (timer <= extendDuration * 2.0f){
			timer += Time.deltaTime;

			if (timer < extendDuration){
				end.position = Vector3.Lerp(origin, player.position, timer/extendDuration);
			} else if (timer <= extendDuration * 2.0f) {
				end.position = player.position;
				start.position = Vector3.Lerp(origin, end.position, (timer - extendDuration)/extendDuration);
			}

			lineRenderer.SetPosition(0, start.position);
			lineRenderer.SetPosition(1, end.position);

			yield return null;
		}

		gameObject.SetActive(false);

		yield break;
	}
}
