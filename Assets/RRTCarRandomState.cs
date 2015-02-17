using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class RRTCarRandomState : MonoBehaviour {
	public int iterations = 15000;
	
	public float minRangeX = 0f;
	public float maxRangeX = 100f;
	public float minRangeZ = 0f;
	public float maxRangeZ = 100f;
	
	public Transform goal;
	public CarModelInterface model;
	private ArrayList goalLines;
	private int bestfScore;

	private LinkedList<CarState> queue;

	// Use this for initialization
	public void SetModel (CarModelInterface inModel) {
		goalLines = new ArrayList ();
		bestfScore = iterations / 2;
		
		model = inModel;
		
		ArrayList CarStates = new ArrayList ();
		LinkedList<CarState> queue = new LinkedList<CarState> ();
		
		Vector3 startPosition = model.StartPosition ();
		CarState initialCarState = new CarState (startPosition,new Vector3(0f,0f,0f), Quaternion.identity ,Vector3.Distance(startPosition, goal.position) ,0);
		
		CarState /*currentCarState,*/ newCarState;
		
		CarStates.Add (initialCarState);
		
		//		float closestDistance;
		//		CarState closestCarState;
		
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
			CarState randomCarState;
			if(queue.Count < 1) {
				randomCarState = (CarState)CarStates[UnityEngine.Random.Range(0, CarStates.Count)];
			} else {
				randomCarState = queue.First.Value;
				queue.RemoveFirst();
			}
			newCarState = model.GetNextState(randomCarState,point);
			newCarState.parent = randomCarState;
			
			float dist = Vector3.Distance(newCarState.position,goal.position);
			if(dist>1f && newCarState.collision){
				continue;
			}
			CarStates.Add (newCarState);
			queue.AddFirst (newCarState);
			
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
