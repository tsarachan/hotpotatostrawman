using UnityEngine;
using System.Collections;

public class CannonballBehavior : MonoBehaviour {

	public float speed = 20.0f;

	private Rigidbody rb;

	private const string BOSS_OBJ = "Boss";

	private void Start(){
		rb = GetComponent<Rigidbody>();
		rb.AddForce(new Vector3(0.0f, 0.0f, speed), ForceMode.Impulse); //move straight up
	}

	private void OnTriggerEnter(Collider other){
		if (other.name.Contains(BOSS_OBJ)){
			other.GetComponent<EnemyBoss>().GetHit();
			Destroy(gameObject);
		}
	}
}
