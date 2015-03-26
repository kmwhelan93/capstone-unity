using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OIPItemScript : MonoBehaviour {
	public void updateContent(int value) {
		this.GetComponentInChildren<Text> ().text = "units: " + value;
	}
}
