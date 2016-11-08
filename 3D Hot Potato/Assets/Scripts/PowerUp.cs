using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PowerUp : MonoBehaviour {

	private Image superMeter;
	private const string SUPER_METER = "Super Meter";

	public float superGainIncrement = 0.1f; //how much super meter do the players get per pass? 0.0f - 1.0f
	private float maxSuperMeter = 1.0f;

	//power-ups

	/*
	 * 
	 * Power-ups are spawned using two dictionaries: one of the powerups, and one of the types of throws players make
	 * (horizontal, vertical, etc.). The powerup dictionary has all the powerups, indexed by ints. The throw type
	 * tracks the number of throws of each type the players make, indexed by the type of throw.
	 * 
	 * The throw types are mapped to ints using an enum. This makes the code shorter, and hopefully more readable.
	 * 
	 * Each time the players throw, the super meter increases and an entry is made in the throw type dictionary under the appropriate type.
	 * 
	 * When the super meter is full, the system checks which throw type has the most entries. It spawns a powerup from the dictionary
	 * of powerups accordingly, and then resets the process.
	 * 
	 */

	private Dictionary<int, GameObject> powerUps = new Dictionary<int, GameObject>();
	enum ThrowTypes { horizontal, vertical }; //horizontal == 0, vertical == 1

	private GameObject clearEnemies;
	private const string CLEAR_ENEMIES_POWERUP = "ClearEnemies";

	private Dictionary<int, int> relativePositions = new Dictionary<int, int>(); //keeps track of where the players have been when they power up

	private void Start(){
		superMeter = GetComponent<Image>();
		superMeter.fillAmount = 0.0f;

		powerUps = SetUpPowerUps();
		relativePositions = SetUpPositionPossibilities();
	}

	/// <summary>
	/// Create a dictionary with all the different ways to power up, and the powerups players get as a result
	/// </summary>
	/// <returns>The dictionary.</returns>
	private Dictionary<int, GameObject> SetUpPowerUps(){
		Dictionary<int, GameObject> temp = new Dictionary<int, GameObject>();

		clearEnemies = Resources.Load(CLEAR_ENEMIES_POWERUP) as GameObject;

		temp.Add((int)ThrowTypes.vertical, clearEnemies);
		temp.Add((int)ThrowTypes.horizontal, clearEnemies);

		return temp;
	}


	/// <summary>
	/// Create a dictionary to keep track of what kinds of throws players make--primarily horizontal ones, primarily vertical ones, etc.
	/// </summary>
	/// <returns>The dictionary.</returns>
	private Dictionary<int, int> SetUpPositionPossibilities(){
		Dictionary<int, int> temp = new Dictionary<int, int>();

		temp.Add((int)ThrowTypes.vertical, 0);
		temp.Add((int)ThrowTypes.horizontal, 0);

		return temp;
	}


	/// <summary>
	/// Call this script to increase the super meter.
	/// </summary>
	/// <param name="player1">Player 1.</param>
	/// <param name="player2">Player 2.</param>
	public void IncreaseSuperMeter(Transform player1, Transform player2){
		superMeter.fillAmount += superGainIncrement;

		//track what type of throw the player made--horizontal, vertical, etc.
		Vector3 difference = player1.position - player2.position;
		float xDist = Mathf.Abs(difference.x);
		float zDist = Mathf.Abs(difference.z);

		if (xDist > zDist){
			relativePositions[(int)ThrowTypes.horizontal]++;
		} else if (zDist > xDist){
			relativePositions[(int)ThrowTypes.vertical]++;
		} else { //tiebreaker
			relativePositions[(int)ThrowTypes.horizontal]++;
		}

		//if the players have filled the meter, trigger a powerup
		if (superMeter.fillAmount >= maxSuperMeter){
			GetPowerUp();
		}
	}


	/// <summary>
	/// Activate the power-up with the most entries in the super meter, then reset the super meter.
	/// 
	/// The power-up is responsible for taking care of itself--what its parent should be, should it destroy itself, etc.
	/// </summary>
	private void GetPowerUp(){

		//chose the power-up to spawn
		if (relativePositions[(int)ThrowTypes.vertical] > relativePositions[(int)ThrowTypes.horizontal]){
			Instantiate(powerUps[(int)ThrowTypes.vertical]);
		} else if (relativePositions[(int)ThrowTypes.horizontal] > relativePositions[(int)ThrowTypes.vertical]){
			Instantiate(powerUps[(int)ThrowTypes.horizontal]);
		} else { //tiebreaker
			Instantiate(powerUps[(int)ThrowTypes.horizontal]);
		}

		//reset the super meter
		relativePositions[(int)ThrowTypes.vertical] = 0;
		relativePositions[(int)ThrowTypes.horizontal] = 0;
		superMeter.fillAmount = 0.0f;
	}
}
