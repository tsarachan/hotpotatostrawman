/*
 * 
 * Test script for object pooling. This shouldn't be in the actual game.
 * 
 */

namespace ObjectPooling
{
	using UnityEngine;
	using System.Collections;

	public class PoolingTest : MonoBehaviour {

		private void Start(){
			for (int i = 0; i < 100; i++){
				GameObject newObj = Instantiate(Resources.Load("Tiny cube"), Vector3.zero, Quaternion.identity) as GameObject;
				ObjectPool.AddObj(newObj);
			}
		}

		private void Update(){
			if (Input.GetKeyDown(KeyCode.Space)){
				ObjectPool.GetObj("Tiny cube");
			}
		}
	}
}
