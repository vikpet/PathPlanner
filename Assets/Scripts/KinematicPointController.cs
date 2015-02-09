using UnityEngine;
using System.Collections;

public class KinematicPointController : MonoBehaviour {
	public GameObject waypoints;
	public float vMax;
	public const float minDistance = 0.2f;

	public UnityEngine.UI.Text countText;

	private int count;
	private Transform target;


	// Use this for initialization
	void Start () {
		count = 0;
		SetCountText ();

		GetNextWaypoint ();
	}

	void FixedUpdate() {
		float step = vMax * Time.deltaTime;
		transform.position = Vector3.MoveTowards(transform.position, target.position, step);

		if ((transform.position - target.transform.position).sqrMagnitude <= minDistance * minDistance)
		{
			GetNextWaypoint();
		}	
	}
	// Update is called once per frame
	void Update () {

	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "PickUp") {
			other.gameObject.SetActive (false);
			count++;
			SetCountText();
//			GetNextWaypoint();
		}
	}
	
	void SetCountText() {
		countText.text = "Count: " + count.ToString ();
	}

	void GetNextWaypoint() {
		if (count >= waypoints.transform.childCount) {
			countText.text = " --- Done! --- ";
		} else {
			target = waypoints.transform.GetChild (count);
		}
	}
}
