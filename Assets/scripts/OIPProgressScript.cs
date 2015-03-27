using UnityEngine;
using System.Collections;
using ProgressBar;
using LitJson;


public class OIPProgressScript : MonoBehaviour {
	public void updateContent(int value) {
	}
	// Use this for initialization
	void Start () {
		GetComponentInChildren<ProgressBarBehaviour> ().Value = 50;
		test t = new test ();
		t.a = 4;
		Debug.Log (JsonMapper.ToJson (t));
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	class test {
		[SerializeField]
		public int a { get; set; }

		public int b { get; set; }
	}
}
