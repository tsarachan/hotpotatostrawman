using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QuadPosition{
    public Vector3 nwPoint;
	public Vector3 nePoint;
	public Vector3 swPoint;
	public Vector3 sePoint;
	public Color32 color;

	// constructor
	public QuadPosition(Vector3 nw, Vector3 ne, Vector3 sw, Vector3 se, Color32 col){
		nwPoint = nw;
		nePoint = ne;
		swPoint = sw;
		sePoint = se;
		color = col;
	}
 
 }

//This class draws a rectangular tube with a fixed number of vertices every frame
public class LightTrail : MonoBehaviour {
	
	[Range(1, 100)] public int numFrames = 6;
	private int oldNumFrames;
	public float width = 0.4f;
	public float height = 0.3f;
	private Transform _cam;
	public Color32 color;
	private List<QuadPosition> transformList;
	private Mesh trailMesh;
	private MeshFilter meshFilter;
	private Vector3[] verticesArray;
	private Vector3[] normalsArray;
	private int[] trianglesArray;
	private Color32[] colorsArray;

	void Start () {
		_cam = Camera.main.transform;
		transformList = new List<QuadPosition>();
		trailMesh = new Mesh();
		meshFilter = GetComponent<MeshFilter>();

		//seed transform list
		for (int i=0; i<numFrames; i++) {
			transformList.Add(new QuadPosition(transform.position,transform.position,transform.position,transform.position, color));
		}

		verticesArray = new Vector3[numFrames * 4];
		normalsArray = new Vector3[numFrames * 4];
		colorsArray = new Color32[numFrames*4];
		trianglesArray = new int[((numFrames)*8+4)*3];//2 extra for the cap

		oldNumFrames = numFrames;
	}

	void OnEnable(){
		if (transformList != null) {

			transformList.Clear();

			for (int i=0; i<numFrames; i++) {
				transformList.Add(new QuadPosition(transform.position,transform.position,transform.position,transform.position, color));
			}
		}	
	}
	
