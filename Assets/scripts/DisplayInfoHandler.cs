using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class DisplayInfoHandler : MonoBehaviour {

	public List<BaseWrapper> baseWrappers { get; set; }

	public void positionText()
	{
		foreach(BaseWrapper bw in baseWrappers) {
			GameObject baseObj = bw.baseObj;
			float radius = baseObj.GetComponent<SphereCollider>().radius;
			Vector3 worldOffset = new Vector3((float)radius*1.1f, (float)radius, 0);

			Vector3 textPosition = Camera.main.WorldToScreenPoint(bw.baseObj.transform.position + worldOffset) - new Vector3(Screen.width / 2, Screen.height / 2, 0);
			bw.displayText.transform.localPosition = textPosition;
		}

	}
}
