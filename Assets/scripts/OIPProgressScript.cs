using UnityEngine;
using System.Collections;
using ProgressBar;
using LitJson;


public class OIPProgressScript : MonoBehaviour {
	private int min = 0;
	private int max;
	private bool maxHasBeenSet = false;
	public void updateContent(int value) {
		if (maxHasBeenSet == false) {
			maxHasBeenSet = true;
			max = value;
		}
		if (value == 0) {
			done ();
		}
		if (GetComponentInChildren<ProgressBarBehaviour> ().Value != null) {
			float percentage = (max - value) / (float)max * 100;
			GetComponentInChildren<ProgressBarBehaviour> ().Value = percentage;
		}
	}

	void Start() {
		//GetComponentInChildren<ProgressBarBehaviour> ().Value = 50;
	}

	public void done() {
		Destroy (gameObject);
	}
}
