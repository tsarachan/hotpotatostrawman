namespace BossBattle2 {

	using System.Collections;
	using System.Linq;
	using UnityEngine;

	public class ChargeUpPointBehavior : MonoBehaviour {

		private EnemyBossBattle2 battleScript;

		private const string PLAYER_TAG = "Player";


		//a particle effect that switches on when the player is in the charge up point
		private GameObject outerParticles;
		private const string OUTER_PARTICLES = "Outer particles";

		private void Start(){
			battleScript = transform.parent.parent.GetComponent<EnemyBossBattle2>();
			outerParticles = transform.Find(OUTER_PARTICLES).gameObject;
		}


		private void OnTriggerEnter(Collider other){
			if (other.tag == PLAYER_TAG){
				if (other.gameObject.name.Last() == gameObject.name.Last()){
					if (other.gameObject.name.Last() == '1'){
						battleScript.P1Charging = true;
					} else if (other.gameObject.name.Last() == '2'){
						battleScript.P2Charging = true;
					}

					outerParticles.SetActive(true);
				}
			}
		}

		private void OnTriggerExit(Collider other){
			if (other.tag == PLAYER_TAG){
				if (other.gameObject.name.Last() == gameObject.name.Last()){
					if (other.gameObject.name.Last() == '1'){
						battleScript.P1Charging = false;
					} else if (other.gameObject.name.Last() == '2'){
						battleScript.P2Charging = false;
					}

					outerParticles.SetActive(false);
				}
			}
		}
	}

}
