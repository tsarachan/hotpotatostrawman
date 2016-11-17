using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ForceSpawn : MonoBehaviour {

	private List<Transform> spawners = new List<Transform>();

	private void Start(){
		spawners = FindSpawners();
	}

	private List<Transform> FindSpawners(){
		List<Transform> temp = new List<Transform>();

		foreach (Transform spawner in transform){
			temp.Add(spawner);
		}

		if (temp.Count == 0) { Debug.Log("Couldn't find any spawners!"); }

		return temp;
	}

	private void Update(){

	}
}
