﻿using UnityEngine;
using System.Collections;
using ProgressBar;
using LitJson;


public class OIPProgressScript : MonoBehaviour {
	private int min = 0;
	private int max;
	private int value = 0;
	public void updateContent(int value) {
		if (value - this.value > 0) {
			this.gameObject.SetActive(true);
			max = value;
		}
		this.value = value;
		if (value == 0) {
			this.gameObject.SetActive (false);
			return;
		}
		if (GetComponentInChildren<ProgressBarBehaviour> ().Value != null) {
			float percentage = (max - value) / (float)max * 100;
			GetComponentInChildren<ProgressBarBehaviour> ().Value = percentage;
		}
	}

	void Start() {
		//GetComponentInChildren<ProgressBarBehaviour> ().Value = 50;
	}
	
}