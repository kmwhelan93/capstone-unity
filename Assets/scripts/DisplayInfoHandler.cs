using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class DisplayInfoHandler : MonoBehaviour {

	public List<BaseWrapper> baseWrappers { get; set; }

	public void positionText()
	{
		foreach(BaseWrapper bw in baseWrappers) {
			Vector3 textPosition = Camera.main.WorldToScreenPoint(bw.baseObj.transform.position) - new Vector3(Screen.width / 2, Screen.height / 2, 0);
			Vector3 offset = new Vector3 (15, 15, 0);
			bw.displayText.transform.localPosition = textPosition + offset;
		}

	}
}
