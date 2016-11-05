using UnityEngine;
using System.Collections;

public abstract class EnemyBase : MonoBehaviour {

	protected const string PLAYER_OBJ = "Player";

	public virtual void GetDestroyed(){
		Destroy(gameObject);
	}
}
