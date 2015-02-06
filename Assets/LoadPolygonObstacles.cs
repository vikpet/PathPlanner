using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoadPolygonObstacles : MonoBehaviour {

	private int[,] edges;

	private float tUnit = 0.25f;
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

		for (int i = 0; i < edges.Length; i++) {
			GameObject polygon = new GameObject();
			polygon.name = "Polygon Obstacle";
			//Transform clone = (Transform) GameObject.Instantiate(boxPrefab, new Vector3(xPos, yPos, zPos), Quaternion.identity);
			polygon.transform.parent = this.transform;

//			MeshFilter mf = polygon.AddComponent("MeshFilter") as MeshFilter;
//			mf.mesh = CreateMesh(new Vector3[] {new Vector3(1, 0, 1), new Vector3(2, 0, 2), new Vector3(1, 0, 2)});
		}
		/*
		 * for(i=1:length(x))
    
    edges(i,1)=i;
    if (button(i)==1)
        edges(i,2)=i+1;
    elseif (button(i)==3)
        edges(i,2)=firstEdge;
        firstEdge=i+1
    end
    plot([x(edges(i,1)),x(edges(i,2))],[y(edges(i,1)),y(edges(i,2))]);
    hold on
    drawnow
end
*/
		// Use this for initialization
			
			mesh = GetComponent<MeshFilter> ().mesh;
			
			float x = transform.position.x;
			float y = transform.position.y;
			float z = transform.position.z;
			
			
			newVertices.Add( new Vector3 (x  , y  , z ));
			newVertices.Add( new Vector3 (x + 1 , y  , z ));
			newVertices.Add( new Vector3 (x + 1 , y-1  , z ));
			newVertices.Add( new Vector3 (x  , y-1  , z ));
			
			newTriangles.Add(0);
			newTriangles.Add(1);
			newTriangles.Add(3);
			newTriangles.Add(1);
			newTriangles.Add(2);
			newTriangles.Add(3);
			
			mesh.Clear ();
			mesh.vertices = newVertices.ToArray();
			mesh.triangles = newTriangles.ToArray();
			mesh.uv = newUV.ToArray(); // add this line to the code here
			mesh.Optimize ();
			mesh.RecalculateNormals ();


		}

}