	void LateUpdate () {

		transformList = MoveBySpeed();

		//get the camera location of the new vertices
		Vector3 nwVertex = transform.TransformPoint(new Vector3(-0.5f*width, 0.5f*height,0));
		Vector3 neVertex = transform.TransformPoint(new Vector3(0.5f*width, 0.5f*height,0));
		Vector3 swVertex = transform.TransformPoint(new Vector3(-0.5f*width, -0.5f*height,0));
		Vector3 seVertex = transform.TransformPoint(new Vector3(0.5f*width, -0.5f*height,0));

		Vector3 nwTransformed = _cam.InverseTransformPoint(nwVertex);
		Vector3 neTransformed = _cam.InverseTransformPoint(neVertex);
		Vector3 swTransformed = _cam.InverseTransformPoint(swVertex);
		Vector3 seTransformed = _cam.InverseTransformPoint(seVertex);

		transformList.Add(new QuadPosition(nwTransformed,neTransformed,swTransformed,seTransformed, color));


		if (oldNumFrames != numFrames) {
			//changed the number - dump the mesh

			Debug.Log ("changing trail size!");
			while (transformList.Count > numFrames) {
				transformList.RemoveAt(0);
			}
			while (transformList.Count < numFrames) {
				transformList.Add (new QuadPosition(nwTransformed,neTransformed,swTransformed,seTransformed, color));
			}
			verticesArray = new Vector3[numFrames * 4];
			normalsArray = new Vector3[numFrames * 4];
			colorsArray = new Color32[numFrames*4];
			trianglesArray = new int[((numFrames)*8+4)*3];//2 extra for the cap
			trailMesh = new Mesh();

			oldNumFrames = numFrames;
		}

		if (transformList.Count > 0) transformList.RemoveAt(0);


		//construct the mesh
		for (int i=0; i<numFrames; i++){
			verticesArray[i*4 + 0] = transform.InverseTransformPoint(_cam.TransformPoint(transformList[i].nwPoint));
			verticesArray[i*4 + 1] = transform.InverseTransformPoint(_cam.TransformPoint(transformList[i].nePoint));
			verticesArray[i*4 + 2] = transform.InverseTransformPoint(_cam.TransformPoint(transformList[i].swPoint));
			verticesArray[i*4 + 3] = transform.InverseTransformPoint(_cam.TransformPoint(transformList[i].sePoint));
			normalsArray[i*4 + 0] = new Vector3(-1,1,0);
			normalsArray[i*4 + 1] = new Vector3(1,1,0);
			normalsArray[i*4 + 2] = new Vector3(-1,-1,0);
			normalsArray[i*4 + 3] = new Vector3(-1,1,0);
			//byte alpha = (byte)(i*(255/numFrames));
			colorsArray[i*4 + 0] = transformList[i].color;//new Color32(255,255,255,255);
			colorsArray[i*4 + 1] = transformList[i].color;
			colorsArray[i*4 + 2] = transformList[i].color;
			colorsArray[i*4 + 3] = transformList[i].color;
		}

		if (numFrames > 1){
			for (int i=1; i<numFrames; i++){
				trianglesArray[24*i + 0] = 4*(i-1);
				trianglesArray[24*i + 1] = 4*(i);
				trianglesArray[24*i + 2] = 4*(i-1)+2;

				trianglesArray[24*i + 3] = 4*(i);
				trianglesArray[24*i + 4] = 4*(i)+2;
				trianglesArray[24*i + 5] = 4*(i-1)+2;

				trianglesArray[24*i + 6] = 4*(i-1)+1;
				trianglesArray[24*i + 7] = 4*(i)+1;
				trianglesArray[24*i + 8] = 4*(i-1);
			
				trianglesArray[24*i + 9] = 4*(i)+1;
				trianglesArray[24*i + 10] = 4*(i);
				trianglesArray[24*i + 11] = 4*(i-1);

				trianglesArray[24*i + 12] = 4*(i-1)+3;
				trianglesArray[24*i + 13] = 4*(i)+3;
				trianglesArray[24*i + 14] = 4*(i-1)+1;
			
				trianglesArray[24*i + 15] = 4*(i)+3;
				trianglesArray[24*i + 16] = 4*(i)+1;
				trianglesArray[24*i + 17] = 4*(i-1)+1;

				trianglesArray[24*i + 18] = 4*(i-1)+2; //2,6,3
				trianglesArray[24*i + 19] = 4*(i)+2;
				trianglesArray[24*i + 20] = 4*(i-1)+3;
			
				trianglesArray[24*i + 21] = 4*(i)+2; //6,7,3
				trianglesArray[24*i + 22] = 4*(i)+3;
				trianglesArray[24*i + 23] = 4*(i-1)+3;
			}		
		}
		//do caps
		trianglesArray[trianglesArray.Length - 12] = verticesArray.Length-4;
		trianglesArray[trianglesArray.Length - 11] = verticesArray.Length-3;
		trianglesArray[trianglesArray.Length - 10] = verticesArray.Length-2;

		trianglesArray[trianglesArray.Length - 9] = verticesArray.Length-3;
		trianglesArray[trianglesArray.Length - 8] = verticesArray.Length-2;
		trianglesArray[trianglesArray.Length - 7] = verticesArray.Length-1;

		trianglesArray[trianglesArray.Length - 6] = 0;
		trianglesArray[trianglesArray.Length - 5] = 1;
		trianglesArray[trianglesArray.Length - 4] = 2;

		trianglesArray[trianglesArray.Length - 3] = 1;
		trianglesArray[trianglesArray.Length - 2] = 3;
		trianglesArray[trianglesArray.Length - 1] = 2;

		trailMesh.vertices = verticesArray;
		trailMesh.colors32 = colorsArray;
		trailMesh.normals = normalsArray;
		trailMesh.SetTriangles(trianglesArray,0);
		trailMesh.RecalculateBounds();
		meshFilter.sharedMesh = trailMesh;
	}


	private List<QuadPosition> MoveBySpeed(){
		List<QuadPosition> temp = transformList;

		Vector3 speed = new Vector3(0.0f, -0.5f, -0.0f);

		for (int i = 0; i < temp.Count; i++){
			temp[i].nwPoint += speed;
			temp[i].nePoint += speed;
			temp[i].sePoint += speed;
			temp[i].swPoint += speed;
		}


		return temp;
	}
}
