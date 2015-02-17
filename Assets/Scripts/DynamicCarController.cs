using UnityEngine;
using System.Collections;

public class DynamicCarController : MonoBehaviour, CarModelInterface {
	public float CURRENT_VELOCITY;
	public float a_max;
	public float phi_max;
	public float car_length;
		
	private Rigidbody carRigidbody;

	public GameObject waypoints;
	public float minDistance;	// Some smoothening should be used. Maybe random?
	
	public GUIText countText;
	
	private int count;
	private int childCount;
	private Transform target;
	private const float BRAKE_THRESHHOLD = 0.001f;
	private bool finish;
	public ArrayList lines;
	private float startTime;


	public Stack states;
	private CarState initialState;

	private Vector3 prevDir;
	private Vector3 prevP;

	private const float DELTA_TIME = 0.02f;

	// Use this for initialization
	void Awake() {
		
		carRigidbody = GetComponent <Rigidbody> ();
		
	}

	public float DistanceTo(CarState s, Vector3 point){
		Vector3 relHit = point - s.position;
		
		return relHit.magnitude + Vector3.Angle (relHit, s.rotation * Vector3.forward)*0.5f;
	}
	
	public CarState GetNextState (CarState s, Vector3 point){
		
		CarState currentState = s.Copy();
		currentState.instruction = point;
		
		Ray ray;
		RaycastHit hit;
		
		for(int i = 0; i <50; i++){
			
			//decision
			currentState = MoveTowards(currentState,point,DELTA_TIME);
			
			// Collision Detection
			ray = new Ray (s.position, currentState.velocity.normalized);
			if (Physics.Raycast (ray, out hit, currentState.velocity.magnitude)) {
				currentState.collision = true;
				break;
			}else{
				lines.Add (new Vector3[2] {currentState.position,currentState.position + currentState.velocity/50});
				currentState.position = currentState.position + currentState.velocity/50;
			}
			
		}
		return currentState;
	}
	
	public void FollowStates(Stack s){
		states = s;
		initialState = (CarState)states.Pop ();
		prevDir = initialState.velocity;
		//Debug.Log (prevDir+" "+prevDir.magnitude);
		prevP = initialState.position;
		count = 0;
	}
	
	public Vector3 StartPosition(){
		return transform.position;
	}
	
	// Use this for initialization
	void Start () {
		count = 0;
		SetCountText ();
		
		finish = false;

		finish = false;
		lines = new ArrayList();
		count = -1;
//		RRTCarRandomState rrt = GetComponent <RRTCarRandomState>();
		RRT_Car rrt = GetComponent <RRT_Car>();
		rrt.SetModel (this);

		childCount = 0;
		SetCountText ();

		finish = false;
		target = waypoints.transform.GetChild (0);
		Debug.Log ("Target: " + target);
	}
	
	void FixedUpdate() {
		
		//		int count = 0;
		
		if(states != null && states.Count > 0){
			if(!finish){
				startTime = Time.time;
				finish = true;
			}
			CarState goTo = (CarState)states.Peek();
			
			initialState.position = transform.position;
			initialState.velocity = rigidbody.velocity;
			
			
			initialState = MoveTowards(initialState, goTo.instruction, 0.02f);

			rigidbody.rotation = initialState.rotation;
			rigidbody.velocity = initialState.velocity;

			count++;
			if(count >= 50){
				states.Pop ();
				//Debug.Log ((prevDir-initialState.direction) + " " + (prevDir-initialState.direction).magnitude);
				prevDir = initialState.velocity;
				//Debug.Log ((prevP-initialState.position).magnitude);
				//Debug.Log (initialState.direction.magnitude);
				//Debug.Log(states.Count);
				count = 0;
				RaycastHit hit;
				//				ray = new Ray(transform.position, (localgoal.position-transform.position).normalized);
				//				if (!Physics.Raycast (ray, out hit, (localgoal.position-transform.position).magnitude)) {
				//					states.Clear();
				//					count = 0;
				//					
				//				}
				
			}
			
			countText.text = "Time: " +(Time.time-startTime) ;
			
		}else if(count == 0 /*&& Vector3.Distance(initialState.position,localgoal.position)>0.1f*/){
			if(!finish){
				startTime = Time.time;
				finish = true;
			}
			
		/*	if(initialState.velocity.magnitude>aMax || Vector3.Distance(initialState.position,localgoal.position)>0.1f){
				initialState.position = transform.position;
				initialState.direction = rigidbody.velocity;
				initialState = MoveTowards(initialState, localgoal.position);
				rigidbody.velocity = initialState.direction;
				
			}else{
				rigidbody.velocity = new Vector3(0f,0f,0f);
				initialState.direction = rigidbody.velocity;
				
				count = -1;
			}*/
			countText.text = "Time: " +(Time.time-startTime) ;
			
		}
		
	}
/**	void Update(){		
		
		if(!finish){
			CarState next = MoveTowards (new CarState(transform.position, carRigidbody.velocity, transform.rotation, 0f, 0f), target.position, Time.deltaTime);
			carRigidbody.rotation = next.rotation;
			carRigidbody.velocity = next.velocity;
		}
	}*/
	
