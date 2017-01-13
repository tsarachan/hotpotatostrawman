using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ScoreManager : MonoBehaviour {


	private Dictionary<string, ScoreCategory> scoreCategories = new Dictionary<string, ScoreCategory>();

	private const char HIGH_SCORE_GOOD = '>';
	private const char LOW_SCORE_GOOD = '<';

	//nonsense value for initialization
	private const int VERY_LOW = -10000;


	//---------------METRICS---------------
	/*
	 * 
	 * Below are all the metrics the game tracks for each player.
	 * 
	 * Each metric has five components:
	 * - a descriptive name that's also used to index the dictionary of metrics
	 * - a default "good" score the player's achievement on this metric is measured against
	 * - a flashy name for displaying to players
	 * - a char that tells the system whether high scores are good for this metric ('>') or low scores ('<')
	 * 
	 */
	private const string TEAM_PLAYER = "number of passes";
	private const int TEAM_PLAYER_DEFAULT_SCORE = 10;
	private const string TEAM_PLAYER_NAME = "Team Player";
	private const string TEAM_PLAYER_MESSAGE = " passes";
	private const char TEAM_PLAYER_COMPARATOR = '>';


	//default accomplishment for players who get through a world
	private const string SURVIVOR = "survive the world";
	private const int SURVIVOR_DEFAULT_SCORE = 0; //this is never used--nothing adds to it
	private const string SURVIVOR_NAME = "Survivor";
	private const string SURVIVOR_MESSAGE = " deaths";
	private const char SURVIVOR_COMPARATOR = '>';


	private void Start(){
		scoreCategories.Add(TEAM_PLAYER, new ScoreCategory(TEAM_PLAYER_DEFAULT_SCORE,
														   TEAM_PLAYER_NAME,
														   TEAM_PLAYER_MESSAGE,
														   HIGH_SCORE_GOOD));
	}

	//for debugging only; remove when the script is ready!
	private void Update(){
		if (Input.GetKeyDown(KeyCode.Space)){
			FindBestPerformances();
		}
	}


	public void Score(string category, string playerName){
		//error check: make sure the type of score is valid
		if (!scoreCategories.ContainsKey(category)){
			Debug.Log("Illegal category: " + category);
			return;
		}

		//this is a category where the game should keep a running tally of a high score, so add to the tally
		if (scoreCategories[category].Comparator == HIGH_SCORE_GOOD){
			//add to the appropriate player's score
			if (playerName.Last() == '1'){
				scoreCategories[category].P1Value++;
			} else if (playerName.Last() == '2'){
				scoreCategories[category].P2Value++;
			} else {
				Debug.Log("Illegal playerName: " + playerName);
			}
		}
	}

	public void Score(string category, string playerName, int value){
		//error check: make sure the type of score is valid
		if (!scoreCategories.ContainsKey(category)){
			Debug.Log("Illegal category: " + category);
			return;
		}

		//this is a category where low scores are good (golf scoring), so only change the value if it's lower
		if (playerName.Last() == '1'){
			if (scoreCategories[category].P1Value > value){
				scoreCategories[category].P1Value = value;
			}
		} else if (playerName.Last() == '2'){
			if (scoreCategories[category].P2Value > value){
				scoreCategories[category].P2Value = value;
			}
		} else {
			Debug.Log("Illegal playerName: " + playerName);
		}
	}


	private class ScoreCategory {
		public int P1Value { get; set; }
		public int P2Value { get; set; }
		public int GoodValue { get; private set; }
		public char Comparator { get; set; } //is a high score good, or a low score?

		private string successName = "";
		private string successMessage = "";


		public ScoreCategory(int goodValue, string successName, string successMessage, char comparator){
			this.GoodValue = goodValue;
			this.successName = successName;
			this.successMessage = successMessage;
			this.Comparator = comparator;
		}
	}


	public void FindBestPerformances(){
		string p1BestPerformance = FindPlayerBest('1');
		//string p2BestPerformance = FindPlayerBest('2');

		if (p1BestPerformance != SURVIVOR){
			Debug.Log("p1 best: " + p1BestPerformance + ", with value of " + scoreCategories[p1BestPerformance].P1Value);
		} else if (p1BestPerformance == SURVIVOR){
			Debug.Log("You made it!");
		}
	}

	private string FindPlayerBest(char playerNum){
		string bestCategory = "";
		int delta = VERY_LOW;

		if (playerNum == '1'){
			//check each the value for each category against the "good" score for that category
			//if it's a category where a high score is good, look for performances above the good score
			//if a low score is good, look for performances below the good score
			foreach (string category in scoreCategories.Keys){
				if (scoreCategories[category].Comparator == HIGH_SCORE_GOOD){
					if (scoreCategories[category].P1Value > scoreCategories[category].GoodValue &&
						scoreCategories[category].P1Value > delta){
						bestCategory = category;
						delta = scoreCategories[category].P1Value - scoreCategories[category].GoodValue;
					}
				} else if (scoreCategories[category].Comparator == LOW_SCORE_GOOD){
					if (scoreCategories[category].P1Value < scoreCategories[category].GoodValue &&
						scoreCategories[category].P1Value < delta){
						bestCategory = category;
						delta = scoreCategories[category].GoodValue - scoreCategories[category].P1Value;
					}
				}
			}
		}
		//do the same for player 2
		else if (playerNum == '2'){
			foreach (string category in scoreCategories.Keys){
				if (scoreCategories[category].Comparator == HIGH_SCORE_GOOD){
					if (scoreCategories[category].P2Value > scoreCategories[category].GoodValue &&
						scoreCategories[category].P2Value > delta){
						bestCategory = category;
						delta = scoreCategories[category].P2Value - scoreCategories[category].GoodValue;
					}
				} else if (scoreCategories[category].Comparator == LOW_SCORE_GOOD){
					if (scoreCategories[category].P2Value < scoreCategories[category].GoodValue &&
						scoreCategories[category].P2Value < delta){
						bestCategory = category;
						delta = scoreCategories[category].GoodValue - scoreCategories[category].P2Value;
					}
				}
			}
		}

		//check; did the player actually have a score better than the default good scores?
		if (bestCategory != ""){
			return bestCategory;
		}

		//uh-oh! The player didn't have any scores better than the good scores. Give the player a default accomplishment.
		return SURVIVOR;
	}
	//eof
}
