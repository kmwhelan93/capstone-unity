using UnityEngine;
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
	private GameObject[] currentBases;
	private GameObject[] currentDisplayText;
	public GameObject canvas;
	private GameObject[] currentPortals;
	public UnityEngine.UI.Text changeModeButtonText;

	public bool mode;
	public Base lastBase;
	public bool secondClick;

	void Awake()
	{
		instance = this;
	}
	
	// Use this for initialization
	void Start () {
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
			materials[i] = (Material) Resources.Load (materialNames[i], typeof(Material));
		}
		portalMaterial = (Material) Resources.Load ("PortalPlasma", typeof(Material));
	}

	// Update is called once per frame
	void Update () {
		
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
		if (currentPortals == null)
			return;
		foreach (GameObject p in currentPortals) {
			Destroy (p);
		}
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
		displayPortals (bases);
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

	private void displayPortals(Base[] bases) {
		StartCoroutine(createPortals (bases));
	}

	// Called in Start() to create portals between player's bases
	private IEnumerator createPortals(Base[] baseLocs) {
		WWWForm wwwform = new WWWForm ();
		wwwform.AddField ("username", "kmw8sf");
		// NOTE: Changed this to "new WWW" from "RequestService.makeRequest" to fix a 500 request failed error
		WWW request = new WWW ("localhost:8080/myapp/world/portals", wwwform);
		yield return request;
		Portal2[] portals = JsonMapper.ToObject<Portal2[]> (request.text);

		currentPortals = new GameObject[portals.Length];
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
			portalObj.renderer.material = portalMaterial;
			currentPortals[i] = portalObj;
		}

	}

	public void clearBases()
	{
		StartCoroutine(coClearBases ());
	}

	private IEnumerator coClearBases() 
	{
		WWW request = RequestService.makeRequest ("world/clear", currentBases [0].GetComponent<TouchBase>().b);
		yield return request;
		GenerateWorld.instance.resetWorldView();
	}

	public void switchBaseClickMode() {
		mode = !mode;
		foreach (GameObject b in currentBases) {
			TouchBase script = (TouchBase)b.GetComponent("TouchBase");
			script.switchMode(mode);
		}
		if (!mode) {
			print ("Click base to add new base");
			changeModeButtonText.text = "In add base mode";

		} else if (mode) {
			print ("Click two bases to add portal between bases");
			changeModeButtonText.text = "In add portal mode";
		}
	}


}
