using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class ObjectInfoPanelScript : MonoBehaviour {
	public GameObject o { get; set; }

	void Awake()
	{
		EventManager.positionText += position;
	}

	void position() {
		//SetActive(Camera.main.transform.position.z >= hideTextDepth);
		// TODO (kevin): change this to call a method based on portals
		transform.position = o.GetComponent<UIPlacer> ().getUIScreenPosition ();
	}
}
