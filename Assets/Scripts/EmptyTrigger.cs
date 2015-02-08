using UnityEngine;
using System.Collections;

public class EmptyTrigger : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Player") {
			renderer.material.SetColor ("_TintColor", new Color(0.1f,0.1f,0.1f,0.9f));
		}
	}
}
