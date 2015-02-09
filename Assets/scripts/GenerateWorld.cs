using UnityEngine;
using System;
using LitJson;
using System.Collections;

public class GenerateWorld : MonoBehaviour {

	public GameObject basePrefab;
	public Material[] materials;
	
	// Use this for initialization
	void Start () {
		// load resources
		loadResources ();
		// Create bases
		StartCoroutine("createWorldView");
		// Create portals

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

	IEnumerator createWorldView() {
		WWWForm wwwform = new WWWForm ();
		wwwform.AddField ("username", "kmw8sf");
		WWW request = new WWW ("localhost:8080/myapp/world/bases", wwwform);
		yield return request;
		Base[] bases = JsonMapper.ToObject<Base[]>(request.text);
		createBases (bases);
		//createPortals (baseLocs);
		yield break;
	}

	// Called in Start() to load player's bases and place them on the screen
	bool createBases(Base[] baseLocs) {
		// Place objects
		for (int i = 0; i < baseLocs.Length; i++) {
			int x = baseLocs[i].world.x * 3 + baseLocs[i].local.x;
			int y = baseLocs[i].world.y * 3 + baseLocs[i].local.y;
			//print ("Base " + i + " -> wx: " + baseLocs[i].world.x + " lx: " + baseLocs[i].local.x + " x: " + x);
			//print ("Base " + i + " -> wy: " + baseLocs[i].world.y + " ly: " + baseLocs[i].local.y + " y: " + y);
			
			GameObject baseObj = (GameObject) Instantiate (basePrefab, new Vector3(x, y, 0), Quaternion.identity);
			baseObj.renderer.material = materials[baseLocs[i].colorId % materials.Length];
			TouchBase tb = baseObj.GetComponent<TouchBase>();
			tb.b = baseLocs[i];
		}
		return true;
	}

	// Called in Start() to create portals between player's bases
	bool createPortals(Base[] baseLocs) {
		// Make request and get JSON
		string json = @"[
            {
				""baseId1"": 0,
				""baseId2"": 1,
			},
			{
				""baseId1"": 0,
				""baseId2"": 2,
			},
			{
				""baseId1"": 0,
				""baseId2"": 3,
			},
			{
				""baseId1"": 1,
				""baseId2"": 6,
			},
        ]";

		Portal[] portals = JsonMapper.ToObject<Portal[]>(json);

		for (int i = 0; i < portals.Length; i++) {
			for (int j = 0; j < baseLocs.Length; j++) {
				if (baseLocs[j].baseId == portals[i].baseId1) {
					for (int k = 0; k < baseLocs.Length; k++) {
						if (baseLocs[k].baseId == portals[i].baseId2) {
							int x1 = baseLocs[j].world.x * 3 + baseLocs[j].local.x;
							int y1 = baseLocs[j].world.y * 3 + baseLocs[j].local.y;
							int x = baseLocs[i].world.x * 3 + baseLocs[i].local.x;
							int y = baseLocs[i].world.y * 3 + baseLocs[i].local.y;
						}
					}
					break;
				}
			}
		}

		return true;
	}


}
