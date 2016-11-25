/*
 * 
 * This script works in concert with "Poolable" to put things (e.g., enemies) into, and bring them out of, the object pool.
 * 
 * To work correctly, all objects to be in a pool must:
 * 1. inherit from Poolable, and
 * 2. be instantiated from a prefab.
 * 
 * Note that this doesn't do anything to the object other than put it in the hierarchy. The object's own script is responsible for
 * its parenting, movement after appearing in the scene, etc.
 * 
 */

namespace ObjectPooling
{
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;

	public class ObjectPool {

		private const string CLONE = "(Clone)";

		public static Dictionary<string, Queue<GameObject>> objectPool = new Dictionary<string, Queue<GameObject>>();


		/// <summary>
		/// Call this to bring an object into the playing area.
		/// 
		/// The function call is responsible for making sure the parameter is correct. This will warn you if you
		/// try to make an enemy with a name that doesn't exist, but it can't fix the problem.
		/// </summary>
		/// <returns>The chosen object.</returns>
		/// <param name="enemyType">The name of the object prefab.</param>
		public static GameObject GetObj(string objectType){
			GameObject obj = null; //default initialization for error-checking
//			Debug.Log("trying to get " + objectType);

			//if an object of the chosen type is in the pool, get one of them
			if (objectPool.ContainsKey(objectType + CLONE)){
				if (objectPool[objectType + CLONE].Count > 0){
					obj = objectPool[objectType + CLONE].Dequeue();
					obj.GetComponent<Poolable>().Reset();
//					Debug.Log("Found one in the pool");
				}
			}

			//if no object of a given type is in the pool, make one
			else {
				obj = MonoBehaviour.Instantiate(Resources.Load(objectType)) as GameObject;
//				Debug.Log("Made something because it wasn't in the pool");
			}

			if (obj == null) { Debug.Log("Unable to find enemy named " + objectType); }

			return obj;
		}


		/// <summary>
		/// Call this to add an object to the pool of objects.
		/// </summary>
		/// <param name="enemyType">The name of the object prefab.</param>
		public static void AddObj(GameObject obj){
			if (!objectPool.ContainsKey(obj.name)){
				objectPool.Add(obj.name, new Queue<GameObject>());
			}

			objectPool[obj.name].Enqueue(obj);
			obj.GetComponent<Poolable>().ShutOff();
 		}
	}
}
