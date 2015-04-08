using UnityEngine;
using System.Collections;

public class TouchBase : MonoBehaviour {

	public Base b;

	public void OnMouseDown() 
	{
		if (Globals.opState == OpState.AddBase) {
			Debug.Log ("base touched to add base");
			GenerateWorld.instance.message.text = "Adding base...";
			StartCoroutine ("createBase");
		} else if (Globals.opState == OpState.AddPortal && !Globals.secondClick) {
			Debug.Log ("base touched to add portal");
			// Create temp portal - red
			PortalHandler.instance.createDragPortal (b);
			GenerateWorld.instance.lastBase = b;
			Globals.secondClick = true;
			// Gray out invalid bases
			PortalHandler.instance.grayOutInvalidBases (b.baseId);
		} else if (Globals.opState == OpState.MoveTroops) {
			Debug.Log ("base touched to move troops");
			GenerateWorld.instance.message.text = "Select bases";
			if (!Globals.secondClick) {
				print ("waiting for second click");
				GenerateWorld.instance.lastBase = b;
				Globals.secondClick = true;
			} else {
				print ("got second click, moving troops");
				Globals.secondClick = false;
				if (GenerateWorld.instance.lastBase.baseId != b.baseId) {
					GenerateWorld.instance.message.text = "How many?";
					GenerateWorld.instance.sliderObject.SetActive(true);
					GenerateWorld.instance.sliderConfirmButton.SetActive(true);
					GenerateWorld.instance.slider.minValue = 0;
					GenerateWorld.instance.slider.maxValue = GenerateWorld.instance.lastBase.units;
					GenerateWorld.instance.slider.value = 0;
					GenerateWorld.instance.sliderValue.text = "0";
					GenerateWorld.instance.secondBase = b;
				} else {
					Debug.Log ("Sorry! Can't move units from a base to itself");
				}
			}
		} else if (Globals.opState == OpState.ZoomBase) {
			Camera.main.GetComponent<LocalView> ().switchToLocalView (gameObject);
		} else if (Globals.opState == OpState.AddTroops) {
			GenerateWorld.instance.message.text = "Waiting for check button click";
			GenerateWorld.instance.lastBase = b;
			GenerateWorld.instance.sliderObject.SetActive(true);
			GenerateWorld.instance.sliderConfirmButton.SetActive(true);
			GenerateWorld.instance.slider.minValue = 0;
			GenerateWorld.instance.slider.maxValue = 100;
			GenerateWorld.instance.slider.value = 0;
			GenerateWorld.instance.sliderValue.text = "0";
		}
	}
	
	IEnumerator createBase() 
	{
		WWW request = RequestService.makeRequest("world/bases/create", b);
		yield return request;
		Debug.Log (request.text);
		GenerateWorld.instance.message.text = request.text;
		UpdateGold.instance.syncGold ();
		GenerateWorld.instance.resetWorldView ();
	}

}
