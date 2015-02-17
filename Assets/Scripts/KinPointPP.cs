using UnityEngine;
using System.Collections;



public class KinPointPP : MonoBehaviour, ModelInterface {
	public float speed ;
	//private float speedMultiplier = 4;

	public GUIText countText;
	public Stack states;
	//private float closeness = 0.5f;
	private float startTime;
	private bool finish ;
	private int count;
	private State initialState;
	//public float hoverForce = 65f;
	//public float hoverHeight = 0.5f;

	//public float turnSpeedDegrees;

//	private float powerInput;
//	private float turnInput;
	private Rigidbody ourRigidbody;

	public ArrayList lines;

//
//	
//	public GameObject waypoints;
//	public float aMax;
//	public float minDistance;	// Some smoothening should be used. Maybe random?
//	
//	public GUIText countText;
//	
//	private int count;
//	private Transform target;

	public Transform goal;

//	private bool finish;

	// Use this for initialization
	void Awake() {

		ourRigidbody = GetComponent <Rigidbody> ();

	}

	public void FollowStates(Stack s){
		states = s;
		initialState = (State)states.Pop ();
	}
	public Vector3 StartPosition(){
		return transform.position;
	}

	// Use this for initialization
	void Start () {
		//count = 0;
		//SetCountText ();
		count = -1;

		finish = false;

		lines = new ArrayList();

		RRTTest rrt = GetComponent <RRTTest>();
		rrt.SetModel (this);
		//turnSpeedDegrees = turnSpeedRadians * 180 / Mathf.PI;
		//finish = false;
		//GetNextWaypoint ();

	}
	public float DistanceTo(State s, Vector3 point){
		return Vector3.Distance(s.position,point);
	}


	void FixedUpdate(){


		if (states != null && states.Count > 0) {
			if (!finish) {
				startTime = Time.time;
				finish = true;
			}
			State goTo = (State)states.Peek ();
//			float step = speed * Time.deltaTime;
//			transform.position = Vector3.MoveTowards (transform.position, goTo.direction, step);
//			
//			if (Vector3.Distance (transform.position, goTo.position) < closeness) {
//				states.Pop ();
//			}	

			initialState.position = transform.position;
			initialState.direction = rigidbody.velocity;

			initialState = GetNextState(initialState, goTo.point);



			count++;
			if(count>=50){
				states.Pop();

			}
			countText.text = "Time: " +(Time.time-startTime) ;


		} else if (finish) {



		}


		//Ray ray = new Ray (transform.position, -transform.up);
		//RaycastHit hit; 

//		if(Physics.Raycast(ray,out hit, hoverHeight)){
//			float proportionalHeight = (hoverHeight - hit.distance)  / hoverHeight;
//			Vector3 appliedHoverForce = Vector3.up * proportionalHeight * hoverForce;
//			carRigidbody.AddForce(appliedHoverForce,ForceMode.Acceleration);
//			//carRigidbody.AddForce(appliedHoverForce);
//		}


//
//		if(!finish){
//			MoveTowards (target.position);
//		}

//		carRigidbody.AddRelativeForce (0f,0f,powerInput * speed);
//
//		carRigidbody.AddRelativeTorque (0f,turnInput*turnSpeed,0f);


	}

	public State GetNextState(State s, Vector3 v){

		Vector3 point = v-s.position;

		Ray ray = new Ray (s.position, point.normalized);
		RaycastHit hit; 

		Vector3 direction = new Vector3 (0f,0f,0f);

		float fScore;
		float gScore;
		bool collision = false;
		Vector3 newPosition;
		if (Physics.Raycast (ray, out hit, speed)) {
			gScore = s.gScore+hit.distance;
			newPosition = s.position;//+(hit.distance*pointDir);
			fScore = gScore ;//+ Vector3.Distance(newPosition,goal.position);
			//return new State(newPosition,hit.distance*pointDir, fScore, gScore);

			collision = true;

		} else {
			gScore = s.gScore++;
			fScore = gScore + Vector3.Distance(s.position+pointDir,goal.position);
			//pointDir *= speed;

			if(point.magnitude<speed){

				direction = point;
				newPosition = point;

			}else{
				direction = point.normalized*speed;
				newPosition = s.position+point.normalized*speed;
			}


		}

		lines.Add (new Vector3[2] {s.position,newPosition});
	
		State retState = new State(newPosition,direction,fScore , gScore, v);
		retState.collision = collision;
//		retState.direction = pointDir*speed;
		countText.text = ""+lines.Count;
		return retState;

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
	
	
	private void MoveTowards(State currentState,Vector3 tarPos) {
		//Vector3 direction = tarPos - transform.position; // Calculate the direction the target is in.

		Vector3 point = tarPos - transform.position;


//		if(point.magnitude<speed){
//			
//			direction = point;
//			newPosition = point;
//			
//		}else{
//			direction = point.normalized*speed;
//			newPosition = s.position+point.normalized*speed;
//		}

		//float angle = Vector3.Angle (difPos,Vector3.forward);


		//float step = turnSpeedRadians * Time.deltaTime;


		//float step = (carRigidbody.velocity.magnitude/speedMultiplier)*turnSpeedRadians * Time.deltaTime;

		
		//Debug.Log (step + " " + (carRigidbody.velocity.magnitude/speedMultiplier)*step);


		targetDir.y = 0.0f;

		//Vector3 newRotation = Vector3.RotateTowards (transform.forward, targetDir, step, 0.0f);



		//Debug.DrawRay(transform.position, newRotation*2	, Color.red);
		

		//transform.rotation = Quaternion.LookRotation(newRotation);




		//carRigidbody.rotation = Quaternion.Euler (Vector3.RotateTowards());


		//carRigidbody.rotation = Quaternion.Euler(Mathf.Sign(difPos.x)*Vector3.up*angle);



		//carRigidbody.velocity = carRigidbody.transform.forward * speedMultiplier;







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
	
//	void OnTriggerEnter(Collider other) {
//		if (other.gameObject.tag == "PickUp") {
//			other.gameObject.SetActive (false);
//
//			count++;
//			SetCountText();
//			GetNextWaypoint();
//			if(target == null){
//				finish = true;
//			}
//		}
//	}
	
//	void SetCountText() {
//		countText.text = "Count: " + count.ToString ();
//	}
//	
//	void GetNextWaypoint() {
//		if (count >= waypoints.transform.childCount) {
//			countText.text = " --- Done! --- ";
//			finish = true;
//		} else {
//
//			target = waypoints.transform.GetChild (count);
//
//			//target = waypoints.transform.GetChild (0);
//		}
//	}
}
