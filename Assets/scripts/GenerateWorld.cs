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
	private GameObject[] currentBases;
	private GameObject[] currentDisplayText;
	public GameObject canvas;

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

	private IEnumerator coResetWorldView() {
		WWWForm wwwform = new WWWForm ();
		wwwform.AddField ("username", "kmw8sf");
		WWW request = new WWW ("localhost:8080/myapp/world/bases", wwwform);
		yield return request;
		destroyCurrentBases ();
		Base[] bases = JsonMapper.ToObject<Base[]>(request.text);
		displayBases (bases);
		StartCoroutine(createPortals (bases));
		yield break;
	}

	// Called in Start() to load player's bases and place them on the screen
	bool displayBases(Base[] baseLocs) {
		// Place objects
		currentBases = new GameObject[baseLocs.Length];
		currentDisplayText = new GameObject[baseLocs.Length];
		List<BaseWrapper> baseWrappers = new List<BaseWrapper> ();
		for (int i = 0; i < baseLocs.Length; i++) {
			int x = baseLocs[i].world.x * 3 + baseLocs[i].local.x;
			int y = baseLocs[i].world.y * 3 + baseLocs[i].local.y;
			//print ("Base " + i + " -> wx: " + baseLocs[i].world.x + " lx: " + baseLocs[i].local.x + " x: " + x);
			//print ("Base " + i + " -> wy: " + baseLocs[i].world.y + " ly: " + baseLocs[i].local.y + " y: " + y);
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
		return true;
	}

	// Called in Start() to create portals between player's bases
	IEnumerator createPortals(Base[] baseLocs) {
		WWW request = RequestService.makeRequest ("world/portals", new {username = "kmw8sf"});
		yield return request;
		Portal2[] portals = JsonMapper.ToObject<Portal2[]> (request.text);
		// Make request and get JSON
		/*
		string json = @"[
            {
				""baseId1"": 1,
				""baseId2"": 142
			},
			{
				""baseId1"": 1,
				""baseId2"": 143
			},
			{
				""baseId1"": 1,
				""baseId2"": 144
			},
			{
				""baseId1"": 142,
				""baseId2"": 145
			}
        ]";

		Portal[] portals = JsonMapper.ToObject<Portal[]>(json);
		*/

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


}
