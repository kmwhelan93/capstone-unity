using UnityEngine;
using System.Collections;

/**
 * This class is for storing variables that belong to the base game object
 */
public class BaseScript : MonoBehaviour, UIPlacer, InstanceObjectScript {
	// TODO: change to type Base if possible
	public InstanceObject instanceObject { get; set; }
	public GameObject objectInfoPanel { get; set; }

	public Vector3 getUIScreenPosition() {
		Vector3 worldOffset = new Vector3((float)Globals.baseRadius*1.3f, 0, -Globals.baseRadius*1.3f);

		Vector3 textPosition = Camera.main.WorldToScreenPoint (transform.position + worldOffset);
		//SetActive(!Globals.isInLocalView);
		return textPosition;
	}

	public Vector2 getPivot() {
		// TODO: implement
		return new Vector2(0, .5f);
	}
}
