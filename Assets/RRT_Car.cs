using UnityEngine;
using System.Collections;
using System;

public interface CarModelInterface{
	CarState GetNextState (CarState s, Vector3 v);
	void FollowStates(Stack s);
	Vector3 StartPosition();
	float DistanceTo(CarState s, Vector3 point);
}

public class CarState /*: IComparable*/ {
	public Vector3 position;
	public Vector3 velocity;
	public Vector3 instruction;
	public Quaternion rotation;
	public CarState(Vector3 p, Vector3 v, Quaternion r){
		this.position = p;
		this.velocity = v;
		this.rotation = r;
	}

	public CarState parent;
	public bool collision;
	public float gScore;  //cost from start along best known path
	public float fScore;  //Estimated total cost from start to goal through this CarState. fScore = heuristic + gScore;
	
	public CarState(float x, float z){
		this.position = new Vector3(x, 0.1f, z);
	}
	
	public CarState(Vector3 position, Vector3 direction, Quaternion rotation, float fScore, float gScore) : this (position, direction, rotation){
		this.fScore = fScore;
		this.gScore = gScore;
		collision = false;
	}
	
	public CarState(Vector3 position, Vector3 direction, Quaternion rotation, float fScore, float gScore, Vector3 point) : this (position, direction, rotation, fScore, gScore)
	{
		this.instruction = point;
		collision = false;
	}
	
	public CarState Copy(){
		CarState retCarState = new CarState(this.position,this.velocity,this.rotation, fScore,gScore, instruction);
		retCarState.parent = parent;
		
		retCarState.collision = collision;
		return retCarState;
	}
}

public class RRT_Car : MonoBehaviour {
	public int iterations = 15000;
	
	public float minRangeX = 0f;
	public float maxRangeX = 100f;
	public float minRangeZ = 0f;
	public float maxRangeZ = 100f;
	
	public Transform goal;
	public CarModelInterface model;
	private ArrayList goalLines;
	public int bestfScore;
	
	// Use this for initialization
	public void SetModel (CarModelInterface inModel) {
		goalLines = new ArrayList ();
		bestfScore = iterations/ 20;

		model = inModel;
		
		ArrayList CarStates = new ArrayList ();
		
		Vector3 startPosition = model.StartPosition ();
		CarState initialCarState = new CarState (startPosition,new Vector3(0f,0f,0f), Quaternion.identity ,Vector3.Distance(startPosition, goal.position) ,0);
		
		CarState /*currentCarState,*/ newCarState;
		
		CarStates.Add (initialCarState);
		
		float closestDistance;
		CarState closestCarState;
		
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
			
			closestDistance =  model.DistanceTo(initialCarState, point);
			
			closestCarState = initialCarState;
			foreach(CarState s in CarStates){
				float tmpDist = model.DistanceTo(s, point);
				if(tmpDist<closestDistance){
					closestDistance = tmpDist;
					closestCarState = s;
				}
			}
			
			newCarState = model.GetNextState(closestCarState,point);
			
			newCarState.parent = closestCarState;

			float dist = Vector3.Distance(newCarState.position,goal.position);
			if(dist>1f && newCarState.collision){
				continue;
			}
			CarStates.Add (newCarState);
			
			//Debug.Log (dist);
			if(dist<2f){
				Stack CarStatesPath = new Stack();
				while(newCarState.parent!=null){
					CarStatesPath.Push(newCarState);
					goalLines.Add (new Vector3[2] {newCarState.parent.position,newCarState.position});
					newCarState = newCarState.parent;
				}
				CarStatesPath.Push (newCarState);
				
				model.FollowStates(CarStatesPath);
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
