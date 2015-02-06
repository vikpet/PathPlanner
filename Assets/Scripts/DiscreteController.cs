using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DiscreteController : MonoBehaviour {

	public int speed;
	public int[][] map;
	private LinkedList<Vector3> path;


	//start(2,2) , goal(15,20);

	void Start ()
	{
		map = GameObject.Find ("Obstacles").GetComponent<LoadObstacles>().map;
		path = A_StarPathPlanner.aStar ( map, new A_StarPathPlanner.Point(1,1), new A_StarPathPlanner.Point(14,19));
		StartCoroutine (SpawnWaves ());
	}
	
	IEnumerator SpawnWaves ()
	{
		yield return new WaitForSeconds (speed);
		while (path.Count>0)
		{
			transform.position = path.First.Value;
			path.RemoveFirst();
			yield return new WaitForSeconds (speed);
		}
	}

}
