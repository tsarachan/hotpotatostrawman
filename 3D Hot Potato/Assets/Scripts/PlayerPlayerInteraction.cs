using System.Collections;
using UnityEngine;

public class PlayerPlayerInteraction : MonoBehaviour {

	private const string PLAYER_TAG = "Player";

	public float bounce = 20.0f;

	private void OnCollisionEnter(Collision collision){
		if (collision.gameObject.tag == PLAYER_TAG){
			collision.gameObject.GetComponent<Rigidbody>().AddExplosionForce(bounce,
				collision.contacts[0].point,
				1.0f);
			GetComponent<Rigidbody>().AddExplosionForce(bounce,
				collision.contacts[0].point,
				1.0f);
		}
	}
}
