using UnityEngine;
using System.Collections;

public class TouchBase : MonoBehaviour {

	public Base b;
	//public Base lastBase;
	//public enum Modes {addBase, addPortal};
	//public static Modes mode;
	//public bool mode;
	//public bool secondClick;
	//public GameObject world;

	public void OnMouseDown() 
	{
		//if (!world) {
		//	world = (GameObject)GenerateWorld.instance;
		//}
		//GenerateWorld script = (GenerateWorld)world.GetComponent("GenerateWorld");
		if (!GenerateWorld.instance.mode) {
			Debug.Log ("base touched to add base");
			StartCoroutine ("createBase");
		}
		else if (GenerateWorld.instance.mode) {
			Debug.Log ("base touched to add portal");
			if (!GenerateWorld.instance.secondClick) {
				print ("Waiting for second click");
				GenerateWorld.instance.lastBase = b;
				GenerateWorld.instance.secondClick = true;
			}
			else {
				print ("Got second click, adding portal");
				GenerateWorld.instance.secondClick = false;
				StartCoroutine ("createPortal");
			}
		}
	}
	
	IEnumerator createBase() 
	{
		WWW request = RequestService.makeRequest("world/bases/create", b);
		yield return request;
		GenerateWorld.instance.resetWorldView ();
	}

	IEnumerator createPortal()
	{
		print ("createPortal called");
		//if (!world) {
		//	world = (GameObject)GenerateWorld.instance;
		//}
		//GenerateWorld script = (GenerateWorld)world.GetComponent("GenerateWorld");
		WWWForm wwwform = new WWWForm ();
		wwwform.AddField ("username", b.username);
		wwwform.AddField ("baseId1", GenerateWorld.instance.lastBase.baseId);
		wwwform.AddField ("baseId2", b.baseId);
		// NOTE: Changed this to "new WWW" from "RequestService.makeRequest" to fix a 500 request failed error
		WWW request = new WWW ("localhost:8080/myapp/world/portals/create", wwwform);
		yield return request;
		print ("REQUEST: " + request.error);
		print ("REQUEST: " + request.url);
		GenerateWorld.instance.resetWorldView ();
	}

	public void switchMode(bool newMode) {
		//if (!world) {
		//	world = (GameObject)GenerateWorld.instance;
		//}
		//GenerateWorld script = (GenerateWorld)world.GetComponent("GenerateWorld");
		GenerateWorld.instance.mode = newMode;
		GenerateWorld.instance.secondClick = false;
	}
}
