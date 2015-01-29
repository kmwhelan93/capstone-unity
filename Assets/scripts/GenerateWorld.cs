using UnityEngine;
using System.Collections;
using LitJson;

public class GenerateWorld : MonoBehaviour {

	// Use this for initialization
	void Start () {
		// Make request and get JSON
		string json = @"[
            {
				""baseId"": 0,
			    ""world"": {""x"": 0, ""y"": 0},
  			    ""local"": {""x"": 0, ""y"": 0}
			},
			{
				""baseId"": 1,
				""world"": {""x"": 1, ""y"": 0},
				""local"": {""x"": 1, ""y"": -1}
			},
			{
				""baseId"": 2,
				""world"": {""x"": 1, ""y"": 1},
				""local"": {""x"": -1, ""y"": 0}
			},
			{
				""baseId"": 3,
				""world"": {""x"": -1, ""y"": 1},
				""local"": {""x"": 1, ""y"": 1}
			},
			{
				""baseId"": 4,
				""world"": {""x"": -1, ""y"": -1},
				""local"": {""x"": -1, ""y"": -1}
			},
			{
				""baseId"": 5,
				""world"": {""x"": 0, ""y"": -1},
				""local"": {""x"": 1, ""y"": -1}
			},
			{
				""baseId"": 6,
				""world"": {""x"": -2, ""y"": 1},
				""local"": {""x"": 0, ""y"": 0}
			}
        ]";
		
		BaseLoc[] baseLocs = JsonMapper.ToObject<BaseLoc[]>(json);
		//print ("JSON length " + baseLocs.Length);

		// Place objects
		Material[] materials = {(Material)Resources.Load("base_orange", typeof(Material)),
		                        (Material)Resources.Load("base_green", typeof(Material)),
								(Material)Resources.Load("base_blue", typeof(Material)),
								(Material)Resources.Load("base_yellow", typeof(Material)),
			                    (Material)Resources.Load("base_purple", typeof(Material)),
								(Material)Resources.Load("base_pink", typeof(Material)),
								(Material)Resources.Load("base_teal", typeof(Material))
			                    };
		for (int i = 0; i < baseLocs.Length; i++) {
			int x = baseLocs[i].world.x * 3 + baseLocs[i].local.x;
			int y = baseLocs[i].world.y * 3 + baseLocs[i].local.y;
			//print ("Base " + i + " -> wx: " + baseLocs[i].world.x + " lx: " + baseLocs[i].local.x + " x: " + x);
			//print ("Base " + i + " -> wy: " + baseLocs[i].world.y + " ly: " + baseLocs[i].local.y + " y: " + y);

			GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			sphere.transform.position = new Vector3(x, y, 0);
			sphere.renderer.material = materials[i % materials.Length];
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
