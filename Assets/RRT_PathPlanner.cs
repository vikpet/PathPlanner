using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RRT_PathPlanner : MonoBehaviour {
	public GameObject MotionModel;
	public float range;
	public Transform goal;
	private DynamicCarController controller;
	// Use this for initialization
	void Start () {
		controller = MotionModel.GetComponent<DynamicCarController> ();
//		DynamicCarController.State ;

		State start_state = new State (controller.transform, Vector3.zero);
		State goal_state = new State (goal, Vector3.zero);

		StateTree random_tree = new StateTree ();

		float x = Random.Range (0f, range);
		float z = Random.Range (0f, range);
		Vector3 random_instruction = new Vector3 (x, 0f, z);

		/*
		G.init(qinit)
		for k = 1 to K
			qrand ← RAND_CONF()
				qnear ← NEAREST_VERTEX(qrand, G)
				qnew ← NEW_CONF(qnear, qrand, Δq)
				G.add_vertex(qnew)
				G.add_edge(qnear, qnew)
				return G
				*/
	}
	
	// Update is called once per frame
	void Update () {
	
	}



	public class StateTree {
		Node root;
		private List<Node> allNodes; // not memory efficient;

		/**
		 *	Performs a random instruction and adds the state to the tree; 
		 */
		public State AddRandomState () {
			return null;
		}

		public Node GetRandomNode() {
			return allNodes [Random.Range (0, allNodes.Count)];
		}

		public void AddState(State state, Node parent, Transform instruction)  {

			Node new_node = new Node (state, parent);
			new_node.parent = parent;
			parent.children.Add (new_node);

			new_node.instruction = instruction;

			if (root == null) {
				root = new_node;
				allNodes = new List<Node>();
			}
			allNodes.Add (new_node);
		}

		public class Node {

			public State state;
			public Transform instruction;
			public Node parent;
			public List<Node> children;

			public Node(State state, Node parent) {
				this.state = state;
				this.parent = parent;
				children = new List<Node>();
			}
		}
	}
}
