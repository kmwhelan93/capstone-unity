using UnityEngine;
using System.Collections;

public class WormHoleScript : MonoBehaviour, InstanceObjectScript, UIPlacer {
	//TODO: change to WormHole type if possible
	public InstanceObject instanceObject { get; set; }

	public Vector3 getUIScreenPosition() {
		WormHole w = (WormHole)instanceObject;
		Vector3 worldOffset = new Vector3 (0, 0, 0);
		if (w.relativeCoords.y > 0) {
			worldOffset = new Vector3 (0, this.gameObject.transform.localScale.x, 0);
		} else if (w.relativeCoords.y < 0) {
			worldOffset = new Vector3 (0, -this.gameObject.transform.localScale.x, 0);
		}
		Vector3 textPosition = Camera.main.WorldToScreenPoint (transform.position + worldOffset);
		//SetActive(!Globals.isInLocalView);
		return textPosition;
	}

	public Vector2 getPivot() {
		WormHole w = (WormHole)instanceObject;
		Vector2 retVal = new Vector2 ();
		if (w.relativeCoords.x < 0) {
			retVal.x = 1;;
		} else {
			retVal.x = 0;
		}
		if (w.relativeCoords.y > 0)
			retVal.y = 0;
		else
			retVal.y = 1;
		return retVal;
	}
}
