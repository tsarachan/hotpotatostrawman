using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneBehavior : MonoBehaviour {

	private const string GAME_SCENE = "Level 1";

	private void Update(){
		if (Input.anyKeyDown){
			SceneManager.LoadScene(GAME_SCENE);
		}
	}
}
