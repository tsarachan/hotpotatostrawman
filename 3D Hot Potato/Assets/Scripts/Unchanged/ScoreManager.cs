/*
 * 
 * This script tracks various metrics about players, and then displays the one on which each player did best
 * at the end of a world.
 * 
 * Each metric is an object of the Metric class. That object contains, among other things, a baseline good score on
 * that metric, as well as player 1's and 2's performance.
 * 
 * When a world ends, this script measures each player's performance on each metric relative to the baseline good
 * score, and displays one on which the player did best.
 * 
 * To add a new metric, do the following:
 * 1. Go down to the metrics region below, and create a new variable for each of the five things a metric needs.
 * Instructions are in the region's comments.
 * 
 * 2. Add the metric to the metrics dictionary, using the syntax in Start().
 * 
 */
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ScoreManager : MonoBehaviour {


	private Dictionary<string, Metric> metrics = new Dictionary<string, Metric>();

	private const char HIGH_SCORE_GOOD = '>';
	private const char LOW_SCORE_GOOD = '<';

	//nonsense value for initialization
	private const int VERY_LOW = -10000;

	//UI elements for displaying scores at the end of a world
	private Transform p1ScoreDisplay;
	private Text p1MetricName;
	private Text p1MetricMessage;
	private const string P1_SCORE_OBJ = "P1 score display";
	private Transform p2ScoreDisplay;
	private Text p2MetricName;
	private Text p2MetricMessage;
	private const string P2_SCORE_OBJ = "P2 score display";
	private const string SCORE_CANVAS = "Score canvas";
	private const string NAME_OBJ = "Metric name";
	private const string MESSAGE_OBJ = "Metric message";

	//timers and locations for bringing the UI elements into view when a world ends
	private bool worldOver = false;
	public float moveOnScreenDuration = 1.0f;
	private float timer = 0.0f;
	private Vector3 p1ScoreDisplayStart = new Vector3(0.0f, 0.0f, 0.0f);
	public Vector3 p1ScoreDisplayEnd = new Vector3(0.0f, 0.0f, 0.0f);
	private Vector3 p2ScoreDisplayStart = new Vector3(0.0f, 0.0f, 0.0f);
	public Vector3 p2ScoreDisplayEnd = new Vector3(0.0f, 0.0f, 0.0f);
	public AnimationCurve p1ScoreDisplayCurve;
	public AnimationCurve p2ScoreDisplayCurve;

	#region metrics
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
	private const int TEAM_PLAYER_DEFAULT_SCORE = 1;
	private const string TEAM_PLAYER_NAME = "Team Player";
	private const string TEAM_PLAYER_MESSAGE = " passes";
	private const char TEAM_PLAYER_COMPARATOR = HIGH_SCORE_GOOD;


	//default accomplishment for players who get through a world
	private const string SURVIVOR = "survive the world";
	private const int SURVIVOR_DEFAULT_SCORE = 0; //this is never used--nothing adds to it
	private const string SURVIVOR_NAME = "Survivor";
	private const string SURVIVOR_MESSAGE = " deaths";
	private const char SURVIVOR_COMPARATOR = HIGH_SCORE_GOOD;

	#endregion

	//initialize variables and set up dictiontary of metrics to track
	private void Start(){
		//set up the UI elements that display metrics at the end of a world
		p1ScoreDisplay = GameObject.Find(P1_SCORE_OBJ).transform;
		p1ScoreDisplayStart = p1ScoreDisplay.position;
		p1MetricName = p1ScoreDisplay.Find(SCORE_CANVAS).Find(NAME_OBJ).GetComponent<Text>();
		p1MetricMessage = p1ScoreDisplay.Find(SCORE_CANVAS).Find(MESSAGE_OBJ).GetComponent<Text>();

		p2ScoreDisplay = GameObject.Find(P2_SCORE_OBJ).transform;
		p2ScoreDisplayStart = p2ScoreDisplay.position;
		p2MetricName = p2ScoreDisplay.Find(SCORE_CANVAS).Find(NAME_OBJ).GetComponent<Text>();
		p2MetricMessage = p2ScoreDisplay.Find(SCORE_CANVAS).Find(MESSAGE_OBJ).GetComponent<Text>();

		//add all metrics to the dictionary
		metrics.Add(TEAM_PLAYER, new Metric(TEAM_PLAYER_DEFAULT_SCORE,
											TEAM_PLAYER_NAME,
											TEAM_PLAYER_MESSAGE,
											HIGH_SCORE_GOOD));
		metrics.Add(SURVIVOR, new Metric(SURVIVOR_DEFAULT_SCORE,
										 SURVIVOR_NAME,
										 SURVIVOR_MESSAGE,
										 HIGH_SCORE_GOOD));
	}

	/// <summary>
	/// When the world is over, scroll the UI elements onto the screen.
	/// </summary>
	private void Update(){
		if (worldOver){
			timer += Time.deltaTime;

			p1ScoreDisplay.position = Vector3.Lerp(p1ScoreDisplayStart,
												   p1ScoreDisplayEnd,
												   p1ScoreDisplayCurve.Evaluate(timer/moveOnScreenDuration));
			p2ScoreDisplay.position = Vector3.Lerp(p2ScoreDisplayStart,
												   p2ScoreDisplayEnd,
												   p2ScoreDisplayCurve.Evaluate(timer/moveOnScreenDuration));
		}

		//for debugging only; remove when the script is ready!
//		if (Input.GetKeyDown(KeyCode.Space)){
//			FindBestPerformances();
//		}
	}


	/// <summary>
	/// Call this whenever a player scores on a metric where a high score is better.
	/// 
	/// IMPORTANT: this method assumes that the player object names end in "1" and "2."
	/// </summary>
	/// <param name="category">The metric on which the player scored.</param>
	/// <param name="playerName">The name of the player's gameobject.</param>
	public void Score(string category, string playerName){
		//error check: make sure the type of score is valid
		if (!metrics.ContainsKey(category)){
			Debug.Log("Illegal category: " + category);
			return;
		}

		//this is a category where the game should keep a running tally of a high score, so add to the tally
		if (metrics[category].Comparator == HIGH_SCORE_GOOD){
			//add to the appropriate player's score
			if (playerName.Last() == '1'){
				metrics[category].P1Value++;
			} else if (playerName.Last() == '2'){
				metrics[category].P2Value++;
			} else {
				Debug.Log("Illegal playerName: " + playerName);
			}
		}
	}


	/// <summary>
	/// Call this when a player scores on a metric where a low score is better.
	/// 
	/// IMPORTANT: this method assumes that the player object names end in "1" and "2."
	/// </summary>
	/// <param name="category">The metric on which the player scored.</param>
	/// <param name="playerName">The name of the player's gameobject.</param>
	/// <param name="value">The value that might be a new low score.</param>
	public void Score(string category, string playerName, int value){
		//error check: make sure the type of score is valid
		if (!metrics.ContainsKey(category)){
			Debug.Log("Illegal category: " + category);
			return;
		}

		//this is a category where low scores are good (golf scoring), so only change the value if it's lower
		if (playerName.Last() == '1'){
			if (metrics[category].P1Value > value){
				metrics[category].P1Value = value;
			}
		} else if (playerName.Last() == '2'){
			if (metrics[category].P2Value > value){
				metrics[category].P2Value = value;
			}
		} else {
			Debug.Log("Illegal playerName: " + playerName);
		}
	}
		

	/// <summary>
	/// Call this when a world ends to determine what metric each player did best on, and then display them.
	/// </summary>
	public void FindBestPerformances(){
		string p1BestPerformance = FindPlayerBest('1');
		string p2BestPerformance = FindPlayerBest('2');

		p1MetricName.text = metrics[p1BestPerformance].SuccessName;
		p1MetricMessage.text = metrics[p1BestPerformance].P1Value + metrics[p1BestPerformance].SuccessMessage;

		p2MetricName.text = metrics[p2BestPerformance].SuccessName;
		p2MetricMessage.text = metrics[p2BestPerformance].P2Value + metrics[p2BestPerformance].SuccessMessage;

		worldOver = true;
	}


	/// <summary>
	/// Determines which metric a player performed best on.
	/// </summary>
	/// <returns>The player's best metric, or "survivor" if the player didn't beat any baseline good scores.</returns>
	/// <param name="playerNum">The player's number, 1 or 2.</param>
	private string FindPlayerBest(char playerNum){
		string bestCategory = "";
		int delta = VERY_LOW;

		if (playerNum == '1'){
			//check each the value for each category against the "good" score for that category
			//if it's a category where a high score is good, look for performances above the good score
			//if a low score is good, look for performances below the good score
			foreach (string category in metrics.Keys){
				if (metrics[category].Comparator == HIGH_SCORE_GOOD){
					if (metrics[category].P1Value > metrics[category].GoodValue &&
						metrics[category].P1Value > delta){
						bestCategory = category;
						delta = metrics[category].P1Value - metrics[category].GoodValue;
					}
				} else if (metrics[category].Comparator == LOW_SCORE_GOOD){
					if (metrics[category].P1Value < metrics[category].GoodValue &&
						metrics[category].P1Value < delta){
						bestCategory = category;
						delta = metrics[category].GoodValue - metrics[category].P1Value;
					}
				}
			}
		}
		//do the same for player 2
		else if (playerNum == '2'){
			foreach (string category in metrics.Keys){
				if (metrics[category].Comparator == HIGH_SCORE_GOOD){
					if (metrics[category].P2Value > metrics[category].GoodValue &&
						metrics[category].P2Value > delta){
						bestCategory = category;
						delta = metrics[category].P2Value - metrics[category].GoodValue;
					}
				} else if (metrics[category].Comparator == LOW_SCORE_GOOD){
					if (metrics[category].P2Value < metrics[category].GoodValue &&
						metrics[category].P2Value < delta){
						bestCategory = category;
						delta = metrics[category].GoodValue - metrics[category].P2Value;
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


	/// <summary>
	/// This private class creates objects containing all the necessary information about each metric.
	/// </summary>
	private class Metric {
		public int P1Value { get; set; }
		public int P2Value { get; set; }
		public int GoodValue { get; private set; }
		public char Comparator { get; set; } //is a high score good, or a low score?

		public string SuccessName = "";
		public string SuccessMessage = "";


		public Metric(int goodValue, string successName, string successMessage, char comparator){
			this.GoodValue = goodValue;
			this.SuccessName = successName;
			this.SuccessMessage = successMessage;
			this.Comparator = comparator;

			//default initializations
			this.P1Value = 0;
			this.P2Value = 0;
		}
	}
}
