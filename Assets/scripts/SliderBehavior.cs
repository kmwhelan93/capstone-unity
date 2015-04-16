using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SliderBehavior : MonoBehaviour {
	// For num troops input
	public static SliderBehavior instance;
	
	public GameObject sliderObject;
	public Slider slider;
	public GameObject sliderConfirmButton;
	public UnityEngine.UI.Text sliderValue;

	// Use this for initialization
	void Start () {
		Debug.Log("start");
		instance = this;
		sliderValue.text = "";
		sliderObject.SetActive(false);
		sliderConfirmButton.SetActive(false);
	}
	
	public void disableSlider() {
		sliderObject.SetActive(false);
		sliderConfirmButton.SetActive(false);
		GenerateWorld.instance.message.text = "";
		sliderValue.text = "";
	}
	
	public void showSlider(int min, int max) {
		sliderObject.SetActive(true);
		sliderConfirmButton.SetActive(true);
		slider.minValue = min;
		slider.maxValue = max;
		slider.value = min;
		sliderValue.text = min + "";
	}
	
	public void displaySliderValue(float f) {
		// Not sure why this next line doesn't work - f always comes in as 0
		//sliderValue.text = (int)f + "";
		sliderValue.text = slider.value + "";
	}
	
	public bool sliderIsActive() {
		return slider.IsActive();
	}
	
	public void onCheckButtonClickAddTroops() {
		if (Globals.opState == OpState.AddTroops) {
			TroopsHandler.instance.addTroops (GenerateWorld.instance.lastBase, (int)slider.value);
		} else if (Globals.opState == OpState.MoveTroops) {
			TroopsHandler.instance.startMoveTroopsAction (GenerateWorld.instance.secondBase, (int)slider.value);
		} else if (Globals.opState == OpState.Attack) {
			EventManager.sliderConfirmed((int)slider.value);
			EventManager.sliderConfirmed = null;
		}
		sliderValue.text = "";
		sliderObject.SetActive(false);
		sliderConfirmButton.SetActive(false);
	}
}
