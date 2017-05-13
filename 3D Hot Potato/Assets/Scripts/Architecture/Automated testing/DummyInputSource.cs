namespace Testing
{
	using UnityEngine;
	using UnityEngine.SceneManagement;

	public class DummyInputSource : MonoBehaviour {


		////////////////////////////////////////////////////////////////////////
		/// Make this a singleton
		////////////////////////////////////////////////////////////////////////
		private static DummyInputSource instance;

		////////////////////////////////////////////////////////////////////////
		/// Timer for faking input
		////////////////////////////////////////////////////////////////////////
		[SerializeField] private float inputDelay = 30.0f;
		[SerializeField] private float currentInputDelay = 0.0f;
		[SerializeField] private float timer = 0.0f;
		[SerializeField] private float delayVariance = 5.0f;


		////////////////////////////////////////////////////////////////////////
		/// Scene names
		////////////////////////////////////////////////////////////////////////
		private const string TITLE_SCENE = "TitleScene3";
		private const string GAME_SCENE = "Level 1";
		private const string HIGH_SCORE_SCENE = "High score scene";


		////////////////////////////////////////////////////////////////////////
		/// Objects in the game scene; used to imitate the effects of an input
		////////////////////////////////////////////////////////////////////////
		private const string UI_CANVAS_OBJ = "UI canvas";
		private const string CONTROLLER_MAP_OBJ = "Controller map";
		private const string PLAYER_1_OBJ = "Player 1";



		private void Awake(){
			if (instance){
				Destroy(gameObject);
			} else {
				instance = this;
				currentInputDelay = inputDelay;
				DontDestroyOnLoad(gameObject);
			}
		}


		private void Update(){
			timer += Time.unscaledDeltaTime;

			if (timer >= currentInputDelay){
				timer = 0.0f;
				currentInputDelay = inputDelay + Random.Range(-delayVariance, delayVariance);

				string currentScene = SceneManager.GetActiveScene().name;
				Debug.Log("faking an input; currentScene == " + currentScene);

				if (currentScene == TITLE_SCENE){
					SceneManager.LoadScene(GAME_SCENE);
				} else if (currentScene == GAME_SCENE){
					ChooseRandomGameInput();
				} else if (currentScene == HIGH_SCORE_SCENE){
					SceneManager.LoadScene(TITLE_SCENE);
				}
			}
		}


		private void ChooseRandomGameInput(){
			int randomAction = Random.Range(0, 2);

			switch (randomAction){
				case 0:
				Debug.Log("Chose to pause/start");
					GameObject.Find(UI_CANVAS_OBJ).transform.Find(CONTROLLER_MAP_OBJ)
						.GetComponent<PauseMenuBehavior>().ChangePauseMenuState();
					break;
				case 1:
					Debug.Log("Chose to pass");
					if (!GameObject.Find(UI_CANVAS_OBJ).transform.Find(CONTROLLER_MAP_OBJ)
					.GetComponent<PauseMenuBehavior>().Paused){
						GameObject.Find(PLAYER_1_OBJ).GetComponent<PlayerBallInteraction>().Throw();
					} else {
						GameObject.Find(UI_CANVAS_OBJ).transform.Find(CONTROLLER_MAP_OBJ)
							.GetComponent<PauseMenuBehavior>().ChangePauseMenuState();
					}
					break;
			}
		}
	}
}
