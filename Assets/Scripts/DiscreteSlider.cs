using UnityEngine;
using System.Collections;

public class DiscreteSlider : MonoBehaviour {

	public UnityEngine.UI.Text hoodText;
	private UnityEngine.UI.Slider slider;

	void Start(){
		slider = GetComponent<UnityEngine.UI.Slider> ();
		hoodText.text = "Hood: " + slider.value;
	}

	public void HoodSelect(){

		if (slider.value <6) {
			slider.value = 4;
		}else if(slider.value < 12){
			slider.value = 8;
		}else{
			slider.value = 16;
		}
		hoodText.text = "Hood: " + slider.value;
	}


}
