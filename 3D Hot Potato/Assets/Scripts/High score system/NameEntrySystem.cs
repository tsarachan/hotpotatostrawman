﻿using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TMPro;

public class NameEntrySystem : MonoBehaviour {

	//the characters that can be included in a player's initials
	private List<char> characters = new List<char> { 'a',
		'b',
		'c',
		'd',
		'e',
		'f',
		'g',
		'h',
		'i',
		'j',
		'k',
		'l',
		'm',
		'n',
		'o',
		'p',
		'q',
		'r',
		's',
		't',
		'u',
		'v',
		'w',
		'x',
		'y',
		'z',
		' ' };


	//UI for the players' initials
	private TextMeshProUGUI p1Initials;
	private const string P1_INITIALS_OBJ = "P1 initials";
	private TextMeshProUGUI p2Initials;
	private const string P2_INITIALS_OBJ = "P2 initials";
	private const string CANVAS_OBJ = "Canvas";
	private const string NAME_ENTRY_ORGANIZER = "Name entry UI";


	//used to determine which character each player is entering for each initial
	private int currentP1Index = 0;
	private int currentP1Character = 0;
	private int currentP2Index = 0;
	private int currentP2Character = 0;


	//used to keep track of where the players are in the name-entry process
	private bool p1DoneEntering = false;
	private bool p2DoneEntering = false;
	private bool bothDoneEntering = false;


	//initialize variables
	private void Start(){
		p1Initials = transform.Find(CANVAS_OBJ).Find(NAME_ENTRY_ORGANIZER)
			.Find(P1_INITIALS_OBJ).GetComponent<TextMeshProUGUI>();
		p1Initials.text = "   ";

		p2Initials = transform.Find(CANVAS_OBJ).Find(NAME_ENTRY_ORGANIZER)
			.Find(P2_INITIALS_OBJ).GetComponent<TextMeshProUGUI>();
		p2Initials.text = "   ";
	}


	/// <summary>
	/// This is where all the work is done.
	/// 
	/// Each frame, the player has the opportunity to change the character they have selected, and to lock in
	/// the current character for the current initial.
	/// 
	/// When a player has locked in three initials, that player is done.
	/// 
	/// When both players are done, this triggers the high score maanger, and then falls out of the update loop
	/// thereafter.
	/// </summary>
	private void Update(){
		if (!bothDoneEntering){
			if (!p1DoneEntering){
				if (currentP1Index >= p1Initials.text.Length){
					p1DoneEntering = true;
				} else {
					currentP1Character = SelectCharacter(KeyCode.A, KeyCode.D, currentP1Character);
					currentP1Index = EnterButton(KeyCode.S, currentP1Index);

					if (currentP1Index < p1Initials.text.Length){
						p1Initials.text = ReplaceCharInString(p1Initials.text, currentP1Index, characters[currentP1Character]);
					}
				}
			}

			if (!p2DoneEntering){
				if (currentP2Index >= p2Initials.text.Length){
					p2DoneEntering = true;
				} else {
					currentP2Character = SelectCharacter(KeyCode.J, KeyCode.L, currentP2Character);
					currentP2Index = EnterButton(KeyCode.K, currentP2Index);

					if (currentP2Index < p2Initials.text.Length){
						p2Initials.text = ReplaceCharInString(p2Initials.text, currentP2Index, characters[currentP2Character]);
					}
				}
			}

			if (p1DoneEntering && p2DoneEntering){
				GetComponent<HighScoreManager>().ReviseScoreList(p1Initials.text + " & " + p2Initials.text,
																 ScoreRepository.Score);
				StartCoroutine(GetComponent<HighScoreManager>().DisplayScore());
				bothDoneEntering = true;
			}
		}
	}


	/// <summary>
	/// Determines what character the player has currently selected
	/// </summary>
	/// <returns>The character.</returns>
	/// <param name="downButton">Down button.</param>
	/// <param name="upButton">Up button.</param>
	/// <param name="currentCharacter">The index in characters of the player's current selection.</param>
	private int SelectCharacter(KeyCode downButton, KeyCode upButton, int currentCharacter){
		int temp = currentCharacter;

		if (Input.GetKeyDown(downButton)){
			temp--;

			if (temp < 0){
				temp = characters.Count - 1;
			}
		} else if (Input.GetKeyDown(upButton)){
			temp++;

			if (temp >= characters.Count){
				temp = 0;
			}
		}

		return temp;
	}


	/// <summary>
	/// Advances the player to the next initial upon input.
	/// </summary>
	/// <returns>The button.</returns>
	/// <param name="button">Button.</param>
	/// <param name="currentIndex">The index of the initial the player is currently entering, 0, 1, or 2.</param>
	private int EnterButton(KeyCode button, int currentIndex){
		int temp = currentIndex;

		if (Input.GetKeyDown(button)){
			temp++;
		}

		return temp;
	}


	/// <summary>
	/// Utility function for replacing an individual character in a string.
	/// </summary>
	/// <returns>A new string with the replacement completed.</returns>
	/// <param name="startString">The original string.</param>
	/// <param name="index">The index where the replacement is to occur.</param>
	/// <param name="newChar">The character to replace with.</param>
	private string ReplaceCharInString(string startString, int index, char newChar){
		StringBuilder temp = new StringBuilder(startString);
		temp[index] = newChar;
		return temp.ToString();
	}
}
