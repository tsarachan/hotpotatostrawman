namespace CannonBoss{
	using UnityEngine;

	public class CannonBossBehavior : MonoBehaviour {


		private CannonBossBattle bossBattleScript;

		private const string CANNONBALL_OBJ = "Cannonball";


		private void Start(){
			bossBattleScript = transform.parent.GetComponent<CannonBossBattle>();
		}


		private void OnTriggerEnter(Collider other){
			if (other.gameObject.name.Contains(CANNONBALL_OBJ)){
				bossBattleScript.Hit();
			}
		}
	}
}
