namespace CannonBoss{
	using UnityEngine;
	using System.Collections;

	public class CannonballBehavior : MonoBehaviour {

		public float speed = 20.0f;

		private Rigidbody rb;

		private const string BOSS_OBJ = "Boss";

		private CannonBossBattle bossBattleScript;
		private const string BOSS_BATTLE_OBJ = "Cannon boss battle(Clone)";

		private void Start(){
			bossBattleScript = GameObject.Find(BOSS_BATTLE_OBJ).GetComponent<CannonBossBattle>();
			rb = GetComponent<Rigidbody>();
			rb.AddForce(new Vector3(0.0f, 0.0f, speed), ForceMode.Impulse); //move straight up
		}

		private void OnTriggerEnter(Collider other){
			if (other.name.Contains(BOSS_OBJ)){
				bossBattleScript.Hit();
				Destroy(gameObject);
			}
		}
	}
}
