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
		} else if (Globals.opState == OpState.AddPortal && !GenerateWorld.instance.secondClick) {
			Debug.Log ("base touched to add portal");
			// Create temp portal - red
			PortalHandler.instance.createDragPortal (b);
			GenerateWorld.instance.lastBase = b;
			GenerateWorld.instance.secondClick = true;
			// Gray out invalid bases
			PortalHandler.instance.grayOutInvalidBases (b.baseId);
		} else if (Globals.opState == OpState.MoveTroops) {
			Debug.Log ("base touched to move troops");
			if (!GenerateWorld.instance.secondClick) {
				print ("waiting for second click");
				GenerateWorld.instance.message.text = "How many?            Then click base to send them to.";
				GenerateWorld.instance.numTroopsInputObject.SetActive (true);
				GenerateWorld.instance.lastBase = b;
				GenerateWorld.instance.secondClick = true;
			} else {
				print ("got second click, moving troops");
				GenerateWorld.instance.secondClick = false;
				GenerateWorld.instance.numTroopsInputObject.SetActive (false);
				GenerateWorld.instance.message.text = "Moving units...";
				if (GenerateWorld.instance.lastBase.baseId != b.baseId) {
					TroopsHandler.instance.startMoveTroopsAction (b);
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
