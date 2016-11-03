namespace ObjectPooling
{
	using UnityEngine;
	using System.Collections;

	public class PoolingTest : MonoBehaviour {

		private void Start(){
			for (int i = 0; i < 100; i++){
				ObjectPool.AddObj("HomingEnemy");
				Debug.Log("Added an enemy");
			}
		}

		private void Update(){
			if (Input.GetKeyDown(KeyCode.Space)){
				ObjectPool.GetObj("HomingEnemy");
			}
		}
	}
}
