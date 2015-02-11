using UnityEngine;
using System;
using LitJson;
using System.Collections;

public class GenerateWorld : MonoBehaviour {

	public static GenerateWorld instance;
	public GameObject basePrefab;
	public GameObject portalPrefab;
	public Material[] materials;
	private GameObject[] currentBases;

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
	}

	private IEnumerator coResetWorldView() {
		WWWForm wwwform = new WWWForm ();
		wwwform.AddField ("username", "kmw8sf");
		WWW request = new WWW ("localhost:8080/myapp/world/bases", wwwform);
		yield return request;
		destroyCurrentBases ();
		Base[] bases = JsonMapper.ToObject<Base[]>(request.text);
		displayBases (bases);
		createPortals (bases);
		yield break;
	}

	// Called in Start() to load player's bases and place them on the screen
	bool displayBases(Base[] baseLocs) {
		// Place objects
		currentBases = new GameObject[baseLocs.Length];
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
		}
		return true;
	}

	// Called in Start() to create portals between player's bases
	bool createPortals(Base[] baseLocs) {
		// Make request and get JSON
		string json = @"[
            {
				""baseId1"": 1,
				""baseId2"": 111
			},
			{
				""baseId1"": 1,
				""baseId2"": 137
			},
			{
				""baseId1"": 1,
				""baseId2"": 138
			},
			{
				""baseId1"": 1,
				""baseId2"": 139
			}
        ]";

		Portal[] portals = JsonMapper.ToObject<Portal[]>(json);

		for (int i = 0; i < portals.Length; i++) {
			for (int j = 0; j < baseLocs.Length; j++) {
				if (baseLocs[j].baseId == portals[i].baseId1) {
					for (int k = 0; k < baseLocs.Length; k++) {
						if (baseLocs[k].baseId == portals[i].baseId2) {
							// Locations of the two bases
							int x1 = baseLocs[j].world.x * 3 + baseLocs[j].local.x;
							int y1 = baseLocs[j].world.y * 3 + baseLocs[j].local.y;
							int x2 = baseLocs[k].world.x * 3 + baseLocs[k].local.x;
							int y2 = baseLocs[k].world.y * 3 + baseLocs[k].local.y;

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
					break;
				}
			}
		}

		return true;
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
