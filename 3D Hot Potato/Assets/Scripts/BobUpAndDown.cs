using UnityEngine;
using System.Collections;

public class BobUpAndDown : MonoBehaviour {

	public float speed = 1.0f;
	public float range = 1.0f;

	Transform tf;
	Vector3 posVec;
	float startY;

	void Awake()
	{
		tf = transform;
		posVec = tf.localPosition;
		startY = posVec.y;
	}

	void Update ()
	{
		posVec.y = startY + range * Mathf.Sin(Time.time * speed); //using Time.time means all bobbing things will
																  //be at the same place in their bob
		tf.localPosition = posVec;
	}
}
