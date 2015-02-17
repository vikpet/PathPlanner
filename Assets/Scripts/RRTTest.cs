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
}

public class RRTTest : MonoBehaviour {
	public int iterations = 15000;

	public float minRangeX = 0f;
	public float maxRangeX = 100f;
	public float minRangeZ = 0f;
	public float maxRangeZ = 100f;
	
	public Transform goal;
	public ModelInterface model;
	private ArrayList goalLines;
	public int bestfScore;

	// Use this for initialization
	public void SetModel (ModelInterface inModel) {
		goalLines = new ArrayList ();

		model = inModel;

		ArrayList states = new ArrayList ();

		Vector3 startPosition = model.StartPosition ();
		State initialState = new State (startPosition,new Vector3(0f,0f,0f), Vector3.Distance(startPosition, goal.position) ,0);

		State /*currentState,*/ newState;

		states.Add (initialState);

		float closestDistance;
		State closestState;

		int count = 0;

		for(int i = 0; i < iterations; i++){
			Vector3 point;
			if(count < bestfScore){
				count++;
				point = new Vector3(UnityEngine.Random.Range(minRangeX,maxRangeX),0.5f,UnityEngine.Random.Range(minRangeZ,maxRangeZ));

			}else{
				point = goal.position;
				bestfScore--;
				minRangeX = minRangeX + (goal.position.x-minRangeX)/40;
				maxRangeX = maxRangeX - (maxRangeX-goal.position.x)/40;
				minRangeZ = minRangeZ + (goal.position.z-minRangeZ)/40;
				maxRangeZ = maxRangeZ - (maxRangeZ-goal.position.z)/40;

				Debug.Log(minRangeX + " " + maxRangeX + " " + minRangeZ + " " + maxRangeZ);
				count = 0;
			}

			closestDistance =  model.DistanceTo(initialState, point);

			closestState = initialState;
			foreach(State s in states){
				float tmpDist = model.DistanceTo(s, point);
				if(tmpDist<closestDistance){
					closestDistance = tmpDist;
					closestState = s;
				}
			}

			newState = model.GetNextState(closestState,point);

			newState.parent = closestState;

			float dist = Vector3.Distance(newState.position,goal.position);
			if(dist>1f && newState.collision){
				continue;
			}
			states.Add (newState);

			//Debug.Log (dist);
			if(dist<2f){
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
		}
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
}
