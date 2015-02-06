using UnityEngine;
using System.Collections;

public class DynamicPointController : MonoBehaviour {
	public GameObject waypoints;
	public float aMax;
	public float minDistance;	// Some smoothening should be used. Maybe random?
	
	public GUIText countText;

	private int count;
	private Transform target;
	// Use this for initialization
	void Start () {
		count = 0;
		SetCountText ();
		
		GetNextWaypoint ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate() {

		MoveTowards (target.position);
	}

	private void MoveTowards(Vector3 tarPos) {
		Vector3 direction = tarPos - transform.position; // Calculate the direction the target is in.
		direction *= 1.0f;
		Vector3 velocity = rigidbody.velocity;
		Vector3 velocityChange = (direction - velocity);
		velocityChange.x = Mathf.Clamp(velocityChange.x, -aMax, aMax);
		velocityChange.z = Mathf.Clamp(velocityChange.z, -aMax, aMax);
		velocityChange.y = 0;
		rigidbody.AddForce(velocityChange.normalized, ForceMode.Acceleration);
		
		// todo: need to rotate the model towards the waypoint we are moving towards
		
		// gravity
		//controller.AddForce(0, -gravity * controller.mass, 0);


		/*Vector3 movement = new Vector3 (moveHorizontal, 0, moveVertical);
		
		rigidbody.AddForce (movement * speed * Time.deltaTime);

		direction -= Vector3.forward;

		if (direction != Vector3(0,0,0)) {
	    	rigidbody.AddForce(direction.normalized*moveSpeed*Time.deltaTime);
		}*/

		if ((transform.position - target.transform.position).sqrMagnitude <= minDistance * minDistance)
		{
			count++;
			GetNextWaypoint();
		}
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "PickUp") {
			other.gameObject.SetActive (false);
			//count++;
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
