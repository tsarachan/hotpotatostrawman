/*
 * 
 * This script works in concert with "Poolable" to put enemies into, and bring them out of, the object pool.
 * 
 * All enemies must inherit from Poolable for this script to work correctly.
 * 
 * Note that this doesn't do anything to the enemy other than put it in the hierarchy. The enemy's own script is responsible for
 * its own parenting, movement after appearing in the scene, etc.
 * 
 */

namespace ObjectPooling
{
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;

	public class ObjectPool {

		public static Dictionary<string, Queue<GameObject>> enemyPool = new Dictionary<string, Queue<GameObject>>();


		/// <summary>
		/// Call this to make an enemy.
		/// 
		/// The function call is responsible for making sure the parameter is correct. This will warn you if you
		/// try to make an enemy with a name that doesn't exist, but it can't fix the problem.
		/// </summary>
		/// <returns>The chosen enemy.</returns>
		/// <param name="enemyType">The name of the enemy prefab.</param>
		public static GameObject GetObj(string enemyType){
			GameObject obj = null; //default initialization for error-checking

			//if an enemy of the chosen type is in the pool, get one of them
			if (enemyPool.ContainsKey(enemyType)){
				if (enemyPool[enemyType].Count > 0){
					obj = enemyPool[enemyType].Dequeue();
					obj.GetComponent<Poolable>().Reset();
				}
			}

			//if no enemy of a given type is in the pool, make one
			else {
				obj = MonoBehaviour.Instantiate(Resources.Load(enemyType)) as GameObject;
			}

			if (obj == null) { Debug.Log("Unable to find enemy named " + enemyType); }

			return obj;
		}


		/// <summary>
		/// Call this to add an enemy to the pool of enemies.
		/// </summary>
		/// <param name="enemyType">The name of the enemy prefab.</param>
		public static void AddObj(string enemyType){
			GameObject obj = MonoBehaviour.Instantiate(Resources.Load(enemyType)) as GameObject;

			if (!enemyPool.ContainsKey(enemyType)){
				enemyPool.Add(enemyType, new Queue<GameObject>());
			}

			enemyPool[enemyType].Enqueue(obj);
			obj.GetComponent<Poolable>().ShutOff();
		}
	}
}
