using UnityEngine;
using System.Collections;

public class TimeLimit : MonoBehaviour {

	public float lifetime = 1.0f;
	private float timer = 0.0f;

	private void Update(){
		timer += Time.deltaTime;

		if (timer >= lifetime){
			Destroy(gameObject);
		}
	}
}
