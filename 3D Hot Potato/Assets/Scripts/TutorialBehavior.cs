using System.Collections; 
using System.Collections.Generic; 
using UnityEngine; 
using UnityEngine.UI; 

public class TutorialBehavior : MonoBehaviour { 


	//the UI this uses to display text 
	private Text tutorialText; 
	private const string TUTORIAL_TEXT_OBJ = "Tutorial text"; 


	//players 
	private const string PLAYER_1_OBJ = "Player 1"; 
	private const string PLAYER_2_OBJ = "Player 2"; 


	//inputs that the tutorial can hear 
	private const string P1_O_BUTTON = "PS4_O_1"; 
	private const string P2_O_BUTTON = "PS4_O_2";
	private KeyCode p1Pass = KeyCode.Z;
	private KeyCode p2Pass = KeyCode.N; 


	private void Start(){ 
		tutorialText = GameObject.Find(TUTORIAL_TEXT_OBJ).GetComponent<Text>(); 
	} 


	public IEnumerator StartPassTutorial(){ 

		//stop time 
		Time.timeScale = 0.0f; 


		//display tutorial text 
		tutorialText.text = "Press circle button to pass"; 
		tutorialText.gameObject.SetActive(true); 


		PlayerBallInteraction p1BallScript = GameObject.Find(PLAYER_1_OBJ).GetComponent<PlayerBallInteraction>(); 
		PlayerBallInteraction p2BallScript = GameObject.Find(PLAYER_2_OBJ).GetComponent<PlayerBallInteraction>(); 


		bool readyToContinue = false; 

		//wait for the player with the ball to pass 
		while (!readyToContinue){ 
			if ((p1BallScript.BallCarrier && (Input.GetButtonDown(P1_O_BUTTON) || Input.GetKeyDown(p1Pass))) || 
				(p2BallScript.BallCarrier && (Input.GetButtonDown(P2_O_BUTTON) || Input.GetKeyDown(p2Pass)))){ 
				readyToContinue = true; 
			} 

			yield return null; 
		} 


		//remove tutorial text 
		tutorialText.gameObject.SetActive(false); 


		//restart time 
		Time.timeScale = 1.0f; 

		yield break; 
	} 
}
