using UnityEngine;
using System.Collections;

public class TouchAndDrag : MonoBehaviour {
	public float speed;

	
	// Update is called once per frame
	void Update () {
		if (Input.touchCount == 1)
		{
			Vector2 deltaPosition = Input.GetTouch (0).deltaPosition;
			Vector2 deltaToScreenRatio = deltaPosition * camera.orthographicSize / 5;
			camera.transform.Translate(deltaToScreenRatio * -1 *speed);
		}
	}
}
