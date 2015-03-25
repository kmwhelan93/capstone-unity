using UnityEngine;
using System.Collections;

public class TouchAndDrag : MonoBehaviour {
	public float speed;
	public Vector2 initPosition;

	// Update is called once per frame
	void Update () {
		if (Input.touchCount == 1)
		{
			Touch t = Input.GetTouch (0);
			// If in AddPortal state and first touch is on a base
			if (Globals.opState == OpState.AddPortal && GenerateWorld.instance.secondClick) {
				if (t.phase == TouchPhase.Moved) {
					// Update temp portal
					// If touch location is on base, temp portal = purple to signify that portal is valid
					Vector3 touchWorldPos = GetComponent<Camera>().ScreenToWorldPoint(new Vector3(t.position.x, t.position.y, -GetComponent<Camera>().transform.position.z));
					touchWorldPos.z = 0;
					PortalHandler.instance.updateDragPortal(
						touchWorldPos, 
												  validBaseAtPos(t.position));
				}
				if (t.phase == TouchPhase.Ended) {
					// Create portal if on base
					int bId = getIdOfBaseAtPos(t.position);
					if (bId == -1) {
						// End location not on base
						PortalHandler.instance.restoreInvalidBaseColors();
						PortalHandler.instance.deleteDragPortal();
					}
					else {
						if (bId == GenerateWorld.instance.lastBase.baseId) {
							// Portal created by touching each base separately, waiting for second touch
							PortalHandler.instance.deleteDragPortal();
							GenerateWorld.instance.message.text = "A portal must connect two different bases";
							GenerateWorld.instance.secondClick = false;
						}
						else if (PortalHandler.instance.validBase(bId)) {
							// Portal created by dragging
							// restore invalid base colors
							PortalHandler.instance.restoreInvalidBaseColors();
							PortalHandler.instance.deleteDragPortal();
							GenerateWorld.instance.secondClick = false;
							GenerateWorld.instance.message.text = "Adding portal...";
							StartCoroutine ("createPortal", bId);
						}
						else {
							PortalHandler.instance.restoreInvalidBaseColors();
							PortalHandler.instance.deleteDragPortal();
						}
					}
				}
			}
			else if (t.phase == TouchPhase.Moved && t.deltaPosition.magnitude > 2 && !GenerateWorld.instance.slider.IsActive()) {
				// Drag to move camera
				Vector2 deltaPosition = Input.GetTouch (0).deltaPosition;
				Vector2 deltaToScreenRatio = deltaPosition * GetComponent<Camera>().orthographicSize / 5;
				if (!Globals.isInLocalView) {
					GetComponent<Camera>().transform.Translate(deltaToScreenRatio * -1 *speed);
					Camera.main.GetComponent<DisplayInfoHandler>().positionText();
				} else {
					GetComponent<LocalView>().rotateQuickly = true;
					GetComponent<LocalView>().rotateAngle += deltaPosition.x / Screen.width * 90;
					GetComponent<LocalView>().moveToNewEndPosition(GetComponent<LocalView>().rotateAngle);
					
				}
			}
		}
	}

	private int getIdOfBaseAtPos(Vector2 pos) {
		Ray ray = Camera.main.ScreenPointToRay(pos);
		RaycastHit hit;
		
		if ( Physics.Raycast(ray, out hit, 100f ) ) {
			string name = hit.transform.gameObject.name;
			if (name.Substring(0,4).Equals("Base")) return int.Parse(name.Substring(4));
		}
		return -1;
	}

	private bool validBaseAtPos(Vector2 pos) {
		Ray ray = Camera.main.ScreenPointToRay(pos);
		RaycastHit hit;
		
		if (Physics.Raycast (ray, out hit, 100f)) {
			if (hit.transform.gameObject.name.Substring(0,4).Equals("Base")) {
				if (PortalHandler.instance.validBase(int.Parse(hit.transform.gameObject.name.Substring(4)))) {
					return true;
				}
			}
		}
		return false;
	}

	IEnumerator createPortal(int base2Id)
	{
		WWWForm wwwform = new WWWForm ();
		wwwform.AddField ("username", Globals.username);
		wwwform.AddField ("baseId1", GenerateWorld.instance.lastBase.baseId);
		wwwform.AddField ("baseId2", base2Id);
		long finishTime = CurrentTime.currentTimeMillis() + (long)(Globals.portalBuildTimeInMins * 60000.0);
		wwwform.AddField ("timeFinished", finishTime + "");
		wwwform.AddField ("cost", PortalHandler.instance.costPerPortal);
		WWW request = new WWW ("localhost:8080/myapp/world/portals/create", wwwform);
		yield return request;
		GenerateWorld.instance.message.text = request.text;
		GenerateWorld.instance.resetWorldView ();
		UpdateGold.instance.syncGold ();
		DisplayTransactionHandler.instance.setCostText(PortalHandler.instance.costPerPortal);
	}
}
