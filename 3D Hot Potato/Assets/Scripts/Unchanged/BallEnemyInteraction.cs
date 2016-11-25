﻿using UnityEngine;
using System.Collections;

public class BallEnemyInteraction : MonoBehaviour {

	private const string ENEMY_OBJ = "Enemy";

	private void OnTriggerEnter(Collider other){
		if (other.gameObject.name.Contains(ENEMY_OBJ)){
			other.gameObject.GetComponent<EnemyBase>().GetDestroyed();
		}
	}
}
