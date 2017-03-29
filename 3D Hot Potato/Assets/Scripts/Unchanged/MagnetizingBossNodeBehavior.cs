namespace MagnetizingBoss
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public class MagnetizingBossNodeBehavior : MonoBehaviour {

		private MagnetizingBossBehavior bossScript;

		//used to determine whether the correct player is attacking
		private const char myName = 'n'; //transmitted to the boss script so it can identify which object has been struck
		private const string PLAYER_TAG = "Player";
		private PlayerBallInteraction p1BallScript;
		private PlayerBallInteraction p2BallScript;
		private const string PLAYER_1_OBJ = "Player 1";
		private const string PLAYER_2_OBJ = "Player 2";


		private void Start(){
			bossScript = transform.parent.GetComponent<MagnetizingBossBehavior>();
			p1BallScript = GameObject.Find(PLAYER_1_OBJ).GetComponent<PlayerBallInteraction>();
			p2BallScript = GameObject.Find(PLAYER_2_OBJ).GetComponent<PlayerBallInteraction>();
		}


		private void OnCollisionEnter(Collision collision){
			if (collision.gameObject.tag == PLAYER_TAG){
				if (collision.gameObject.name.Last() == '1' &&
					p1BallScript.BallCarrier){
					bossScript.PlayerBlock(myName);
				} else if (collision.gameObject.name.Last() == '2' &&
						   p2BallScript.BallCarrier){
						   bossScript.PlayerBlock(myName);
				}
			}
		}
	}
}
