using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class LoadPolygonObstacles : MonoBehaviour {

	public string saveAs;

	private int[,] edges;
	private double[,] vertices;

//	private float tUnit = 0.25f;
	private Vector2 tStone = new Vector2 (0, 0);
	private Vector2 tGrass = new Vector2 (0, 1);

	// This first list contains every vertex of the mesh that we are going to render
	public List<Vector3> newVertices = new List<Vector3>();
	
	// The triangles tell Unity how to build each section of the mesh joining
	// the vertices
	public List<int> newTriangles = new List<int>();
	
	// The UV list is unimportant right now but it tells Unity how the texture is
	// aligned on each polygon
	public List<Vector2> newUV = new List<Vector2>();
	
	
	// A mesh is made up of the vertices, triangles and UVs we are going to define,
	// after we make them up we'll save them as this mesh
	private Mesh mesh;

	void Start () {
		edges = new int[,] {{1, 2},
			{2, 3},
			{3, 4},
			{4, 1},
			{5, 6},
			{6, 7},
			{7, 8},
			{8, 9},
			{9, 10},
			{10, 5},
			{11, 12},
			{12, 13},
			{13, 14},
			{14, 11},
			{15, 16},
			{16, 17},
			{17, 18},
			{18, 15},
			{19, 20},
			{20, 21},
			{21, 22},
			{22, 23},
			{23, 19}};

		vertices = new double[,] {{8.8710, 84.6491},
				{8.6406, 62.1345},
				{21.0829, 62.4269},
				{32.3733, 75.5848},
				{58.8710, 76.4620},
				{77.5346, 66.5205},
				{53.5714, 42.8363},
				{13.9401, 40.4971},
				{14.8618, 47.8070},
				{56.1060, 57.7485},
				{72.9263, 50.4386},
				{94.5853, 51.0234},
				{93.4332, 38.1579},
				{69.0092, 37.8655},
				{75.4608, 25.5848},
				{75.0000, 13.5965},
				{37.6728, 19.4444},
				{35.5991, 29.6784},
				{19.4700, 35.2339},
				{28.9171, 34.0643},
				{29.3779, 13.0117},
				{17.1659, 13.0117},
				{12.0968, 24.4152}};

			
			mesh = GetComponent<MeshFilter> ().mesh;
			
			float x = transform.position.x;
			float y = transform.position.y;
			float z = transform.position.z;
			
		for(int i = 0; i < vertices.GetLength(0); i++) {
			newVertices.Add(new Vector3((float) vertices[i,0], 0, (float) vertices[i,1]));
			newVertices.Add(new Vector3((float) vertices[i,0], 1, (float) vertices[i,1]));
		}

			
			newTriangles.Add(0 + 1);
			newTriangles.Add(2*3 + 1);
			newTriangles.Add(2*2 + 1);
			newTriangles.Add(0 + 1);
			newTriangles.Add(2*2 + 1);
			newTriangles.Add(2*1 + 1);

		newTriangles.Add(2*4 + 1);
		newTriangles.Add(2*5 + 1);
		newTriangles.Add(2*9 + 1);
		newTriangles.Add(2*5 + 1);
		newTriangles.Add(2*6 + 1);
		newTriangles.Add(2*9 + 1);
		newTriangles.Add(2*6 + 1);
		newTriangles.Add(2*7 + 1);
		newTriangles.Add(2*8 + 1);
		newTriangles.Add(2*6 + 1);
		newTriangles.Add(2*8 + 1);
		newTriangles.Add(2*9 + 1);

		newTriangles.Add(2*10 + 1);
		newTriangles.Add(2*11 + 1);
		newTriangles.Add(2*12 + 1);
		newTriangles.Add(2*12 + 1);
		newTriangles.Add(2*13 + 1);
		newTriangles.Add(2*10 + 1);

		newTriangles.Add(2*14 + 1);
		newTriangles.Add(2*15 + 1);
		newTriangles.Add(2*16 + 1);
		newTriangles.Add(2*16 + 1);
		newTriangles.Add(2*17 + 1);
		newTriangles.Add(2*14 + 1);

		newTriangles.Add(2*18 + 1);
		newTriangles.Add(2*19 + 1);
		newTriangles.Add(2*20 + 1);
		newTriangles.Add(2*20 + 1);
		newTriangles.Add(2*21 + 1);
		newTriangles.Add(2*18 + 1);
		newTriangles.Add(2*21 + 1);
		newTriangles.Add(2*22 + 1);
		newTriangles.Add(2*18 + 1); 

		for (int i = 0; i < edges.GetLength(0); i++) {
			newTriangles.Add ((edges[i,0] - 1)*2);
			newTriangles.Add ((edges[i,0] - 1)*2 + 1);
			newTriangles.Add ((edges[i,1] - 1)*2);
			newTriangles.Add ((edges[i,0] - 1)*2 + 1);
			newTriangles.Add ((edges[i,1] - 1)*2 + 1);
			newTriangles.Add ((edges[i,1] - 1)*2);

			newTriangles.Add ((edges[i,0] - 1)*2);
			newTriangles.Add ((edges[i,1] - 1)*2);
			newTriangles.Add ((edges[i,0] - 1)*2 + 1);
			newTriangles.Add ((edges[i,0] - 1)*2 + 1);
			newTriangles.Add ((edges[i,1] - 1)*2);
			newTriangles.Add ((edges[i,1] - 1)*2 + 1);




		}

			



			mesh.Clear ();
			mesh.vertices = newVertices.ToArray();
			mesh.triangles = newTriangles.ToArray();

		transform.GetComponent<MeshCollider>().sharedMesh = mesh;
	//		mesh.uv = newUV.ToArray(); // add this line to the code here
			mesh.Optimize ();
			mesh.RecalculateNormals ();

		Mesh m1 = transform.GetComponent<MeshFilter>().mesh;
//		AssetDatabase.CreateAsset(m1, "Assets/Meshes/" + saveAs + ".asset");

		}

}
