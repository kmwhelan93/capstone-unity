using UnityEngine;
using System.Collections;

/**
 * A class to hold variables that belong to the Portal GameObject itself
 */
public class PortalScript : MonoBehaviour, UIPlacer, InstanceObjectScript {

	// TODO: change to type Portal2 if possible
	public InstanceObject instanceObject { get; set; }
	//public GameObject objectInfoPanel { get; set; }

	public Vector3 getUIScreenPosition() 
	{
		Vector3 worldOffset = new Vector3(0, 0, 0);
		Vector3 textPosition = Camera.main.WorldToScreenPoint (transform.position + worldOffset);
		return textPosition;
	}

	public Vector2 getPivot() {
		// TODO: implement
		return new Vector2 (0, .5f);
	}
}
