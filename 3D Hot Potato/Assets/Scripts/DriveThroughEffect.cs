using UnityEngine;

public class DriveThroughEffect : MonoBehaviour {

	private const string PLAYER_TAG = "Player";
	private const string DRIVE_THRU_PARTICLE = "Go through explosion particle";


	private void OnTriggerEnter(Collider other){
		if (other.tag == PLAYER_TAG){
			ParticleSystem driveThruParticle = 
				other.transform.Find(DRIVE_THRU_PARTICLE).GetComponent<ParticleSystem>();

			driveThruParticle.Stop();
			driveThruParticle.Play();
		}
	}
}
