using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class GenPolygObst : MonoBehaviour {

	public int numPolygons;
	public string saveAs;

	public string mapFileName;

	private Mesh mesh;
	private List<int> newTriangles = new List<int>();
	private List<Vector3> newVertices = new List<Vector3>();
	private List<List<Vector3>> map;
	private List<int[]> edges;

	// Use this for initialization
	void Start () {
		LoadMapFromFile ();
		GenerateObstacles ();

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private void GenerateObstacles() {
		mesh = GetComponent<MeshFilter> ().mesh;
		
		float x = transform.position.x;
		float y = transform.position.y;
		float z = transform.position.z;
		
		foreach(List<Vector3> polyg in map) {
			foreach(Vector3 vertice in polyg) {
				newVertices.Add(new Vector3((float) vertice.x, 0, (float) vertice.z));
				newVertices.Add(new Vector3((float) vertice.x, 1, (float) vertice.z));
			}
		}
		
		/*newTriangles.Add(0 + 1);
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
		newTriangles.Add(2*18 + 1); */
		
		for (int i = 0; i < edges.Count; i++) {
			newTriangles.Add ((edges[i][0] - 1)*2);
			newTriangles.Add ((edges[i][0] - 1)*2 + 1);
			newTriangles.Add ((edges[i][1] - 1)*2);
			newTriangles.Add ((edges[i][0] - 1)*2 + 1);
			newTriangles.Add ((edges[i][1] - 1)*2 + 1);
			newTriangles.Add ((edges[i][1] - 1)*2);
			
			newTriangles.Add ((edges[i][0] - 1)*2);
			newTriangles.Add ((edges[i][1] - 1)*2);
			newTriangles.Add ((edges[i][0] - 1)*2 + 1);
			newTriangles.Add ((edges[i][0] - 1)*2 + 1);
			newTriangles.Add ((edges[i][1] - 1)*2);
			newTriangles.Add ((edges[i][1] - 1)*2 + 1);
		}
		
		
		
		
		
		mesh.Clear ();
		mesh.vertices = newVertices.ToArray();
		mesh.triangles = newTriangles.ToArray();
		
		transform.GetComponent<MeshCollider>().sharedMesh = mesh;
		//		mesh.uv = newUV.ToArray(); // add this line to the code here
		mesh.Optimize ();
		mesh.RecalculateNormals ();
		
		Mesh m1 = transform.GetComponent<MeshFilter>().mesh;
		AssetDatabase.CreateAsset(m1, "Assets/Meshes/" + saveAs + ".asset");
	}

	private void LoadMapFromFile() {
		numPolygons = 0;
		map = new List<List<Vector3>> ();
		edges = new List<int[]> ();
		Debug.Log ("Number of polygons " + numPolygons);
		map.Add(new List<Vector3> ());
		string line;
		
		StreamReader streamReader = new StreamReader(mapFileName, Encoding.Default);
		
		using (streamReader) {
			do {
				line = streamReader.ReadLine();
				if (line != null) {
					string[] entries = line.Split(',');
					if (entries.Length > 0) {
						float x = float.Parse(entries[0]);
						float z = float.Parse(entries[1]);
						int b = int.Parse(entries[2]);
						int e1 = int.Parse(entries[3]);
						int e2 = int.Parse(entries[4]);
						map[numPolygons].Add(new Vector3(x, 0.0f, z));
						edges.Add (new int[] {e1, e2});
						if(b == 3) {
							numPolygons++;
							map.Add(new List<Vector3>());
						}
					}
				}
			} while (line != null);
			
			map.RemoveAt(numPolygons);
			streamReader.Close();
		}
	}
}
