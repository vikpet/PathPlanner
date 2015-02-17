using UnityEngine;
using System.Collections;

public class DynamicCarController : MonoBehaviour {
	public float CURRENT_VELOCITY;
	public float a_max;
	public float phi_max;
	public float car_length;
		
	private Rigidbody carRigidbody;
	
	
	public GameObject waypoints;
	public float minDistance;	// Some smoothening should be used. Maybe random?
	
	public GUIText countText;
	
	private int count;
	private Transform target;
	private const float BRAKE_THRESHHOLD = 0.001f;
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
			State next = MoveTowards (target.position, new State(transform, carRigidbody.velocity), Time.deltaTime);
			carRigidbody.velocity = next.velocity;
		}
	}
	
	private State MoveTowards(Vector3 tarPos, State currentState, float delta_time) {
	
		Vector3 targetDir = tarPos - currentState.trans.position;
		
		targetDir.y = 0.0f; 

		CURRENT_VELOCITY = currentState.trans.InverseTransformDirection(currentState.velocity).z;
		float theta_max = currentState.velocity.magnitude / car_length * Mathf.Tan(phi_max);
		float turning_radius = currentState.velocity.magnitude * currentState.velocity.magnitude / a_max;
//		float turning_perimiter_length = 2.0f * Mathf.PI * turning_radius;
		float angular_velocity = Mathf.Abs(CURRENT_VELOCITY) / turning_radius;
		float stopping_distance = CURRENT_VELOCITY * CURRENT_VELOCITY / (2.0f * a_max);

		if (angular_velocity > theta_max) {
			angular_velocity = theta_max;
		}
		
		Vector3 newRotation;
		
		if (Mathf.Abs (Vector3.Angle (targetDir, currentState.trans.forward)) > 60.0f) {	// We should back.
			Debug.Log("Target behind car. Angle: " + Mathf.Abs (Vector3.Angle (targetDir, currentState.trans.forward)));

			newRotation = Vector3.RotateTowards (currentState.trans.forward, targetDir, angular_velocity * delta_time, 0.0f);
			newRotation.y = 0.0f;
			currentState.trans.rotation = Quaternion.LookRotation(newRotation);
			currentState.velocity = currentState.trans.forward * (CURRENT_VELOCITY - a_max * delta_time);
		} else if (targetDir.magnitude <= stopping_distance){ // We should break.

			newRotation = Vector3.RotateTowards (currentState.trans.forward, targetDir, angular_velocity * delta_time, 0.0f);
			newRotation.y = 0.0f;
			currentState.trans.rotation = Quaternion.LookRotation(newRotation);
			if(CURRENT_VELOCITY > 0) {
				currentState.velocity = currentState.trans.forward * (CURRENT_VELOCITY - a_max * delta_time);
			} else {
				currentState.velocity = currentState.trans.forward * (CURRENT_VELOCITY + a_max * delta_time);
			}
			Debug.Log("Break");

		} else {	// else accelerate.
			newRotation = Vector3.RotateTowards (currentState.trans.forward, targetDir, angular_velocity * delta_time, 0.0f);
			newRotation.y = 0.0f;
			currentState.trans.rotation = Quaternion.LookRotation(newRotation);
			currentState.velocity = currentState.trans.forward * (CURRENT_VELOCITY + a_max * delta_time);

			Debug.Log("Accelerate");

		}
		
		Debug.DrawRay(currentState.trans.position, newRotation*2	, Color.red);

//		transform.rotation = Quaternion.LookRotation(newRotation);

		/*
		Debug.Log (
			"Target position: " + tarPos
			+"\n Car position: " + carRigidbody.position
			+"\n Target - car: " + (tarPos-carRigidbody.transform.position)
			+"\n Angle : " + theta_max
			
			);*/

		if ((currentState.trans.position - tarPos).sqrMagnitude <= minDistance * minDistance)
		{
			count++;
			GetNextWaypoint();
		}
		return currentState;

	}
	
	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "PickUp") {
			other.gameObject.SetActive (false);
			
//			count++;
			SetCountText();
//			GetNextWaypoint();
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

	public State GetNextState(Vector3 targetPos, State current) {
		return MoveTowards (targetPos, current, 1.0f);
	}


}
public class State {
	public Transform trans;
	public Vector3 velocity;
	public State(Transform p, Vector3 v){
		this.trans = p;
		this.velocity = v;
	}
}
