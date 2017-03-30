namespace MagnetizingBoss
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public class MagneticArmBehavior : MonoBehaviour {

		private MagnetizingBossBehavior bossScript;


		//used to change the color of the magnet based on the stage of the boss fight
		public enum Materials { Player1, Player2, Repelling, Vulnerable };
		public Material[] materials;
		private Material magnetMaterial;
		private const string MAGNET_OBJ = "Magnet";


		//used to determine whether this arm has contacted the player it's attracting
		private char myTarget = '1';
		private const string PLAYER_TAG = "Player";


		private void Start(){
			bossScript = transform.parent.parent.GetComponent<MagnetizingBossBehavior>();
			magnetMaterial = transform.Find(MAGNET_OBJ).GetComponent<Renderer>().material;
		}


		private void OnCollisionEnter(Collision collision){
			if(collision.gameObject.tag == PLAYER_TAG){
				if (collision.gameObject.name.Last() == myTarget){
					collision.gameObject.GetComponent<PlayerEnemyInteraction>().LoseTheGame();
				} else {
					bossScript.PlayerBlock(myTarget);
					myTarget = ChangeTargets();
				}
			}
		}


		private char ChangeTargets(){
			if (myTarget == '1'){
				return '2';
			} else {
				return '1';
			}
		}


		public void ChangeMaterial(Materials nextMat){
			transform.Find(MAGNET_OBJ).GetComponent<Renderer>().material = materials[(int)nextMat];
		}
	}
}
