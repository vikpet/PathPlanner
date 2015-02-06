using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class A_StarPathPlanner : MonoBehaviour {

	//public static int hood = 4;

	private static int[][] fourHood = new int[4][] {new int[2]{-1, 0},new int[2]{1,0},new int[2]{0,-1},new int[2]{0,1}};
	private static int[][] eightHood = new int[8][] {new int[2]{-1, 0},new int[2]{1,0},new int[2]{0,-1},new int[2]{0,1},
		new int[2]{-1,-1},new int[2]{-1,1},new int[2]{1,-1},new int[2]{1,1}};
	private static int[][] sixteenHood = new int[16][] {new int[2]{-1, 0},new int[2]{1,0},new int[2]{0,-1},new int[2]{0,1},
		new int[2]{-1,-1},new int[2]{-1,1},new int[2]{1,-1},new int[2]{1,1},new int[2]{-2, 1},new int[2]{-1,2},
		new int[2]{1,2},new int[2]{2,1},new int[2]{2,-1},new int[2]{1,-2},new int[2]{-1,-2},new int[2]{-2,-1}};


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public struct Point{
		public int x, y;
		public Point(int x, int y){
			this.x = x;
			this.y = y;
		}

		public bool numEquals(Point other){
			return x == other.x && y == other.y;

		}

		public double distance(Point other){
			return Mathf.Sqrt (Mathf.Pow((x-other.x),2.0f)+Mathf.Pow((y-other.y),2.0f));
		}
	}



	public static LinkedList<Vector3> aStar(int[][] map, Point start, Point goal, int hood) {
		int[][] currentHood;
		if(hood == 4){
			currentHood = fourHood;
		}else if(hood == 8){
			currentHood = eightHood;
		}else if(hood == 16){
			currentHood = sixteenHood;
		}else{
			return null;
		}


		HashSet<Point> closedSet = new HashSet<Point>();
		HashSet<Point> openSet = new HashSet<Point>();
		
		Dictionary<Point, double> gScore = new Dictionary<Point, double>(); 
		Dictionary<Point, double> fScore = new Dictionary<Point, double>(); 
		
		openSet.Add(start);
		
		Dictionary<Point, Point> cameFrom = new Dictionary<Point, Point>(); 
		
		gScore.Add(start, 0.0d);
		fScore.Add(start, gScore[start] + start.distance(goal));
		double bestFScore;
		Point bestPoint = start;
		while(openSet.Count>=0) {
			//double bestFScore = fScore[openSet.First.Value];
			bestFScore = int.MaxValue;


			double tmpfScore;
			foreach(Point p in openSet){

				tmpfScore = fScore[p];
				if(tmpfScore < bestFScore) {
					bestPoint = p;
					bestFScore = tmpfScore;
				}

			}
			//Debug.Log("Best Now x:" + bestPoint.x + " y:" + bestPoint.y +" \n");
			openSet.Remove(bestPoint);
			if(bestPoint.numEquals(goal)) 
				return reconstructPath(cameFrom, goal);
			
			closedSet.Add(bestPoint);
			// Neighbours
			LinkedList<Point> neighbours = new LinkedList<Point>();

			Point tmpPoint;
			for(int i = 0; i < currentHood.Length; i++){
				tmpPoint = new Point(bestPoint.x + currentHood[i][0],
				                     bestPoint.y + currentHood[i][1]);

				if(!closedSet.Contains(tmpPoint) && tmpPoint.x >= 0 &&
				   tmpPoint.y >= 0 && tmpPoint.x < map.Length &&
				   tmpPoint.y < map[i].Length && map[tmpPoint.x][tmpPoint.y]==0){
					neighbours.AddLast (tmpPoint);
				}
			}

			//if(current.x > 0 && map[current.y][current.x - 1] == 0) {
			//	neighbours.add(new Point(current.x - 1, current.y));
			//}

			foreach(Point n in neighbours) {
				if(closedSet.Contains(n)) continue;
				double tentativeGScore = gScore[bestPoint] + bestPoint.distance(n);
				
				if(!openSet.Contains(n) || tentativeGScore < gScore[n]) {
					if(openSet.Contains(n)){
						cameFrom[n] = bestPoint;
						gScore[n] =  tentativeGScore;
						fScore[n] = gScore[n] + n.distance(goal);
					}else{
						cameFrom.Add(n, bestPoint);
						gScore.Add(n, tentativeGScore);
						fScore.Add(n, gScore[n] + n.distance(goal));
						openSet.Add(n);
					}
				}
			}
		}
		return null;
	}
	
	private static LinkedList<Vector3> reconstructPath(Dictionary<Point, Point> cameFrom, Point current) {
		LinkedList<Vector3> path = new LinkedList<Vector3>();
		path.AddFirst(new Vector3(current.y,0.5f,current.x)); // x och y är inverterade i kartan
		while(cameFrom.ContainsKey(current)) {
			current = cameFrom[current];
			path.AddFirst(new Vector3(current.y,0.5f,current.x));
		}
		return path;
	}
}
