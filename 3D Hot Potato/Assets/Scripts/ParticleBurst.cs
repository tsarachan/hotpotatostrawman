/*
 * 
 * Use this script to create simple bursts of cubes.
 * 
 */

using UnityEngine;
using System.Collections;

public class ParticleBurst : MonoBehaviour {

	private const string TINY_CUBE = "Tiny cube";
	public int numCubes = 4; //number of cubes to make when destroyed
	public float forceMax = 2.0f; //how hard to push the cubes as they spawn


	public void MakeBurst(){
		for (int i = 0; i < numCubes; i++){
			GameObject newCube = ObjectPooling.ObjectPool.GetObj(TINY_CUBE);

			newCube.GetComponent<Rigidbody>().AddForce(Random.Range(-forceMax, forceMax),
													   Random.Range(-forceMax, forceMax),
													   forceMax,
													   ForceMode.Impulse);
			newCube.GetComponent<Rigidbody>().AddTorque(Random.Range(-forceMax, forceMax),
														Random.Range(-forceMax, forceMax),
														Random.Range(-forceMax, forceMax),
														ForceMode.Impulse);
			newCube.GetComponent<Renderer>().material = GetComponent<Renderer>().material;
		}
	}
}
