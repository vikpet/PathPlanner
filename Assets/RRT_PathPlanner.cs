using UnityEngine;
using System.Collections;

public class RRT_PathPlanner : MonoBehaviour {
	public GameObject MotionModel;
	private DynamicCarController controller;
	// Use this for initialization
	void Start () {
		controller = MotionModel.GetComponent<DynamicCarController> ();
//		DynamicCarController.State ;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
