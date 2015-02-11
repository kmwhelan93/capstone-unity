#if UNITY_IPHONE

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OnTouchDown : MonoBehaviour
{
	void Update () {
		// Code for OnMouseDown in the iPhone. Unquote to test.
		RaycastHit hit = new RaycastHit();
		if (Input.touchCount == 1) {
			if (Input.GetTouch(0).phase.Equals(TouchPhase.Began)) {
				// Construct a ray from the current touch coordinates
				Vector2 touchScreenPosition = Input.GetTouch (0).position;
				touchScreenPosition.x = touchScreenPosition.x * -1;
				Vector3 cameraPosition = Camera.main.GetComponent<Transform>().position;
				Vector2 screenDim = new Vector2(Screen.width, Screen.height);
				Vector3 adjustScreenRatio = new Vector3((touchScreenPosition.x - screenDim.x / 2)/ screenDim.x, (touchScreenPosition.y - screenDim.y / 2) / screenDim.y, 1);
				Vector3 touchWorldPosition = cameraPosition + new Vector3(-1*(Camera.main.orthographicSize * 2 * adjustScreenRatio.x * Screen.width / Screen.height),Camera.main.orthographicSize * 2 * adjustScreenRatio.y, 0); 
				Ray ray = new Ray(touchWorldPosition, new Vector3(0, 0, 1));
				if (Physics.Raycast(ray, out hit)) {
					Debug.Log ("hit something!");
					hit.transform.gameObject.SendMessage("OnMouseDown");
				}
			}
		}
	}
}
#endif