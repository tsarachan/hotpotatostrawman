using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScoreManager : MonoBehaviour {


	private Dictionary<string, ScoreCategory> scoreCategories = new Dictionary<string, ScoreCategory>();



	private class ScoreCategory {
		public int P1Value { get; set; }
		public int P2Value { get; set; }
		private int goodValue = 0;

		public ScoreCategory(int goodValue){
			this.goodValue = goodValue;
		}
	}
}
