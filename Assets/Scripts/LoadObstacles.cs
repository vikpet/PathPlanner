using UnityEngine;
using System.Collections;

public class LoadObstacles : MonoBehaviour {

	public int[][] map= new int[20][] { 
		new int[20]{ 1, 0, 1, 0, 1, 1, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0 }, 
		new int[20]{ 1, 0, 0, 1, 1, 1, 0, 0, 0, 1, 0, 1, 0, 0, 1, 0, 1, 1, 0, 0 },
		new int[20]{ 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 0, 1, 1, 0 },
		new int[20]{ 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1, 1, 0, 1, 0, 0 },
		new int[20]{ 1, 0, 0, 1, 1, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0 },
		new int[20]{ 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0 },
		new int[20]{ 1, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0 },
		new int[20]{ 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0, 1, 0, 1, 0, 0, 0 },
		new int[20]{ 1, 0, 0, 0, 0, 0, 1, 1, 0, 1, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1 },
		new int[20]{ 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
		new int[20]{ 0, 0, 0, 1, 1, 1, 0, 1, 1, 0, 0, 0, 1, 1, 0, 1, 0, 1, 0, 0 },
		new int[20]{ 1, 0, 0, 1, 0, 0, 1, 0, 0, 0, 1, 0, 1, 0, 0, 1, 1, 0, 1, 0 },
		new int[20]{ 0, 0, 1, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 1, 0, 0, 1, 0, 0, 1 },
		new int[20]{ 0, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0 },
		new int[20]{ 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 1, 0, 1, 0, 0, 0, 0, 0 },
		new int[20]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0 },
		new int[20]{ 0, 0, 0, 1, 0, 0, 0, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 0, 0 },
		new int[20]{ 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1 },
		new int[20]{ 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 1 },
		new int[20]{ 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0, 0, 0, 1, 0 } };
	public GameObject boxPrefab;
	public GameObject emptySlot;
	// Use this for initialization
	void Start () {
		//InilializeMap ();

	}

	public void InilializeMap(){
		for (int z = 0; z < map.Length; z++) {
			for (int x = 0; x < map[0].Length; x++) {
				float xPos = this.transform.position.x + x;
				float yPos = this.transform.position.y + 0.5f;
				float zPos = this.transform.position.z + z;
				GameObject clone;
				if(map[z][x] == 1) {
					clone = boxPrefab;
				}
				else{
					clone = emptySlot;
				}
				clone = GameObject.Instantiate(clone,
				                               new Vector3(xPos, yPos, zPos),
				                               this.transform.rotation * clone.transform.rotation) as GameObject;
				clone.transform.parent = this.transform;
			}
		}
	}

	public void Reset(){
		GameObject[] children = GameObject.FindGameObjectsWithTag ("Obstacle");
		foreach(GameObject child in children){
			Destroy(child);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
