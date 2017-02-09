using System.Collections;
using UnityEngine;

public class PlayerPlayerInteraction : MonoBehaviour {

	//----------Tunable variables----------
	public float bounce = 20.0f; //the strength of the force exerted on each player when they collide


	//----------Internal variables----------
	private const string PLAYER_TAG = "Player";
	private const string COLLISION_PARTICLE = "Spark prefab";

	private void OnCollisionEnter(Collision collision){
		if (collision.gameObject.tag == PLAYER_TAG){
			collision.gameObject.GetComponent<Rigidbody>().AddExplosionForce(bounce,
				collision.contacts[0].point,
				1.0f);
			GetComponent<Rigidbody>().AddExplosionForce(bounce,
				collision.contacts[0].point,
				1.0f);

			GameObject particle = ObjectPooling.ObjectPool.GetObj(COLLISION_PARTICLE);
			particle.transform.position = collision.contacts[0].point;
		}
	}
}
