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
	 * Power-ups are spawned using two dictionaries: one of the powerups, and one of the relative positions players were in for the throw.
	 * 
	 * Each time the players throw, the super meter increases and the appropriate entry in the relative positions dictionary increases.
	 * 
	 * When the super meter is full, the system checks which relative position has the highest total. It spawns a powerup from the dictionary
	 * of powerups accordingly, and then resets the process.
	 * 
	 */

	private Dictionary<ThrowTypes, GameObject> powerUps = new Dictionary<ThrowTypes, GameObject>();
	enum ThrowTypes { horizontal, vertical }; //horizontal == 0, vertical == 1

	private GameObject clearEnemies;
	private const string CLEAR_ENEMIES_POWERUP = "ClearEnemies";

	private Dictionary<ThrowTypes, int> relativePositions = new Dictionary<ThrowTypes, int>(); //keeps track of where the players have been when they power up

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
	private Dictionary<ThrowTypes, GameObject> SetUpPowerUps(){
		Dictionary<ThrowTypes, GameObject> temp = new Dictionary<ThrowTypes, GameObject>();

		clearEnemies = Resources.Load(CLEAR_ENEMIES_POWERUP) as GameObject;

		temp.Add(ThrowTypes.vertical, clearEnemies);
		temp.Add(ThrowTypes.horizontal, clearEnemies);

		return temp;
	}


	/// <summary>
	/// Create a dictionary to keep track of what kinds of throws players make--primarily horizontal ones, primarily vertical ones, etc.
	/// </summary>
	/// <returns>The dictionary.</returns>
	private Dictionary<ThrowTypes, int> SetUpPositionPossibilities(){
		Dictionary<ThrowTypes, int> temp = new Dictionary<ThrowTypes, int>();

		temp.Add(ThrowTypes.vertical, 0);
		temp.Add(ThrowTypes.horizontal, 0);

		return temp;
	}


	/// <summary>
	/// Call this script to increase the super meter.
	/// </summary>
	/// <param name="player1">Where the throw began.</param>
	/// <param name="player2">The location of the intended receiver.</param>
	public void IncreaseSuperMeter(Vector3 start, Transform end){
		superMeter.fillAmount += superGainIncrement;

		//track what type of throw the player made--horizontal, vertical, etc.
		Vector3 difference = start - end.position;
		float xDist = Mathf.Abs(difference.x);
		float zDist = Mathf.Abs(difference.z);

		if (xDist > zDist){
			relativePositions[ThrowTypes.horizontal]++;
		} else if (zDist > xDist){
			relativePositions[ThrowTypes.vertical]++;
		} else { //tiebreaker
			relativePositions[ThrowTypes.horizontal]++;
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
		if (relativePositions[ThrowTypes.vertical] > relativePositions[ThrowTypes.horizontal]){
			Instantiate(powerUps[ThrowTypes.vertical]);
		} else if (relativePositions[ThrowTypes.horizontal] > relativePositions[ThrowTypes.vertical]){
			Instantiate(powerUps[ThrowTypes.horizontal]);
		} else { //tiebreaker
			Instantiate(powerUps[ThrowTypes.horizontal]);
		}

		//reset the super meter
		relativePositions[ThrowTypes.vertical] = 0;
		relativePositions[ThrowTypes.horizontal] = 0;
		superMeter.fillAmount = 0.0f;
	}
}
