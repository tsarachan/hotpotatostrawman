using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ScoreManager : MonoBehaviour {


	private Dictionary<string, ScoreCategory> scoreCategories = new Dictionary<string, ScoreCategory>();

	private const string NUMBER_OF_PASSES = "number of passes";
	private const int goodNumberOfPasses = 10;

	private void Start(){
		scoreCategories.Add(NUMBER_OF_PASSES, new ScoreCategory(goodNumberOfPasses));
	}


	public void Score(string category, string playerName){
		//error check: make sure the type of score is valid
		if (!scoreCategories.ContainsKey(category)){
			Debug.Log("Illegal category: " + category);
			return;
		}

		//add to the appropriate player's score
		if (playerName.Last() == '1'){
			scoreCategories[category].P1Value++;
		} else if (playerName.Last() == '2'){
			scoreCategories[category].P2Value++;
		} else {
			Debug.Log("Illegal playerName: " + playerName);
		}
	}


	private class ScoreCategory {
		public int P1Value { get; set; }
		public int P2Value { get; set; }
		private int goodValue = 0;

		public ScoreCategory(int goodValue){
			this.goodValue = goodValue;
		}
	}
}
