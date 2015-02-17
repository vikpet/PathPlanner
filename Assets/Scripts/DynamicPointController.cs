﻿using UnityEngine;
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

		//return relHit.magnitude;
		//float dist = (s.direction.magnitude*s.direction.magnitude)/(2*aMax);

		return relHit.magnitude + Vector3.Angle (relHit,s.direction)*0.01f;

		//return relHit.magnitude + Vector3.Dot (s.direction,relHit);

		//if(Vector3.Dot(s.direction,relHit)<0.0f){
		//	

		//	return Vector3.Distance(s.direction.normalized*dist,relHit);
		//}


//		if((s.position.x*s.position.x)/(2*aMax)>Mathf.Abs(s.position.x-point.x)/2){ //brake in x
//			if(s.position.z*s.position.z)/(2*aMax)>Mathf.Abs(s.position.z-point.z)/2){//brake in z
//
//			}
//
//		}


//		float distance = Vector3.Distance (s.position+s.direction, relHit/2);
//		relHit = point - (s.position+s.direction);
//	
//		return distance+Vector3.Distance (s.position+2*s.direction,relHit/2);

		//return Vector3.Distance (s.position,point);
	}

	public State GetNextState (State s, Vector3 point){

		State currentState = s.Copy();
		currentState.point = point;

		Ray ray;
		RaycastHit hit;

		for(int i = 0; i <50; i++){

			//decision
			currentState = MoveTowards(currentState,point);



			ray = new Ray (s.position, currentState.direction.normalized);
			if (Physics.Raycast (ray, out hit, currentState.direction.magnitude)) {
				//Vector3 incomingVec = hit.point - currentState.position;
				//Vector3 reflectVec = Vector3.Reflect(incomingVec, hit.normal);
				//currentState
				currentState.collision = true;
				break;
			}else{
				lines.Add (new Vector3[2] {currentState.position,currentState.position + currentState.direction/50});
				currentState.position = currentState.position + currentState.direction/50;
			}

		}
		return currentState;
//
//		Vector3 changeDir = relHitPoint - s.direction;
//
//		if (changeDir.magnitude > aMax) {
//			changeDir = changeDir.normalized/*aMax*/;
//		}
//		Vector3 dirVector = s.direction + changeDir;
//
//		Ray ray = new Ray (s.position, dirVector/dirVector.magnitude);
//		RaycastHit hit; 
//
//		Debug.Log ("original : " + s.direction + " new: " + dirVector + " change: " + Vector3.Distance (s.direction,dirVector));
//		//Vector3 pointDir = Vector3.Normalize (v-s.position);
//
//		float fScore;
//		float gScore;
//		bool collision = true;
//		Vector3 newPosition = s.position+s.direction;
//		Vector3 currentPosition = s.position;
//
//
//	
//		if (Physics.Raycast (ray, out hit, dirVector.magnitude)) {
//
//			lines.Add (new Vector3[2] {currentPosition,hit.point});
//			
//			//Vector3 incomingVec = hit.point - currentPosition;
//			//Vector3 reflectVec = Vector3.Reflect(incomingVec, hit.normal);
//			//reflectVec = (dirVector.magnitude-hit.distance)*reflectVec.normalized;
//			newPosition = hit.point+hit.normal;
//			dirVector = new Vector3(0f,0f,0f);
//			lines.Add (new Vector3[2] {hit.point,newPosition});
//			//fScore = gScore + Vector3.Distance(newPosition,goal.position);
//			//return new State(newPosition,hit.distance*pointDir, fScore, gScore);
//			//pointDir *=hit.distance;
//			//collision = true;
//			//currentPosition = hit.point;
//			//ray = new Ray (currentPosition, dirVector/dirVector.magnitude);
//			
//		} else {
//			//gScore = s.gScore++;
//			//fScore = gScore + Vector3.Distance(s.position+pointDir,goal.position);
//			newPosition = currentPosition+dirVector;
//			lines.Add (new Vector3[2] {currentPosition,newPosition});
//			collision =false;
//			
//		}
//		
//		State retState = new State(newPosition,dirVector,0.0f , 0.0f);
//		retState.collision = collision;
//		retState.accel = changeDir;
//		countText.text = ""+lines.Count;
//		return retState;


	}
	public void FollowStates(Stack s){

		states = s;
		initialState = (State)states.Pop ();
		prevDir = initialState.direction;
		//Debug.Log (prevDir+" "+prevDir.magnitude);
		prevP = initialState.position;
	}

	public State StartState(){
		return new State (transform.position,new Vector3(0f,0f,0f), Vector3.Distance(transform.position, localgoal.position) ,0);
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


			//rigidbody.AddForce(initialState.direction,ForceMode.Acceleration);

			//initialState.position = initialState.position+initialState.direction*Time.deltaTime;
			//transform.position = initialState.position;
			//rigidbody.AddForce(initialState.direction,ForceMode.VelocityChange);

			rigidbody.velocity = initialState.direction;

			count++;
			if(count>=50){
				states.Pop ();
				//Debug.Log ((prevDir-initialState.direction) + " " + (prevDir-initialState.direction).magnitude);
				prevDir = initialState.direction;
				//Debug.Log ((prevP-initialState.position).magnitude);
				//Debug.Log (initialState.direction.magnitude);
				//Debug.Log(states.Count);
				count = 0;
				RaycastHit hit;
				ray = new Ray(transform.position, (localgoal.position-transform.position).normalized);
				if (!Physics.Raycast (ray, out hit, (localgoal.position-transform.position).magnitude)) {
					states.Clear();
					count = 0;
					
				}

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
//			initialState.position = initialState.position+initialState.direction*Time.deltaTime;
//			transform.position = initialState.position;
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

					//currentState.direction = currentState.direction + (aMax * 0.02f) * Vector3.Normalize (//accel
					//	(((distanceOnRelDir * distanceOnRelDir) / (2 * aMax)) * relHitPoint.normalized) - currentState.direction);

					currentState.direction = currentState.direction + (aMax * 0.02f)*relHitPoint.normalized;
				}




			} else{



				currentState.direction = currentState.direction + (aMax * 0.02f) * Vector3.Normalize (relHitPoint * distanceOnRelDir - currentState.direction);

			}


