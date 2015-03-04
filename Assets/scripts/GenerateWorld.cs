﻿using UnityEngine;
using System;
using LitJson;
using System.Collections;
using System.Collections.Generic;

public class GenerateWorld : MonoBehaviour {

	public static GenerateWorld instance;

	public GameObject basePrefab;
	public GameObject textPrefab;
	public GameObject portalPrefab;

	public Material[] materials;
	public Material portalMaterial;
	public Material portalUnfinishedMaterial;

	private GameObject[] currentBases;
	private GameObject[] currentDisplayText;
	private GameObject[] currentPortals;
	private List<Portal2> currentUnfinishedPortals;
	public GameObject canvas;

	public Base lastBase;
	public bool secondClick;
	public UnityEngine.UI.Text message;
	public GameObject numTroopsInputObject;
	public UnityEngine.UI.InputField numTroopsInputField;

	void Awake()
	{
		instance = this;
	}
	
	// Use this for initialization
	void Start () {
		message.text = "";
		numTroopsInputObject.SetActive(false);
		Globals.portalBuildTimeInMins = 1;
		// load resources
		loadResources ();
		// Create bases
		resetWorldView ();
	}

	void loadResources() 
	{
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
		portalMaterial = (Material) Resources.Load ("materials/PortalPlasma", typeof(Material));
		portalUnfinishedMaterial = (Material)Resources.Load ("materials/PortalLava", typeof(Material));
	}

	// Update is called once per frame
	void Update () {
		if (currentUnfinishedPortals != null) {
			if (currentUnfinishedPortals.Count != 0) {
				syncPortals ();
			}
		}
	}

	public void resetWorldView()
	{
		StartCoroutine (coResetWorldView());
	}

	private void destroyCurrentBases()
	{
		if (currentBases == null)
			return;
		foreach (GameObject b in currentBases) {
			Destroy (b);
		}
		foreach (GameObject t in currentDisplayText) {
			Destroy (t);
		}
	}

	private void destroyCurrentPortals()
	{
		if (currentPortals != null) {
			foreach (GameObject p in currentPortals) {
				Destroy (p);
			}
		}
		currentUnfinishedPortals = null;
		return;
	}

	private IEnumerator coResetWorldView() {
		WWWForm wwwform = new WWWForm ();
		wwwform.AddField ("username", "kmw8sf");
		WWW request = new WWW ("localhost:8080/myapp/world/bases", wwwform);
		yield return request;
		destroyCurrentBases ();
		destroyCurrentPortals ();
		Base[] bases = JsonMapper.ToObject<Base[]>(request.text);
		displayBases (bases);
		displayPortals ();
		//displayUnfinishedPortals ();
		yield break;
	}

	// Called in Start() to load player's bases and place them on the screen
	private bool displayBases(Base[] baseLocs) {
		// Place objects
		currentBases = new GameObject[baseLocs.Length];
		currentDisplayText = new GameObject[baseLocs.Length];
		List<BaseWrapper> baseWrappers = new List<BaseWrapper> ();
		for (int i = 0; i < baseLocs.Length; i++) {
			int x = baseLocs[i].world.x * 3 + baseLocs[i].local.x;
			int y = baseLocs[i].world.y * 3 + baseLocs[i].local.y;
			GameObject baseObj = (GameObject) Instantiate (basePrefab, new Vector3(x, y, 0), Quaternion.identity);
			baseObj.renderer.material = materials[baseLocs[i].colorId % materials.Length];
			TouchBase tb = baseObj.GetComponent<TouchBase>();
			tb.b = baseLocs[i];
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

	// To display finished portals
	private void displayPortals() {
		StartCoroutine(createPortals ());
	}
	
	private IEnumerator createPortals() {
		WWWForm wwwform = new WWWForm ();
		wwwform.AddField ("username", "kmw8sf");
		// NOTE: Changed this to "new WWW" from "RequestService.makeRequest" to fix a 500 request failed error
		WWW request = new WWW ("localhost:8080/myapp/world/portals", wwwform);
		yield return request;
		Portal2[] portals = JsonMapper.ToObject<Portal2[]> (request.text);

		currentPortals = new GameObject[portals.Length];
		currentUnfinishedPortals = new List<Portal2>();
		for (int i = 0; i < portals.Length; i++) {
			// Locations of the two bases
			Portal2 portal = portals[i];
			int x1 = portal.base1.world.x * 3 + portal.base1.local.x;
			int y1 = portal.base1.world.y * 3 + portal.base1.local.y;
			int x2 = portal.base2.world.x * 3 + portal.base2.local.x;
			int y2 = portal.base2.world.y * 3 + portal.base2.local.y;

			// Create the portal (cylinder prefab)
			GameObject portalObj = (GameObject) Instantiate (portalPrefab, new Vector3((x1+x2)/2.0f, (y1+y2)/2.0f, 0f), Quaternion.identity);
			// Scale portal based on distance between bases
			Vector3 scale = portalObj.transform.localScale;
			scale.y = Vector2.Distance(new Vector2(x1,y1), new Vector2(x2,y2)) / 2.0f;
			portalObj.transform.localScale = scale;
			// Rotate portal based on angle between bases
			float slope = (y2*1.0f-y1)/(x2*1.0f-x1);
			float angle = Mathf.Rad2Deg*Mathf.Atan(slope) + 90;
			Vector3 rotate = portalObj.transform.eulerAngles;
			rotate.z = angle;
			portalObj.transform.eulerAngles = rotate;
			portalObj.name = "Portal" + portal.portalId;
			if (portal.timeFinished <= CurrentTime.currentTimeMillis()) {
				// Portal finished
				portalObj.renderer.material = portalMaterial;
			}
			else {
				// Portal unfinished
				portalObj.renderer.material = portalUnfinishedMaterial;
				currentUnfinishedPortals.Add(portal);
			}
			currentPortals[i] = portalObj;
		}

	}

	public void syncPortals() {
		List<Portal2> toRemove = new List<Portal2> ();
		foreach (Portal2 p in currentUnfinishedPortals) {
			// Finished yet?
			int percentFinished = getPortalPercentFinished(p);
			if (percentFinished == 100) {
				// Finish portal
				GameObject pObj = GameObject.Find("Portal" + p.portalId);
				pObj.renderer.material = portalMaterial;
				toRemove.Add(p);
				GenerateWorld.instance.message.text = "Portal finished!";
			}
			else {
				// Update portal completion percent
				GenerateWorld.instance.message.text = "Portal now " + percentFinished + "% finished";
			}
		}
		foreach (Portal2 p in toRemove) {
			currentUnfinishedPortals.Remove(p);
		}
	}

	// true if portal finished, false if unfinished
	public int getPortalPercentFinished(Portal2 p) {
		long buildTimeMilli = (long)(Globals.portalBuildTimeInMins * 60000.0);
		long curTime = CurrentTime.currentTimeMillis ();
		if (p.timeFinished <= curTime) {
			// Portal finished
			return 100;
		}
		long startTime = p.timeFinished - buildTimeMilli;
		return (int)(((curTime - startTime) / (buildTimeMilli*1.0))*100);
	}

	public void clearBases()
	{
		StartCoroutine(coClearBases ());
	}

	private IEnumerator coClearBases() 
	{
		WWW request = RequestService.makeRequest ("world/clear", currentBases [0].GetComponent<TouchBase>().b);
		yield return request;
		Debug.Log (request.text);
		UpdateGold.instance.syncGold ();
		GenerateWorld.instance.resetWorldView();
	}


}
