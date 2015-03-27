using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class ObjectInfoPanelScript : MonoBehaviour {
	public GameObject o { get; set; }
	public event EventManager.UpdateContentEvent updateContentEvent;

	void Awake()
	{
		EventManager.positionText += position;
	}

	void position() {
		//SetActive(Camera.main.transform.position.z >= hideTextDepth);
		// TODO (kevin): change this to call a method based on portals
		Vector3 worldOffset = new Vector3((float)Globals.baseRadius*1.1f, 0, 0);
		if (o != null) {
			Vector3 textPosition = Camera.main.WorldToScreenPoint (o.transform.position + worldOffset);
			//SetActive(!Globals.isInLocalView);
			transform.position = textPosition;
		}
	}
}
