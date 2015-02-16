using UnityEngine;
using System.Collections;

public class KinematicCarController : MonoBehaviour {

	public float v_max;
	public float phi_max;
	public float car_length;
	private Rigidbody carRigidbody;
	
	
	public GameObject waypoints;
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
	
	
	void Update(){		
		
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

		targetDir.y = 0.0f; 


		float delta_theta = v_max / car_length * Mathf.Tan(phi_max);

		Vector3 newRotation;

		if (Mathf.Abs (Vector3.Angle (targetDir, rigidbody.transform.forward)) > 90.0f) {
			newRotation = Vector3.RotateTowards (transform.forward, targetDir, delta_theta * Time.deltaTime, 0.0f);
			newRotation.y = 0.0f;
			carRigidbody.velocity = carRigidbody.transform.forward * v_max * -1;
		} else {
			newRotation = Vector3.RotateTowards (transform.forward, targetDir, delta_theta * Time.deltaTime, 0.0f);
			newRotation.y = 0.0f;
			carRigidbody.velocity = carRigidbody.transform.forward * v_max;
		}

		Debug.DrawRay(transform.position, newRotation*2	, Color.red);
		
		transform.rotation = Quaternion.LookRotation(newRotation);

		Debug.Log (
			"Target position: " + tarPos
			+"\n Car position: " + carRigidbody.position
			+"\n Target - car: " + (tarPos-carRigidbody.transform.position)
			+"\n Angle : " + delta_theta
			
			);
		

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
