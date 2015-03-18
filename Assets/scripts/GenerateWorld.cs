﻿using UnityEngine;
using System;
using LitJson;
using System.Collections;
using System.Collections.Generic;

public class GenerateWorld : MonoBehaviour {

	public static GenerateWorld instance;

	public GameObject basePrefab;
	public GameObject textPrefab;

	public Material[] materials;

	private GameObject[] currentBases;
	private GameObject[] currentDisplayText;
	public GameObject canvas;

	public Base lastBase;
	public bool secondClick;
	public UnityEngine.UI.Text message;
	public GameObject numTroopsInputObject;
	public UnityEngine.UI.InputField numTroopsInputField;
	
	void Awake() {
		instance = this;
	}
	
	// Use this for initialization
	void Start () {
		message.text = "";
		numTroopsInputObject.SetActive(false);
		Globals.portalBuildTimeInMins = 1;
		// load resources
		loadResources ();
		// Create bases and portals
		resetWorldView ();
		// Restart any troop transfers in progress
		//TroopsHandler.instance.restartMoveTroops ();
	}

	// Update is called once per frame
	void Update () {
		PortalHandler.instance.updateUnfinishedPortals ();
	}

	private void loadResources() {
		String[] materialNames = {
						"base_orange",
						"base_green",
						"base_blue",
						"base_yellow",
						"base_purple",
						"base_pink",
						"base_teal"
				};
		materials = new Material[materialNames.Length];
		for (int i = 0; i < materialNames.Length; i++) 
		{
			materials[i] = (Material) Resources.Load ("materials/"+ materialNames[i], typeof(Material));
		}
	}

	public void resetWorldView() {
		StartCoroutine (coResetWorldView());
	}

	private IEnumerator coResetWorldView() {
		WWWForm wwwform = new WWWForm ();
		wwwform.AddField ("username", "kmw8sf");
		WWW request = new WWW ("localhost:8080/myapp/world/bases", wwwform);
		yield return request;
		destroyCurrentBases ();
		PortalHandler.instance.destroyCurrentPortals ();
		Base[] bases = JsonMapper.ToObject<Base[]>(request.text);
		displayBases (bases);
		PortalHandler.instance.displayPortals ();
		yield break;
	}

	private void destroyCurrentBases() {
		if (currentBases == null)
			return;
		foreach (GameObject b in currentBases) {
			Destroy (b);
		}
		foreach (GameObject t in currentDisplayText) {
			Destroy (t);
		}
	}

	private bool displayBases(Base[] baseLocs) {
		// Place objects
		currentBases = new GameObject[baseLocs.Length];
		currentDisplayText = new GameObject[baseLocs.Length];
		List<BaseWrapper> baseWrappers = new List<BaseWrapper> ();
		for (int i = 0; i < baseLocs.Length; i++) {
			int x = baseLocs[i].world.x * 3 + baseLocs[i].local.x;
			int y = baseLocs[i].world.y * 3 + baseLocs[i].local.y;
			GameObject baseObj = (GameObject) Instantiate (basePrefab, new Vector3(x, y, .5f), basePrefab.transform.rotation);
			baseObj.GetComponent<Renderer>().material = materials[baseLocs[i].colorId % materials.Length];
			TouchBase tb = baseObj.GetComponent<TouchBase>();
			tb.b = baseLocs[i];
			baseObj.name = "Base" + baseLocs[i].baseId;
			currentBases[i] = baseObj;

			GameObject displayTextObj = (GameObject) Instantiate (textPrefab, new Vector3(0, 0, -1000), Quaternion.identity);
			currentDisplayText[i] = displayTextObj;
			displayTextObj.transform.SetParent (canvas.transform, false);
			baseWrappers.Add (new BaseWrapper(baseObj, displayTextObj));
		}
		Camera.main.GetComponent<DisplayInfoHandler> ().baseWrappers = baseWrappers;
		Camera.main.GetComponent<DisplayInfoHandler> ().positionText ();
		Camera.main.GetComponent<DisplayInfoHandler> ().updateContent ();
		return true;
	}

	public GameObject getBaseObj(String name) {
		foreach (GameObject b in currentBases) {
			if (b.name.Equals(name)) return b;
		}
		return null;
	}

	public GameObject[] getCurrentBases() {
		return currentBases;
	}

	public void clearBases() {
		StartCoroutine(coClearBases ());
	}

	private IEnumerator coClearBases() {
		WWW request = RequestService.makeRequest ("world/clear", currentBases [0].GetComponent<TouchBase>().b);
		yield return request;
		Debug.Log (request.text);
		UpdateGold.instance.syncGold ();
		GenerateWorld.instance.resetWorldView();
	}

}
