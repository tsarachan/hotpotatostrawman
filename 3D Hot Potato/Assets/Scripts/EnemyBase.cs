using UnityEngine;
using System.Collections;

public abstract class EnemyBase : MonoBehaviour {



	public virtual void GetDestroyed(){
		Destroy(gameObject);
	}
}
