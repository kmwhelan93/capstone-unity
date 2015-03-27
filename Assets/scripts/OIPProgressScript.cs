using UnityEngine;
using System.Collections;
using ProgressBar;
using LitJson;


public class OIPProgressScript : MonoBehaviour {
	public int min { get; set; }
	public int max { get; set; }
	public void updateContent(int value) {
		if (GetComponentInChildren<ProgressBarBehaviour> ().Value != null) {
			float percentage = (max - value) / (float)max * 100;
			Debug.Log (percentage);
			GetComponentInChildren<ProgressBarBehaviour> ().Value = percentage;
		}
	}

	void Start() {
		//GetComponentInChildren<ProgressBarBehaviour> ().Value = 50;
	}
}
