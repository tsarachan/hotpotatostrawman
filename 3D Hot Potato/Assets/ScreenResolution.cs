using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenResolution : MonoBehaviour {

	// Use this for initialization
	void Start () {

		//Screen.SetResolution (400, 400, true, 60);
		
	}
	
	// Update is called once per frame
	void Update () {

		Screen.SetResolution (400, 400, true, 60);

	}
}
