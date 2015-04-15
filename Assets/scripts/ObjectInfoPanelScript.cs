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
		if (o == null)
			return;
		Vector3 position = o.GetComponent<UIPlacer> ().getUIScreenPosition ();
		if (position.z < 2 ||  position.z > 10) {
			this.gameObject.SetActive (false);
		} else {
			this.gameObject.SetActive(true);
			transform.position = position;
		}

	}
}
