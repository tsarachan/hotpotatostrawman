namespace TitleScene
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class TitleSceneBallBehavior : MonoBehaviour {

		private const string PLAYER_1_OBJ = "Player 1";
		private const string CYCLE_OBJ = "Cycle and rider";


		private void Start(){
			transform.parent = GameObject.Find(PLAYER_1_OBJ).transform.Find(CYCLE_OBJ);
		}
	}
}
