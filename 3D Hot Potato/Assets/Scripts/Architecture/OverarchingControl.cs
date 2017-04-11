using UnityEngine;

public class OverarchingControl : MonoBehaviour {

	private void Awake(){
		Services.EventManager = new EventManager();

		ScoreRepository.Score = 0;
	}
}
