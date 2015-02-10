using UnityEngine;
using System.Collections;

public class HoverMotor : MonoBehaviour {
	public float speed = 90;
	public float turnSpeedRadians = 1f;
	public float hoverForce = 65f;
	public float hoverHeight = 0.5f;

	//public float turnSpeedDegrees;

	private float powerInput;
	private float turnInput;
	private Rigidbody carRigidbody;

	
	public GameObject waypoints;
	public float aMax;
	public float minDistance;	// Some smoothening should be used. Maybe random?
	
	public GUIText countText;
	
	private int count;
	private Transform target;

	private bool finish;

	// Use this for initialization
	void Awake() {

		carRigidbody = GetComponent <Rigidbody> ();

	}

	// Use this for initialization
	void Start () {
		count = 0;
		SetCountText ();

		//turnSpeedDegrees = turnSpeedRadians * 180 / Mathf.PI;
		finish = false;
		GetNextWaypoint ();

	}


	void FixedUpdate(){
		Ray ray = new Ray (transform.position, -transform.up);
		RaycastHit hit; 

		if(Physics.Raycast(ray,out hit, hoverHeight)){
			float proportionalHeight = (hoverHeight - hit.distance)  / hoverHeight;
			Vector3 appliedHoverForce = Vector3.up * proportionalHeight * hoverForce;
			carRigidbody.AddForce(appliedHoverForce,ForceMode.Acceleration);
			//carRigidbody.AddForce(appliedHoverForce);
		}



		if(!finish){
			MoveTowards (target.position);
		}

//		carRigidbody.AddRelativeForce (0f,0f,powerInput * speed);
//
//		carRigidbody.AddRelativeTorque (0f,turnInput*turnSpeed,0f);


	}

	private void MoveTowards(Vector3 tarPos) {
		//Vector3 direction = tarPos - transform.position; // Calculate the direction the target is in.

		Vector3 targetDir = tarPos - transform.position;

		//float angle = Vector3.Angle (difPos,Vector3.forward);


		float step = turnSpeedRadians * Time.deltaTime;



		Vector3 newRotation = Vector3.RotateTowards (transform.forward, targetDir, step, 0.0f);

		Debug.DrawRay(transform.position, newRotation*2	, Color.red);

		transform.rotation = Quaternion.LookRotation(newRotation);

		//carRigidbody.rotation = Quaternion.Euler (Vector3.RotateTowards());


		//carRigidbody.rotation = Quaternion.Euler(Mathf.Sign(difPos.x)*Vector3.up*angle);



		carRigidbody.velocity = carRigidbody.transform.forward * 4;


		Debug.Log (
			"Target position: " + tarPos
			+"\n Car position: " + carRigidbody.position
			+"\n Target - car: " + (tarPos-carRigidbody.transform.position)
			+"\n Angle : " + step

			);

//		direction *= 1.0f;
//		Vector3 velocity = rigidbody.velocity;
//		Vector3 velocityChange = (direction - velocity);
//		//		velocityChange.x = Mathf.Clamp(velocityChange.x, -aMax, aMax);
//		//		velocityChange.z = Mathf.Clamp(velocityChange.z, -aMax, aMax);
//		velocityChange.y = 0;
//		if (velocityChange.magnitude > aMax) {
//			velocityChange = velocityChange.normalized * aMax;
//		}
//		rigidbody.AddForce(velocityChange, ForceMode.Acceleration);
		
		// todo: need to rotate the model towards the waypoint we are moving towards
		
		// gravity
		//controller.AddForce(0, -gravity * controller.mass, 0);
		
		
		/*Vector3 movement = new Vector3 (moveHorizontal, 0, moveVertical);
		
		rigidbody.AddForce (movement * speed * Time.deltaTime);

		direction -= Vector3.forward;

		if (direction != Vector3(0,0,0)) {
	    	rigidbody.AddForce(direction.normalized*moveSpeed*Time.deltaTime);
		}*/
		
//		if ((transform.position - target.transform.position).sqrMagnitude <= minDistance * minDistance)
//		{
//			count++;
//			GetNextWaypoint();
//		}
	}
	
	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "PickUp") {
			other.gameObject.SetActive (false);

			count++;
			SetCountText();
			GetNextWaypoint();
			if(target == null){
				finish = true;
			}
		}
	}
	
	void SetCountText() {
		countText.text = "Count: " + count.ToString ();
	}
	
	void GetNextWaypoint() {
		if (count >= waypoints.transform.childCount) {
			countText.text = " --- Done! --- ";
			finish = true;
		} else {

			target = waypoints.transform.GetChild (count);

			//target = waypoints.transform.GetChild (0);
		}
	}
}
