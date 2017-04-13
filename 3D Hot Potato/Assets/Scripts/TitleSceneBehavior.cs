using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneBehavior : MonoBehaviour {

	private const string GAME_SCENE = "Level 1";

	private const string O_BUTTON = "PS4_O_";
	private const string TRIANGLE_BUTTON = "PS4_Triangle_";
	private const string X_BUTTON = "PS4_X_";
	private const string SQUARE_BUTTON = "PS4_Square_";

	private const string P1 = "1";
	private const string P2 = "2";


	private void Update(){
		if (Input.anyKeyDown ||
			Input.GetButtonDown(O_BUTTON + P1) ||
			Input.GetButtonDown(X_BUTTON + P1) ||
			Input.GetButtonDown(SQUARE_BUTTON + P1) ||
			Input.GetButtonDown(TRIANGLE_BUTTON + P1) ||
			Input.GetButtonDown(O_BUTTON + P2) ||
			Input.GetButtonDown(X_BUTTON + P2) ||
			Input.GetButtonDown(SQUARE_BUTTON + P2) ||
			Input.GetButtonDown(TRIANGLE_BUTTON + P2))
		{
			SceneManager.LoadScene(GAME_SCENE);
		} 
	}
}
