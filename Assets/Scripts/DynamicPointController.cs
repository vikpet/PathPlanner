using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class DynamicPointController : MonoBehaviour {
	public GameObject waypoints;
	public float aMax;
	public float minDistance;	// Some smoothening should be used. Maybe random?

	private int count;
	private Transform target;
	// Use this for initialization

	public GameObject map;
//private LinkedList<Vector3> path;

	void Start () {

		count = 0;

		GetNextWaypoint ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate() {
		if (target != null) {
			MoveTowards (target.position);
		}
	}

	private void MoveTowards(Vector3 tarPos) {
		Vector3 direction = tarPos - transform.position; // Calculate the direction the target is in.
		direction *= 1.0f;
		Vector3 velocity = rigidbody.velocity;
		Vector3 velocityChange = (direction - velocity);

		velocityChange.y = 0;
		if (velocityChange.magnitude > aMax) {
			velocityChange = velocityChange.normalized * aMax;
		}
		rigidbody.AddForce(velocityChange, ForceMode.Acceleration);

		if ((transform.position - target.transform.position).sqrMagnitude <= minDistance * minDistance)
		{
			count++;
			GetNextWaypoint();
		}
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "PickUp") {
			other.gameObject.SetActive (false);
			count++;
			GetNextWaypoint();
		}
		if (other.gameObject.tag == "Obstacle") {
			Debug.Log(" --- Collision! --- ");
		}
	}
	

	void GetNextWaypoint() {
		if (count >= waypoints.transform.childCount) {
		} else {
			target = waypoints.transform.GetChild (count);
		}
	}

	void CalcRandomTree() {
		float stepLength = 1.0f;

	}
}
