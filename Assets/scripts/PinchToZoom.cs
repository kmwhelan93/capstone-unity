using UnityEngine;
using System.Collections;

public class PinchToZoom : MonoBehaviour {

	public float orthoZoomSpeed;
	public float perspectiveZoomSpeed;
	
	// Update is called once per frame
	void Update () {
		if (Input.touchCount == 2)
		{
			Touch touchZero = Input.GetTouch(0);
			Touch touchOne = Input.GetTouch (1);

			Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
			Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

			float prevLength = (touchZeroPrevPos - touchOnePrevPos).magnitude;
			float currLength = (touchZero.position - touchOne.position).magnitude;

			if (camera.isOrthoGraphic)
			{
				camera.orthographicSize -= (currLength - prevLength) * orthoZoomSpeed;
				camera.orthographicSize = Mathf.Max (0.1f, camera.orthographicSize);
			} else
			{
				camera.fieldOfView -= (currLength - prevLength) * perspectiveZoomSpeed;
				camera.fieldOfView = Mathf.Clamp (camera.fieldOfView, 0.1f, 179.9f);
			}
			Camera.main.GetComponent<DisplayInfoHandler>().positionText();
		}
	}
}
