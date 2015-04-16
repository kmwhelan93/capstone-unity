using UnityEngine;
using System.Collections;

public class TouchWormHole : MonoBehaviour {
	public void OnMouseDown() {
		if (Globals.opState == OpState.Attack) {
			WormHole w = (WormHole)gameObject.GetComponent<InstanceObjectScript>().instanceObject;
			Base attachedBase = w.b;
			GenerateWorld.instance.message.text = "Touch checkmark to confirm amount";
			SliderBehavior.instance.showSlider(0,attachedBase.units);
			EventManager.sliderConfirmed += gameObject.GetComponent<WormHoleScript>().onAttackConfirmed;
		}
	}
}
