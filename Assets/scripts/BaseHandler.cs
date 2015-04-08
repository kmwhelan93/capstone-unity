using UnityEngine;
using System.Collections;

public class BaseHandler : MonoBehaviour {
	public static BaseHandler instance { get; set; }

	private Material[] materials { get; set; }
	// Use this for initialization
	void Awake () {
		instance = this;
	}

	void Start() {
		string[] materialNames = {
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

	public void setColor(Base b) {
		b.gameObject.GetComponent<Renderer> ().material = materials[b.colorId % materials.Length];
	}

}
