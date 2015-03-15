using UnityEngine;
using System.Collections;

public class TouchBase : MonoBehaviour {

	public Base b;

	public void OnMouseDown() 
	{
		if (Globals.addState == AddState.Base) {
			Debug.Log ("base touched to add base");
			GenerateWorld.instance.message.text = "Adding base...";
			StartCoroutine ("createBase");
		} else if (Globals.addState == AddState.Portal) {
			Debug.Log ("base touched to add portal");
			if (!GenerateWorld.instance.secondClick) {
				GenerateWorld.instance.lastBase = b;
				GenerateWorld.instance.secondClick = true;
			}
		} else if (Globals.moveState == MoveState.Troops) {
			Debug.Log("base touched to move troops");
			if (!GenerateWorld.instance.secondClick) {
				print ("waiting for second click");
				GenerateWorld.instance.message.text = "How many?            Then click base to send them to.";
				GenerateWorld.instance.numTroopsInputObject.SetActive(true);
				GenerateWorld.instance.lastBase = b;
				GenerateWorld.instance.secondClick = true;
			} else {
				print ("got second click, moving troops");
				GenerateWorld.instance.secondClick = false;
				GenerateWorld.instance.numTroopsInputObject.SetActive(false);
				GenerateWorld.instance.message.text = "Moving units...";
				if (GenerateWorld.instance.lastBase.baseId != b.baseId) {
					TroopsHandler.instance.startMoveTroopsAction(b);
				} else {
					Debug.Log ("Sorry! Can't move units from a base to itself");
				}
			}
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
