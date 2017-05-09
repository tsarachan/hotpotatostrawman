namespace CannonBoss{
	using UnityEngine;
	using System.Collections;

	public class CannonballBehavior : MonoBehaviour {

		public float speed = 20.0f;
		private Vector3 moveStep = new Vector3(0.0f, 0.0f, 0.0f);

		private Rigidbody rb;

		private const string BOSS_OBJ = "Boss";

		private CannonBossBattle bossBattleScript;
		private const string BOSS_BATTLE_OBJ = "CannonBossBattle(Clone)";


		//particle for when the cannonball hits the boss
		private const string HIT_PARTICLE = "Boss hit particle";

		private void Start(){
			moveStep.z = speed;
			bossBattleScript = GameObject.Find(BOSS_BATTLE_OBJ).GetComponent<CannonBossBattle>();
			rb = GetComponent<Rigidbody>();
		}


		private void Update(){
			rb.MovePosition(transform.position + moveStep);
		}


		private void OnTriggerEnter(Collider other){
			if (other.gameObject.name.Contains(BOSS_OBJ)){
				bossBattleScript.Hit();

				GameObject particle = ObjectPooling.ObjectPool.GetObj(HIT_PARTICLE);
				particle.transform.position = transform.position;

				Destroy(gameObject);
			}
		}
	}
}