	private CarState MoveTowards(CarState currentState, Vector3 tarPos, float delta_time) {
	
		Vector3 targetDir = tarPos - currentState.position;
		
		targetDir.y = 0.0f; 

		if((Vector3.Dot(currentState.velocity, currentState.rotation * Vector3.forward)) < 0) {
			CURRENT_VELOCITY = -currentState.velocity.magnitude;
		} else {
			CURRENT_VELOCITY = currentState.velocity.magnitude;
		}
		//CURRENT_VELOCITY = currentState.velocity; // TODO THIS MIGHT BE WRONG
		Debug.Log("Current Velocity: " + CURRENT_VELOCITY);
		float theta_max = currentState.velocity.magnitude / car_length * Mathf.Tan(phi_max);
		float turning_radius = currentState.velocity.magnitude * currentState.velocity.magnitude / a_max;
//		float turning_perimiter_length = 2.0f * Mathf.PI * turning_radius;
		float angular_velocity = Mathf.Abs(CURRENT_VELOCITY) / turning_radius;
		float stopping_distance = CURRENT_VELOCITY * CURRENT_VELOCITY / (2.0f * a_max);

		if (angular_velocity > theta_max) {
			angular_velocity = theta_max;
		}
		
		Vector3 newRotation;
		
		if (Mathf.Abs (Vector3.Angle (targetDir, currentState.rotation * Vector3.forward)) > 60.0f) {	// We should back.
			Debug.Log("Target behind car. Angle: " + Mathf.Abs (Vector3.Angle (targetDir, currentState.rotation * Vector3.forward)));

			newRotation = Vector3.RotateTowards (currentState.rotation * Vector3.forward, targetDir, angular_velocity * delta_time, 0.0f);
			newRotation.y = 0.0f;
			currentState.rotation = Quaternion.LookRotation(newRotation);
			currentState.velocity = (currentState.rotation * Vector3.forward).normalized * (CURRENT_VELOCITY - a_max * delta_time);

		} else if (targetDir.magnitude <= stopping_distance){ // We should break.

			newRotation = Vector3.RotateTowards (currentState.rotation * Vector3.forward, targetDir, angular_velocity * delta_time, 0.0f);
			newRotation.y = 0.0f;
			currentState.rotation = Quaternion.LookRotation(newRotation);
			if(CURRENT_VELOCITY > 0) {
				currentState.velocity = (currentState.rotation * Vector3.forward).normalized * (CURRENT_VELOCITY - a_max * delta_time);
			} else {
				currentState.velocity = (currentState.rotation * Vector3.forward).normalized * (CURRENT_VELOCITY + a_max * delta_time);
			}
			Debug.Log("Break");

		} else {	// else accelerate.
			newRotation = Vector3.RotateTowards (currentState.rotation * Vector3.forward, targetDir, angular_velocity * delta_time, 0.0f);
			newRotation.y = 0.0f;
			currentState.rotation = Quaternion.LookRotation(newRotation);
			currentState.velocity = (currentState.rotation * Vector3.forward).normalized * (CURRENT_VELOCITY + a_max * delta_time);

			Debug.Log("Accelerate");

		}
		
		Debug.DrawRay(currentState.position, newRotation*2	, Color.red);

		if ((currentState.position - tarPos).sqrMagnitude <= minDistance * minDistance)
		{
			childCount++;
			GetNextWaypoint();
		}
		return currentState;

	}
	
	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "PickUp") {
			other.gameObject.SetActive (false);
			
//			childCount++;
			SetCountText();
//			GetNextWaypoint();
			if(target == null){
				finish = true;
			}
		}
	}
	
	void SetCountText() {
		countText.text = "Count: " + childCount.ToString ();
	}
	
	void GetNextWaypoint() {
		if (childCount >= waypoints.transform.childCount) {
			countText.text = " --- Done! --- ";
			finish = true;
		} else {
			target = waypoints.transform.GetChild (childCount);
		}
	}

	public CarState GetNextState(Vector3 targetPos, CarState current) {
		return MoveTowards (current, targetPos, 1.0f);
	}
	
	void OnDrawGizmos() {
		Gizmos.color = Color.black;
		if(lines == null){
			return;
		}
		foreach(Vector3[] line in lines){
			Gizmos.DrawLine(line[0],line[1]);
		}
	}
}

