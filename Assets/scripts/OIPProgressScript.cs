using UnityEngine;
using System.Collections;
using ProgressBar;
using LitJson;


public class OIPProgressScript : MonoBehaviour {
	private long min = 0;
	public long max;
	// value specifies how many are left, not how many have been used
	public long value = 0;
	public void updateContent(long value) {
		value = (long)Mathf.Abs (value);
		if (value - this.value > 0) {
			this.gameObject.SetActive(true);
			max = value;
		}
		this.value = value;
		if (value == 0) {
			this.gameObject.SetActive (false);
			return;
		}
		if (GetComponentInChildren<ProgressBarBehaviour> () != null && GetComponentInChildren<ProgressBarBehaviour> ().Value != null) {
			float percentage = (max - value) / (float)max * 100;
			GetComponentInChildren<ProgressBarBehaviour> ().Value = percentage;
		}
	}

	void Start() {
		//GetComponentInChildren<ProgressBarBehaviour> ().Value = 50;
	}
	
}