//			if(Mathf.Sign(s.direction.x)*(s.direction.x*s.direction.x)/(2*aMax)>halfPoint.x){ //brake in x
//				if(Mathf.Sign(s.direction.z)*(s.direction.z*s.direction.z)/(2*aMax)>halfPoint.z){//brake in z
//
//					currentState.direction = currentState.direction - ((aMax*0.02f))*new Vector3(
//						currentState.direction.x/Mathf.Abs (currentState.direction.x-relHitPoint.x)
//						,0f,
//						currentState.direction.z/Mathf.Abs (currentState.direction.z-relHitPoint.z)).normalized;
//
//				}else{
//					currentState.direction.x = currentState.direction.x - (aMax*0.02f);
//				}
//	
//			}else if(Mathf.Sign(s.direction.z)*(s.position.z*s.position.z)/(2*aMax)>halfPoint.z){//brake in z
//
//			
//				currentState.direction.z = currentState.direction.z - (aMax*0.02f);
//
//			}else{
//
//				currentState.direction = currentState.direction + ((aMax*0.02f))*relHitPoint.normalized;
//			}


		} 
	



//		if(Vector3.Distance(currentState.direction,halfPoint)<aMax*0.02){
//			if(Vector3.Distance(relHitPoint,currentState.direction)<aMax*0.02){ //go for goal
//
//				currentState.direction = relHitPoint;
//			}else{ // go for mid
//				currentState.direction = halfPoint;
//			}
//		}else{ // go in direction of mid
//
//			currentState.direction = currentState.direction + (aMax*0.02f)*Vector3.Normalize(halfPoint - currentState.direction);
//		}


		return currentState;


//		Vector3 direction = tarPos - transform.position; // Calculate the direction the target is in.
//		direction *= 1.0f;
//		Vector3 velocity = rigidbody.velocity;
//		Vector3 velocityChange = (direction - velocity);
//
//		velocityChange.y = 0;
//		if (velocityChange.magnitude > aMax) {
//			velocityChange = velocityChange.normalized * aMax;
//		}
//		rigidbody.AddForce(velocityChange, ForceMode.Acceleration);
//
//		if ((transform.position - target.transform.position).sqrMagnitude <= minDistance * minDistance)
//		{
//			count++;
//			//GetNextWaypoint();
//		}
	}

//	void OnTriggerEnter(Collider other) {
//		if (other.gameObject.tag == "PickUp") {
//			other.gameObject.SetActive (false);
//			count++;
//			GetNextWaypoint();
//		}
//		if (other.gameObject.tag == "Obstacle") {
//			Debug.Log(" --- Collision! --- ");
//		}
//	}

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
