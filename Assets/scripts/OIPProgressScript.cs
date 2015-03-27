using UnityEngine;
using System.Collections;
using ProgressBar;
using LitJson;


public class OIPProgressScript : MonoBehaviour {
	public void updateContent(int value) {
		if (GetComponentInChildren<ProgressBarBehaviour> ().Value != null) {
			//Debug.Log (GetComponentInChildren<ProgressBarBehaviour> ().Value);
			//GetComponentInChildren<ProgressBarBehaviour> ().Value = 100;
		}
	}

	void Start() {
		//GetComponentInChildren<ProgressBarBehaviour> ().Value = 50;
	}
}
