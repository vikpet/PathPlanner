using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class DynamicPointController : MonoBehaviour, ModelInterface {
	public GameObject waypoints;
	public Transform localgoal;
	public float aMax = 0.1f;
	public float minDistance;	// Some smoothening should be used. Maybe random?
	public Stack states;
	private State initialState;
	public GUIText countText;
	private Vector3 prevDir;
	private int count;
	private Transform target;
	private Vector3 prevP;
	private float startTime;
	private bool finish ;
	// Use this for initialization
	public ArrayList lines;

	private Ray ray;
	//private float closeness = 0.1f;

	//public GameObject map;


	public float DistanceTo(State s, Vector3 point){
		Vector3 relHit = point - s.position;

		return relHit.magnitude + Vector3.Angle (relHit,s.direction)*0.01f;
	}

	public State GetNextState (State s, Vector3 point){

		State currentState = s.Copy();
		currentState.point = point;

		Ray ray;
		RaycastHit hit;

		for(int i = 0; i <50; i++){

			//decision
			currentState = MoveTowards(currentState,point);

			// Collision Detection
			ray = new Ray (s.position, currentState.direction.normalized);
			if (Physics.Raycast (ray, out hit, currentState.direction.magnitude)) {
				currentState.collision = true;
				break;
			}else{
				lines.Add (new Vector3[2] {currentState.position,currentState.position + currentState.direction/50});
				currentState.position = currentState.position + currentState.direction/50;
			}

		}
		return currentState;


	}

	public void FollowStates(Stack s){
		states = s;
		initialState = (State)states.Pop ();
		prevDir = initialState.direction;
		//Debug.Log (prevDir+" "+prevDir.magnitude);
		prevP = initialState.position;
		count = 0;
	}

	public Vector3 StartPosition(){
		return transform.position;
	}

	void Start () {

		finish = false;
		lines = new ArrayList();
		count = -1;
		RRTTest rrt = GetComponent <RRTTest>();
		rrt.SetModel (this);


	}


	void FixedUpdate() {

//		int count = 0;

		if(states != null && states.Count > 0){
			if(!finish){
				startTime = Time.time;
				finish = true;
			}
			State goTo = (State)states.Peek();

			initialState.position = transform.position;
			initialState.direction = rigidbody.velocity;


			initialState = MoveTowards(initialState, goTo.point);

			rigidbody.velocity = initialState.direction;

			count++;
			if(count >= 50){
				states.Pop ();
				//Debug.Log ((prevDir-initialState.direction) + " " + (prevDir-initialState.direction).magnitude);
				prevDir = initialState.direction;
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

			if(initialState.direction.magnitude>aMax || Vector3.Distance(initialState.position,localgoal.position)>0.1f){
				initialState.position = transform.position;
				initialState.direction = rigidbody.velocity;
				initialState = MoveTowards(initialState, localgoal.position);
				rigidbody.velocity = initialState.direction;

			}else{
				rigidbody.velocity = new Vector3(0f,0f,0f);
				initialState.direction = rigidbody.velocity;

				count = -1;
			}
			countText.text = "Time: " +(Time.time-startTime) ;

		}

	}

	private State MoveTowards(State s,Vector3 v) {

		State currentState = s;
		Vector3 relHitPoint = v - s.position;
		Vector3 halfPoint = relHitPoint / 2;

		float dotSdirPoint = Vector3.Dot (s.direction, relHitPoint);

		if(currentState.direction.magnitude<aMax){
			currentState.direction = currentState.direction + (aMax * 0.02f)*relHitPoint.normalized;

		}else if (dotSdirPoint < 0.0f) {
			currentState.direction = currentState.direction - (aMax * 0.02f) * currentState.direction.normalized;
		} else  {
		

			float currAngle = Vector3.Angle (s.direction, relHitPoint);
			float distanceOnRelDir = dotSdirPoint / relHitPoint.magnitude;
			if (currAngle < 40f) {

				if (distanceOnRelDir < 0.1f) {

					currentState.direction = currentState.direction + (aMax * 0.02f) * relHitPoint.normalized;

				} else if ((distanceOnRelDir * distanceOnRelDir) / (2 * aMax) > halfPoint.magnitude) {  //brake
					currentState.direction = currentState.direction - (aMax * 0.02f) * currentState.direction.normalized;
				} else {
					currentState.direction = currentState.direction + (aMax * 0.02f)*relHitPoint.normalized;
				}
			} else{
				currentState.direction = currentState.direction + (aMax * 0.02f) * Vector3.Normalize (relHitPoint * distanceOnRelDir - currentState.direction);
			}

		} 

		return currentState;

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

//	void GetNextWaypoint() {
//		if (count >= waypoints.transform.childCount) {
//		} else {
//			target = waypoints.transform.GetChild (count);
//		}
//	}
	
}
