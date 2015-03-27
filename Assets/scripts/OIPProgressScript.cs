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
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
