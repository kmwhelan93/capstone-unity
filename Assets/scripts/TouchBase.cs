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
				print ("waiting for second click");
				GenerateWorld.instance.message.text = "Now click which base to attach portal to";
				GenerateWorld.instance.lastBase = b;
				GenerateWorld.instance.secondClick = true;
			} else {
				print ("got second click, adding portal");
				GenerateWorld.instance.secondClick = false;
				if (GenerateWorld.instance.lastBase.baseId != b.baseId) {
					GenerateWorld.instance.message.text = "Adding portal...";
					StartCoroutine ("createPortal");
				} else {
					Debug.Log ("Sorry! Can't add a portal from a base to itself");
					GenerateWorld.instance.message.text = "Can't add a portal from a base to itself";
				}
			}
		} else if (Globals.addState == AddState.Troops) {
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
					StartCoroutine ("moveTroops");
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

	IEnumerator createPortal()
	{
		WWWForm wwwform = new WWWForm ();
		wwwform.AddField ("username", b.username);
		wwwform.AddField ("baseId1", GenerateWorld.instance.lastBase.baseId);
		wwwform.AddField ("baseId2", b.baseId);
		long finishTime = CurrentTime.currentTimeMillis() + (long)(Globals.portalBuildTimeInMins * 60000.0);
		wwwform.AddField ("timeFinished", finishTime + "");
		// NOTE: Changed this to "new WWW" from "RequestService.makeRequest" to fix a 500 request failed error
		WWW request = new WWW ("localhost:8080/myapp/world/portals/create", wwwform);
		yield return request;
		GenerateWorld.instance.message.text = request.text;
		GenerateWorld.instance.resetWorldView ();
	}

	IEnumerator moveTroops() 
	{
		WWWForm wwwform = new WWWForm ();
		wwwform.AddField ("username", b.username);
		wwwform.AddField ("baseId1", GenerateWorld.instance.lastBase.baseId);
		wwwform.AddField ("baseId2", b.baseId);
		wwwform.AddField ("numTroops", GenerateWorld.instance.numTroopsInputField.text);
		WWW request = new WWW ("localhost:8080/myapp/world/troops/move", wwwform);
		yield return request;
		GenerateWorld.instance.message.text = request.text;
		GenerateWorld.instance.resetWorldView ();
	}

}
