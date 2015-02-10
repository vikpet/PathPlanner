using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathSmoothener : MonoBehaviour {

	public Transform motionModel;

	public float alpha;
	public float beta;
	public float tolerance;

	private List<Transform> waypoints;
	// Use this for initialization
	void Start () {
		waypoints = new List<Transform>();
		foreach(Transform child in transform) {
			waypoints.Add(child);
		}
		SmoothenPath ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void SmoothenPath() {
		Vector3[] x = new Vector3[waypoints.Count + 1];
		Vector3[] y = new Vector3[waypoints.Count + 1];
		x [0] = y [0] = motionModel.position;
		for (int i = 0; i < waypoints.Count; i++) {
			x[i + 1] = y[i + 1] = waypoints[i].position;
		}

		float change = tolerance;
		while (change >= tolerance) {
			change = 0.0f;
			for (int i = 1; i < x.Length - 1; i++) {
				Vector3 tmp = y[i];
				y [i] += alpha * (x [i] - y [i]) + beta * (y [i + 1] + y [i - 1] - 2 * y [i]);	
				change += Mathf.Abs((tmp - y[i]).magnitude);
			}
		}
		for (int i = 0; i < waypoints.Count; i++) {
			waypoints[i].position = y[i + 1];
		}
	}

}
