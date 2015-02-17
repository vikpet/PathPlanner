using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UtilityFunctions : MonoBehaviour {

	public static void PlaceWaypoints(LinkedList<Vector3> waypointPositions, GameObject waypointParent, GameObject waypointPrefab) {
		foreach(Vector3 wpPos in waypointPositions) {
			GameObject clone;
			clone = GameObject.Instantiate(waypointPrefab,
		                               wpPos,
		                               waypointParent.transform.rotation) as GameObject;
			clone.transform.parent = waypointParent.transform;
		}
	}


	// From: http://wiki.unity3d.com/index.php/PolyContainsPoint
	public static bool ContainsPoint (Vector3[] polyPoints3 , Vector3 p3) { 

		Vector2 p = new Vector2(p3.x, p3.z);
		Vector2[] polyPoints = new Vector2[polyPoints3.Length];
		for (int i = 0; i < polyPoints3.Length; i++) {
			polyPoints[i] = new Vector2(polyPoints3[i].x, polyPoints3[i].z);
		}

		var j = polyPoints.Length-1; 
		var inside = false; 
		for (int i = 0; i < polyPoints.Length; j = i++) { 
			if ( ((polyPoints[i].y <= p.y && p.y < polyPoints[j].y) || (polyPoints[j].y <= p.y && p.y < polyPoints[i].y)) && 
			    (p.x < (polyPoints[j].x - polyPoints[i].x) * (p.y - polyPoints[i].y) / (polyPoints[j].y - polyPoints[i].y) + polyPoints[i].x)) 
				inside = !inside; 
		} 
		return inside; 
	}
}
