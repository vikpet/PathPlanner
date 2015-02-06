using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DiscreteController : MonoBehaviour {

	public float delay;
	public int[][] map;
	private LinkedList<Vector3> path;
	//public GUIText countText;
	public UnityEngine.UI.Text countText;
	public int hood;
	private float count;





	//start(2,2) , goal(15,20);



	void Start ()
	{

	}

	private float RoundTo(float rMe, int toDP){
		return Mathf.Round (rMe * Mathf.Pow(10 , toDP)) / (Mathf.Pow(10 , toDP));
	}

	public void Run(int hood){
		LoadObstacles obstacles = GameObject.Find ("Obstacles").GetComponent<LoadObstacles>();
		obstacles.Reset ();
		obstacles.InilializeMap ();
		map = obstacles.map;
		transform.position = new Vector3 (1.0f, 0.5f, 1.0f);
		path = A_StarPathPlanner.aStar ( map, new A_StarPathPlanner.Point(1,1), 
		                                new A_StarPathPlanner.Point(19,14), hood);
		StartCoroutine (SpawnWaves ());
	}

	
	IEnumerator SpawnWaves ()
	{
		yield return new WaitForSeconds (delay);
		count = 0.0f;
		while (path.Count>0)
		{

			SetCountText();


			Debug.Log ("Curr x: " + transform.position.x + " z: " + transform.position.z +
			           " next x: " + path.First.Value.x + " z: " + path.First.Value.z +" dist: " +
			           RoundTo(Vector3.Distance(transform.position,path.First.Value),2));

			count += RoundTo(Vector3.Distance(transform.position,path.First.Value),2); 
			transform.position = path.First.Value;
			path.RemoveFirst();
			yield return new WaitForSeconds (delay);
		}
		SetCountText();

		gameObject.SetActive (false);
	}

	void SetCountText() {

		countText.text = "Count: " + count.ToString ();
	}

}
