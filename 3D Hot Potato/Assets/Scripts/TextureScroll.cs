using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureScroll : MonoBehaviour {


	private float speed = 0.85f;


	private float offset = 0.0f;
	private Renderer rend;
	private Material myMaterial;


	private void Start(){
		rend = GetComponent<Renderer>();
		myMaterial = rend.material;
	}


	private void Update(){
		offset += Time.deltaTime * speed;

		if (offset > 1.0f){
			offset = 0.0f;
		}

		myMaterial.SetTextureOffset("_MainTex", new Vector2(0.0f, offset));
	}
}
