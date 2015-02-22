﻿using UnityEngine;
using System.Collections;

public class TouchBase : MonoBehaviour {

	public Base b;

	public void OnMouseDown() 
	{
		if (Globals.addState == AddState.Base) {
			Debug.Log ("base touched to add base");
			StartCoroutine ("createBase");
		} else if (Globals.addState == AddState.Portal) {
			Debug.Log ("base touched to add portal");
			if (!GenerateWorld.instance.secondClick) {
				print ("Waiting for second click");
				GenerateWorld.instance.lastBase = b;
				GenerateWorld.instance.secondClick = true;
			} else {
				print ("Got second click, adding portal");
				GenerateWorld.instance.secondClick = false;
				if (GenerateWorld.instance.lastBase.baseId != b.baseId) {
					StartCoroutine ("createPortal");
				} else {
					Debug.Log ("Sorry! Can't add a portal from a base to itself");
				}
			}
		} else if (Globals.addState == AddState.Troops) {
			Debug.Log("base touched to move troops");
			if (!GenerateWorld.instance.secondClick) {
				print ("Waiting for second click");
				GenerateWorld.instance.lastBase = b;
				GenerateWorld.instance.secondClick = true;
			} else {
				print ("Got second click, moving troops");
				GenerateWorld.instance.secondClick = false;
				if (GenerateWorld.instance.lastBase.baseId != b.baseId) {
					StartCoroutine ("moveTroops");
				} else {
					Debug.Log ("Sorry! Can't move troops from a base to itself");
				}
			}
		}
	}
	
	IEnumerator createBase() 
	{
		WWW request = RequestService.makeRequest("world/bases/create", b);
		yield return request;
		Debug.Log (request.text);
		UpdateGold.instance.syncGold ();
		GenerateWorld.instance.resetWorldView ();
	}

	IEnumerator createPortal()
	{
		WWWForm wwwform = new WWWForm ();
		wwwform.AddField ("username", b.username);
		wwwform.AddField ("baseId1", GenerateWorld.instance.lastBase.baseId);
		wwwform.AddField ("baseId2", b.baseId);
		// NOTE: Changed this to "new WWW" from "RequestService.makeRequest" to fix a 500 request failed error
		WWW request = new WWW ("localhost:8080/myapp/world/portals/create", wwwform);
		yield return request;
		GenerateWorld.instance.resetWorldView ();
	}

	IEnumerator moveTroops() 
	{
		WWWForm wwwform = new WWWForm ();
		wwwform.AddField ("username", b.username);
		wwwform.AddField ("baseId1", GenerateWorld.instance.lastBase.baseId);
		wwwform.AddField ("baseId2", b.baseId);
		wwwform.AddField ("numTroops", 1);
		WWW request = new WWW ("localhost:8080/myapp/world/troops/move", wwwform);
		yield return request;
		GenerateWorld.instance.resetWorldView ();
	}
	
}
