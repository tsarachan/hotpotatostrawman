namespace MagnetizingBoss
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public class MagneticArmBehavior : MonoBehaviour {

		private MagnetizingBossBehavior bossScript;


		//used to determine whether this arm has contacted the player it's attracting
		private char myTarget = 'z';
		private const string PLAYER_TAG = "Player";


		private void Start(){
			bossScript = transform.parent.parent.GetComponent<MagnetizingBossBehavior>();
			myTarget = gameObject.name[1];
		}


		private void OnCollisionEnter(Collision collision){
			if(collision.gameObject.tag == PLAYER_TAG){
				if (collision.gameObject.name.Last() == myTarget){
					collision.gameObject.GetComponent<PlayerEnemyInteraction>().LoseTheGame();
				} else {
					bossScript.PlayerBlock(myTarget);
				}
			}
		}
	}
}
