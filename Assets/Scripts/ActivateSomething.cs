using UnityEngine;
using System.Collections;

public class ActivateSomething : MonoBehaviour {

	public GameObject discreteModel;
	public UnityEngine.UI.Slider slider;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public void ActivateDiscrete(){
		if (!discreteModel.activeSelf) {
			discreteModel.SetActive (true);
			int hood = (int) slider.value;
			discreteModel.GetComponent<DiscreteController> ().Run (hood);
		}


	}
}
