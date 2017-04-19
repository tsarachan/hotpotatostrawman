using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;


public class HighScoreManager : MonoBehaviour {


	//----------Tunable variables----------

	public float highScoreDisplayTime = 5.0f;



	//----------Internal variables----------


	//class for entries in the high score list
	public class Entry{
		public string Name { get; set; }
		public int Score { get; set; }

		public Entry(string name, int score){
			Name = name;
			Score = score;
		}


		public string GetEntry(){
			if (Score != 0){
				return string.Concat(Name, " ", Score.ToString());
			} else {
				return string.Concat(Name, " ", "----");
			}
		}


		public string ScoreAsString(){
			return Score.ToString();
		}
	}


	//the object that displays the high score list
	private TextMeshProUGUI highScoreText;
	private const string CANVAS_OBJ = "Canvas";
	private const string HIGH_SCORES_OBJ = "High scores text";


	//the name entry UI
	private GameObject nameEntryUI;
	private const string NAME_ENTRY_UI_OBJ = "Name entry UI";


	//the list of high scores
	public int numEntries = 10; //the length of the high score list
	public List<Entry> entries;


	//used to label PlayerPrefs keys
	private const string HIGH_SCORE_HEADER = "HighScore";


	//the name entry system, so that this can manage when it's turned on or off
	private NameEntrySystem nameEntrySystem;


	//the title scene, to return to when done displaying high scores
	private const string TITLE_SCENE = "TitleScene3";


	/// <summary>
	/// Populates the list of high scores, and then determine whether the players should enter their names, or
	/// whether the system should skip ahead to displaying the current high scores.
	/// </summary>
	private void Start(){
		highScoreText = transform.Find(CANVAS_OBJ).Find(HIGH_SCORES_OBJ).GetComponent<TextMeshProUGUI>();
		entries = new List<Entry>();
		nameEntrySystem = GetComponent<NameEntrySystem>();
		nameEntryUI = transform.Find(CANVAS_OBJ).Find(NAME_ENTRY_UI_OBJ).gameObject;


		string key;

		for (int i = 0; i < numEntries; i++){
			key = HIGH_SCORE_HEADER + i.ToString();

			if (PlayerPrefs.HasKey(key + "score")){
				entries.Add(new Entry(PlayerPrefs.GetString(key + "name"), PlayerPrefs.GetInt(key + "score")));
			} else {
				entries.Add(new Entry("rck & rll", 0));
			}
		}


		//if the players have a high score, allow them to enter their names, and shut off the high score list until
		//they do. If they do not, shut off the name entry system and display the high score list
		if (CheckIfEnterNames()){
			nameEntryUI.SetActive(true);
			highScoreText.gameObject.SetActive(false);
		} else {
			nameEntryUI.gameObject.SetActive(false);
			StartCoroutine(DisplayScore());
		}
	}


	/// <summary>
	/// Used to determine whether the players have a high score, such that they should get to enter their names.
	/// </summary>
	/// <returns><c>true</c>if the players' score is better than the lowest high score in the list,
	/// <c>false</c> otherwise.</returns>
	private bool CheckIfEnterNames(){
		if (ScoreRepository.Score > entries[entries.Count - 1].Score){
			return true;
		} else {
			return false;
		}
	}


	/// <summary>
	/// This is the master coroutine used to display the high score list. It switches on the high score display,
	/// and then calls PrintScores() to display the entries in the high score list. After five secon
	/// </summary>
	/// <returns>The score.</returns>
	public IEnumerator DisplayScore(){
		highScoreText.gameObject.SetActive(true);
		PrintScores();
		yield return new WaitForSeconds(5.0f);

		SceneManager.LoadScene(TITLE_SCENE);

		yield break;
	}


	/// <summary>
	/// Displays the high scores. Note that this does not format them; that's handled by Entry.GetEntry().
	/// </summary>
	private void PrintScores() {
		string scoreList = "";
		foreach(Entry entry in entries) {
			scoreList = string.Concat(scoreList, entry.GetEntry(), "\n");
		}
		highScoreText.text = scoreList;
	}


	/// <summary>
	/// Adds a high score to the list in the correct place, and then caps the list at [numEntries] entries.
	/// 
	/// NameEntrySystem calls this when name entry is complete.
	/// </summary>
	/// <param name="name">The players' names.</param>
	/// <param name="score">Their score.</param>
	public void ReviseScoreList(string name, int score){
		Entry newChallenger = new Entry(name, score);

		for (int i = 0; i < entries.Count; i++){
			if (newChallenger.Score > entries[i].Score){
				entries.Insert(i, newChallenger);
				break;
			}
		}

		SetScores();
		entries.Remove(entries[numEntries]);
	}


	/// <summary>
	/// Records the high score list in PlayerPrefs.
	/// </summary>
	private void SetScores(){
		string key;

		for (int i = 0; i < entries.Count; i++){
			key = HIGH_SCORE_HEADER + i.ToString();
			PlayerPrefs.SetString(key + "name", entries[i].Name);
			PlayerPrefs.SetInt(key + "score", entries[i].Score);
		}
	}


	/// <summary>
	/// Saves the current high score list to PlayerPrefs when the game closes.
	/// </summary>
	private void OnApplicationQuit(){
		PlayerPrefs.Save();
	}
}