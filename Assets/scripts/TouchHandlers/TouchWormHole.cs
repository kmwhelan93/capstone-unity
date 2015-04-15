using UnityEngine;
using System.Collections;

public class TouchWormHole : MonoBehaviour {
	public void OnMouseDown() {
		if (Globals.opState == OpState.Attack) {
			WormHole w = (WormHole)gameObject.GetComponent<InstanceObjectScript>().instanceObject;
			Base attachedBase = w.b;
			GenerateWorld.instance.message.text = "Touch checkmark to confirm amount";
			GenerateWorld.instance.sliderObject.SetActive(true);
			GenerateWorld.instance.sliderConfirmButton.SetActive(true);
			GenerateWorld.instance.slider.minValue = 0;
			GenerateWorld.instance.slider.maxValue = attachedBase.units;
			GenerateWorld.instance.slider.value = 0;
			GenerateWorld.instance.sliderValue.text = "0";
			EventManager.sliderConfirmed += gameObject.GetComponent<WormHoleScript>().onAttackConfirmed;
		}
	}
}
