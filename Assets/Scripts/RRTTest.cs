using UnityEngine;
using System.Collections;
using System;

public interface ModelInterface{
	State GetNextState (State s, Vector3 v);
	void FollowStates(Stack s);
	Vector3 StartPosition();
	float DistanceTo(State s, Vector3 point);
}

public class State /*: IComparable*/ {
	public Vector3 position;
	public Vector3 direction;
	public Vector3 point;
	public State parent;
	public bool collision;
	public float gScore;  //cost from start along best known path
	public float fScore;  //Estimated total cost from start to goal through this state. fScore = heuristic + gScore;


	public State(float x, float z){
		position = new Vector3(x, 0.5f, z);
	}

	public State(Vector3 position, Vector3 direction, float fScore, float gScore){
		this.position = position;
		this.direction = direction;
		this.fScore = fScore;
		this.gScore = gScore;
		collision = false;

	}

	public State(Vector3 position, Vector3 direction, float fScore, float gScore, Vector3 point){
		this.position = position;
		this.direction = direction;
		this.fScore = fScore;
		this.gScore = gScore;
		this.point = point;
		collision = false;
	}

	public State Copy(){
		State retState = new State(position,direction,fScore,gScore, point);
		retState.parent = parent;
	
		retState.collision = collision;
		return retState;



	}

	
//	public int CompareTo( System.Object other )  {
//
//		State oState = (State)other;
//
//
//		if (other == null) {
//			return 1;
//		}
//
//		if (Mathf.Approximately (fScore, oState.fScore)) {
//			if(Mathf.Approximately (position.x, oState.position.x) && Mathf.Approximately (position.z, oState.position.z)){
//				return 0;
//			}else{
//				return 1;
//			}
//
//		} else {
//			if((fScore-oState.fScore) > 0.0f){
//				return 1;
//			}else{
//				return -1;
//			}
//		}
//	}
}



public class RRTTest : MonoBehaviour {

	public Transform goal;
	public ModelInterface model;
	private ArrayList goalLines;

	// Use this for initialization
	public void SetModel (ModelInterface inModel) {
		goalLines = new ArrayList ();
		//model = GetComponent<HoverMotor> ();
		model = inModel;
		//SortedList orderedStates = new SortedList ();
		ArrayList states = new ArrayList ();

		//Vector3 startPosition = new Vector3 (33f,0.5f,70f);
		Vector3 startPosition = model.StartPosition ();
		State initialState = new State (startPosition,new Vector3(0f,0f,0f), Vector3.Distance(startPosition, goal.position) ,0);

		State /*currentState,*/ newState;

		//orderedStates.Add (initialState,null);

		states.Add (initialState);

		float closestDistance;
		State closestState;

		int count = 0;
		int bestfScore = 5;



		for(int i = 0; i < 1000; i++){


			//currentState = (State)orderedStates.GetKey(0);
			//orderedStates.Remove(currentState);
			Vector3 point;
			if(count < bestfScore){
				count++;
				point = new Vector3(UnityEngine.Random.Range(0f,100f),0.5f,UnityEngine.Random.Range(0f,100f));
			}else{
				point = goal.position;
				count = 0;
			}

			//newState = model.GetNextState(currentState,point);
			//newState = model.GetNextState(currentState,goal.position);
			//closestDistance = Vector3.Distance(initialState.position, point);
			closestDistance =  model.DistanceTo(initialState, point);

			closestState = initialState;
			foreach(State s in states){
				//float tmpDist = Vector3.Distance(s.position,point);
				float tmpDist = model.DistanceTo(s, point);
				if(tmpDist<closestDistance){
					closestDistance = tmpDist;
					closestState = s;
				}
			}

			newState = model.GetNextState(closestState,point);

			newState.parent = closestState;

			//newState.direction = point;  //WHy is this here?
			float dist = model.DistanceTo(newState,goal.position);
			if(dist>1f && newState.collision){
				continue;
			}
			states.Add (newState);

			//Debug.Log (dist);
			if(dist<0.5f){
				Stack statesPath = new Stack();
				while(newState.parent!=null){
					statesPath.Push(newState);
					goalLines.Add (new Vector3[2] {newState.parent.position,newState.position});
					newState = newState.parent;
				}
				statesPath.Push (newState);

				model.FollowStates(statesPath);
				break;
			}

//			if(bestfScore>Mathf.RoundToInt(newState.fScore)){
//				bestfScore = Mathf.RoundToInt(newState.fScore);
//				Debug.Log(bestfScore);
//			}

		}





		/**
		 * 
		 * State initialState = new State(startX, StartZ);
		 * OrderedState.add(initialState);
		 * State currentState;
		 * Satet newState;
		 * for(int i = 1; i < 1000; i++){
		 * 		currentState = OrderedStates.GetKey (0);
		 * 		currentState.gScore++;
		 *		OrderedStates.RemoveAt (0);
		 *		Vector3 point = RandomPOint();
		 *		newState = model.getNextState(currentState, point);
		 *		newState.parent = currentState;
		 *		OrderedState.add(newState);
		 *		OrderedState.add(currentState);
		 * }
		 * 
		 */



	}
	void OnDrawGizmos() {
		Gizmos.color = Color.red;
		if(goalLines == null){
			return;
		}
		foreach(Vector3[] line in goalLines){
			Gizmos.DrawLine(line[0],line[1]);
		}
		
		
	}

	
	// Update is called once per frame
	void Update () {

	}
}
