using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class A_StarPathPlanner : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	/*
	public static LinkedList<PointF> aStar(int[][] map, Point start, Point goal) {
		
		HashSet<PointF> closedSet = new HashSet<PointF>();
		LinkedList<PointF> openSet = new LinkedList<PointF>();
		
		Dictionary<Point, Double> gScore = new Dictionary<Point, Double>(); 
		Dictionary<Point, Double> fScore = new Dictionary<Point, Double>(); 
		
		openSet.add(start);
		
		Dictionary<Point, Point> cameFrom = new Dictionary<Point, Point>(); 
		
		gScore.put(start, 0.0);
		fScore.put(start, gScore.get(start) + start.distance(goal));
		
		while(!openSet.isEmpty()) {
			double bestFScore = fScore.get(openSet.peek());
			int i = 0;
			int bestIndex = 0;
			for(Point p : openSet) {
				if(fScore.get(p) < bestFScore) {
					bestIndex = i;
				}
				i++;
			}
			
			Point current = openSet.remove(bestIndex);
			if(current.equals(goal)) 
				return reconstructPath(cameFrom, goal);
			
			closedSet.add(current);
			// Neighbours
			LinkedList<Point> neighbours = new LinkedList<Point>();
			if(current.x > 0 && map[current.y][current.x - 1] == 0) {
				neighbours.add(new Point(current.x - 1, current.y));
			}
			if(current.y > 0 && map[current.y - 1][current.x] == 0) {
				neighbours.add(new Point(current.x, current.y - 1));
			}
			if(current.x < map[0].length - 1 && map[current.y][current.x + 1] == 0) {
				neighbours.add(new Point(current.x + 1, current.y));
			}
			if(current.y < map.length - 1 && map[current.y + 1][current.x] == 0) {
				neighbours.add(new Point(current.x, current.y + 1));
			}
			for(Point n : neighbours) {
				if(closedSet.contains(n)) continue;
				double tentativeGScore = gScore.get(current) + current.distance(n);
				
				if(!openSet.contains(n) || tentativeGScore < gScore.get(n)) {
					cameFrom.put(n, current);
					gScore.put(n, tentativeGScore);
					fScore.put(n, gScore.get(n) + n.distance(goal));
					if(!openSet.contains(n)) {
						openSet.add(n);
					}
				}
			}
		}
		return null;
	}
	
	private static LinkedList<Point> reconstructPath(HashMap<Point, Point> cameFrom, Point current) {
		LinkedList<Point> path = new LinkedList<Point>();
		path.add(current);
		while(cameFrom.containsKey(current)) {
			current = cameFrom.get(current);
			path.add(current);
		}
		return path;
	}*/
}
