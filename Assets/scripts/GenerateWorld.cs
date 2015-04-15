using UnityEngine;
using System;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GenerateWorld : MonoBehaviour {
	
	public static GenerateWorld instance;
	
	public GameObject basePrefab;
	public GameObject textPrefab;
	public GameObject objectInfoPanelPrefab;
	public GameObject OIPItemPrefab;
	public Sprite prodSprite;
	public Sprite unitSprite;
	public GameObject ProgressBarPrefab;
	public Sprite addUnitSprite;
	public Sprite moveTroopSprite;
	
	public Material[] materials;
	
	private List<GameObject> currentBases;
	public GameObject canvas;
	
	// TODO: investigate what this is used for
	public Base lastBase;
	// TODO (cem6at): do this a better way
	public Base secondBase;
	
	public UnityEngine.UI.Text message;
	public GameObject numTroopsInputObject;
	public GameObject sliderObject;
	public Slider slider; // for num troops input
	public GameObject sliderConfirmButton;
	public UnityEngine.UI.Text sliderValue;
	
	void Awake() {
		instance = this;
	}
	
	// Use this for initialization
	void Start () {
		message.text = "";
		sliderValue.text = "";
		sliderObject.SetActive(false);
		sliderConfirmButton.SetActive(false);
		Globals.portalBuildTimeInMins = 1;
		// load resources
		loadResources ();
		// Create bases and portals
		resetWorldView ();
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
		WormHoleHandler.instance.loadWormHoles ();
		
		yield break;
	}
	
	private void destroyCurrentBases() {
		if (currentBases == null)
			return;
		foreach (GameObject b in currentBases) {
			Destroy(b.GetComponent<BaseScript>().objectInfoPanel);
			// TODO: destroy progress bars also?
			Destroy (b);
		}
	}
	
	private bool displayBases(Base[] baseLocs) {
		// Place objects
		currentBases = new List<GameObject>();
		for (int i = 0; i < baseLocs.Length; i++) {
			addBase (baseLocs[i]);
		}
		EventManager.positionText ();
		if (TroopsHandler.instance.addTroopsActions.Count == 0) {
			Debug.Log ("restarting adding troops");
			TroopsHandler.instance.restartAddTroops();		
		}
		return true;
	}
	
	public void addBase(Base b) {
		Vector3 loc = b.convertBaseCoordsToWorld();
		// adjust for base prefab
		loc = loc + new Vector3(0, 0, .5f);
		GameObject baseObj = (GameObject) Instantiate (basePrefab, loc, basePrefab.transform.rotation);
		baseObj.GetComponent<Renderer>().material = materials[b.colorId % materials.Length];
		TouchBase tb = baseObj.GetComponent<TouchBase>();
		tb.b = b;
		baseObj.name = "Base" + b.baseId;
		ObjectInstanceDictionary.registerGameObject(baseObj.name, baseObj);
		baseObj.GetComponent<InstanceObjectScript>().instanceObject = b;
		b.gameObject = baseObj;
		currentBases.Add(baseObj);
		
		GameObject objectInfoPanel = (GameObject) Instantiate (objectInfoPanelPrefab, new Vector3(0, 0, 0), Quaternion.identity);
		objectInfoPanel.transform.SetParent(canvas.transform, false);
		// TODO: check if I need the following line
		objectInfoPanel.GetComponent<ObjectInfoPanelScript>().o = baseObj;
		baseObj.GetComponent<BaseScript>().objectInfoPanel = objectInfoPanel;
		
		GameObject prodText = (GameObject) Instantiate (OIPItemPrefab, new Vector3(0, 0, -1000), Quaternion.identity);
		prodText.transform.SetParent (objectInfoPanel.transform, false);
		prodText.GetComponentInChildren<Image>().sprite = prodSprite;
		tb.b.updateProdRateEvent += prodText.GetComponent<OIPItemScript>().updateContent;
		
		
		GameObject unitsText = (GameObject) Instantiate (OIPItemPrefab, new Vector3(0, 0, 0), Quaternion.identity);
		unitsText.transform.SetParent(objectInfoPanel.transform, false);
		unitsText.GetComponentInChildren<Image>().sprite = unitSprite;
		tb.b.updateUnitsEvent += unitsText.GetComponent<OIPItemScript>().updateContent;
		
		// create progress bar ui
		// There is always one progress bar per base and it just hides and shows
		GameObject progressBar = (GameObject)Instantiate (ProgressBarPrefab, new Vector3 (0, 0, 0), Quaternion.identity);
		progressBar.transform.SetParent (objectInfoPanel.transform);
		progressBar.GetComponentInChildren<Image> ().sprite = addUnitSprite;
		progressBar.SetActive(false);
		tb.b.updateAddUnitProgress += progressBar.GetComponent<OIPProgressScript> ().updateContent;
	}
	
	public GameObject getBaseObj(String name) {
		foreach (GameObject b in currentBases) {
			if (b.name.Equals(name)) return b;
		}
		return null;
	}
	
	public List<GameObject> getCurrentBases() {
		return currentBases;
	}
	
	public void clearBases() {
		StartCoroutine(coClearBases ());
	}
	
	private IEnumerator coClearBases() {
		//WWW request = RequestService.makeRequest ("world/clear", currentBases [0].GetComponent<TouchBase>().b);
		WWW request = RequestService.makeRequest ("world/clear", ObjectInstanceDictionary.getObjectInstanceById("Base", 1));
		yield return request;
		ObjectInstanceDictionary.clearDictionary();
		UpdateGold.instance.syncGold ();
		GenerateWorld.instance.resetWorldView();
	}
	
}
