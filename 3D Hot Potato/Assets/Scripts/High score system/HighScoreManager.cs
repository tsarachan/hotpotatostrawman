using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;


public class HighScoreManager : MonoBehaviour {



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



	public int numEntries = 10;
	public List<Entry> entries;
	private const string HIGH_SCORE_HEADER = "HighScore";
	private string nameEntry;
	private string scoreEntry;


	/// <summary>
	/// Populates the list of high scores.
	/// </summary>
	private void Start(){
		highScoreText = transform.Find(CANVAS_OBJ).Find(HIGH_SCORES_OBJ).GetComponent<TextMeshProUGUI>();
		entries = new List<Entry>();
		string key;

		for (int i = 0; i < numEntries; i++){
			key = HIGH_SCORE_HEADER + i.ToString();

			if (PlayerPrefs.HasKey(key + "score")){
				entries.Add(new Entry(PlayerPrefs.GetString(key + "name"), PlayerPrefs.GetInt(key + "score")));
			} else {
				entries.Add(new Entry("rck&rll", 0));
			}
		}
	}


	public IEnumerator DisplayScore(){
		highScoreText.gameObject.SetActive(true);
		PrintScores();
		yield return new WaitForSeconds(5.0f);
		//load a new scene

		yield break;
	}


	private void PrintScores() {
		string scoreList = "";
		foreach(Entry entry in entries) {
			scoreList = string.Concat(scoreList, entry.GetEntry(), "\n");
		}
		highScoreText.text = scoreList;
	}


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


	private void SetScores(){
		string key;

		for (int i = 0; i < entries.Count; i++){
			key = HIGH_SCORE_HEADER + i.ToString();
			PlayerPrefs.SetString(key + "name", entries[i].Name);
			PlayerPrefs.SetInt(key + "score", entries[i].Score);
		}
	}


	private void OnApplicationQuit(){
		PlayerPrefs.Save();
	}

	//
//	public class Entry {
//		public string name;
//		public int score;
//		public Entry(string newName, int newScore) {
//			name = newName;
//			score = newScore;
//		}
//		public string GetEntry() {
//			if(score != 0) {
//				return string.Concat(name, " ", score.ToString());
//			}
//			else {
//				return string.Concat(name, " ----");
//			}
//		}
//		public string scoreVal() {
//			return score.ToString();
//		}
//	}


//	public Text HighScoreDisplay;
//	public int numEntries = 10;
//	public List<Entry> entries;
//	private string HighScoreHeader;
//	private string nameEntry;
//	private int scoreEntry;
//	bool enteringScore;



//	void Start () {
//		enteringScore = false;
//		HighScoreHeader = SceneManager.GetActiveScene().name + "HighScore";
//		entries = new List<Entry>();
//		string key;
//		for(int i = 0; i < numEntries; i++) {
//			key = HighScoreHeader + i.ToString();
//			if(PlayerPrefs.HasKey(key + "score")) {
//				entries.Add(new Entry(PlayerPrefs.GetString(key + "name"), PlayerPrefs.GetInt(key + "score")));
//			}
//			else {
//				entries.Add(new Entry("Sam", 0));
//			}
//		}
//	}


//	public void SetScores() {
//		string key;
//		for(int i = 0; i < numEntries; i++) {
//			key = HighScoreHeader + i.ToString();
//			PlayerPrefs.SetString(key + "name", entries[i].name);
//			PlayerPrefs.SetInt(key + "score", entries[i].score);
//		}
//	}
//
//
//	public void PrintScores() {
//		string scoreList = "";
//		foreach(Entry entry in entries) {
//			scoreList = string.Concat(scoreList, entry.GetEntry(), "\n");
//		}
//		HighScoreDisplay.text = scoreList;
//	}
//
//
//	public void ClearLeaderBoard() {
//		string key;
//		for(int i = 0; i < numEntries; i++) {
//			key = HighScoreHeader + i.ToString();
//			PlayerPrefs.DeleteKey(key + "name");
//			PlayerPrefs.DeleteKey(key + "score");
//		}
//	}
//
//
//	void OnApplicationQuit()
//	{
//		PlayerPrefs.Save();
//	}
//
//
//	public bool MadeHighScoreList(int score) {
//		return score > entries[numEntries -1].score;
//	}
//
//
//	public void CreateNewEntry(string name, int score) {
//		Entry newChallenger = new Entry(name, score);
//		for(int i = 0; i < entries.Count; i++) {
//			if(entries[i].score < newChallenger.score) {
//				entries.Insert(i, newChallenger);
//				break;
//			}
//		}
//		SetScores();
//		entries.Remove(entries[numEntries]);
//	}
//
//
//	public void InputNewName(int score) {
//		enteringScore = true;
//		scoreEntry = score;
//	}
//
//
//	IEnumerator DisplayAndReset() {
//		PrintScores();
//		yield return new WaitForSeconds(5);
//		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
//	}
}