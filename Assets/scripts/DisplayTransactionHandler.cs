using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class DisplayTransactionHandler : MonoBehaviour {

	public static DisplayTransactionHandler instance;

	public GameObject costText;

	// Use this for initialization
	void Start () {
		instance = this;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void setCostText(int cost) {
		costText.GetComponent<Text> ().text = "-"  + cost + " gold";

		// Start fade coroutine
		StartCoroutine ("fade");
	}

	IEnumerator fade() {
		float f = 1f;
		while (f > -0.1f) {
			Color c = costText.GetComponent<Text> ().color;
			// to make sure end alpha is always zero
			if (f > 0f) {
				c.a = f;
				f -= 0.01f;
			} else {
				c.a = 0f;
				f = -0.1f;
			}
			costText.GetComponent<Text> ().color = c;
			yield return null;
		}
	}
}
