using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OIPItemScript : MonoBehaviour {
	public void updateContent(int value) {
		if (this.GetComponentInChildren<Text> () != null)
			this.GetComponentInChildren<Text> ().text = "" + value;
	}
}
