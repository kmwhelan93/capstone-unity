using UnityEngine;
using LitJson;
using System.Collections;
using System.Collections.Generic;

public class PortalHandler : MonoBehaviour {

	public static PortalHandler instance;

	public GameObject portalPrefab;

	public Material portalMaterial;
	public Material portalBuildingMaterial;
	public Material dragPortalInvalidMaterial;
	public Material invalidBaseMaterial;

	private Portal2[] portals;
	private GameObject[] currentPortalObjects;
	private List<Portal2> currentUnfinishedPortals;
	private List<GameObject> currentUnfinishedPortalObjs;
	private GameObject dragToCreateTempPortal;
	private int[] validBaseIds; // For graying out bases during portal creation

	void Start() {
		instance = this;
		loadResources ();
	}

	private void loadResources() {
		portalMaterial = (Material) Resources.Load ("materials/PortalPlasma", typeof(Material));
		portalBuildingMaterial = (Material)Resources.Load ("materials/PortalPlasma2", typeof(Material));
		dragPortalInvalidMaterial = (Material)Resources.Load ("materials/PortalLava", typeof(Material));
		invalidBaseMaterial = (Material)Resources.Load ("materials/base_gray", typeof(Material));
	}

	public GameObject createPortal(Vector3 p1, Vector3 p2) {
		GameObject portalObj = (GameObject) Instantiate (portalPrefab, (p1 + p2) / 2, Quaternion.identity);
		scalePortalBetweenPoints (portalObj, p1, p2);
		rotatePortalBetweenPoints (portalObj, p1, p2);
		return portalObj;
	}
	
	public void scalePortalBetweenPoints(GameObject portalObj, Vector3 p1, Vector3 p2) {
		// Scale portal based on distance between bases
		Vector3 scale = portalObj.transform.localScale;
		scale.y = Vector2.Distance(p1, p2) / 2.0f;
		portalObj.transform.localScale = scale;
	}
	
	public void rotatePortalBetweenPoints(GameObject portalObj, Vector3 p1, Vector3 p2) {
		// Rotate portal based on angle between bases
		Vector3 rotate = portalObj.transform.eulerAngles;
		rotate.z = Utility.getAngleRelativeToVertical (p2 - p1);
		portalObj.transform.eulerAngles = rotate;
	}

	public void destroyCurrentPortals() {
		if (currentPortalObjects != null) {
			foreach (GameObject p in currentPortalObjects) {
				Destroy (p);
			}
		}
		currentUnfinishedPortals = null;
		Destroy (dragToCreateTempPortal);
		return;
	}

	public void displayPortals() {
		StartCoroutine(createPortals ());
	}

	// TODO: move this and change Base's points to Vector2's to make this easier
	private Vector2 convertBaseCoordsToWorld(Base b)
	{
		return new Vector3 ( 2 * Globals.baseRadius * (b.world.x * 3 + b.local.x), Globals.baseRadius * 2 * (b.world.y * 3 + b.local.y), 0);
	}
	
	private IEnumerator createPortals() {
		WWWForm wwwform = new WWWForm ();
		wwwform.AddField ("username", "kmw8sf");
		// NOTE: Changed this to "new WWW" from "RequestService.makeRequest" to fix a 500 request failed error
		WWW request = new WWW ("localhost:8080/myapp/world/portals", wwwform);
		yield return request;
		portals = JsonMapper.ToObject<Portal2[]> (request.text);
		
		currentPortalObjects = new GameObject[portals.Length];
		currentUnfinishedPortals = new List<Portal2>();
		currentUnfinishedPortalObjs = new List<GameObject>();
		for (int i = 0; i < portals.Length; i++) {
			// Locations of the two bases
			Portal2 portal = portals[i];
			Vector3 p1 = convertBaseCoordsToWorld(portal.base1);
			Vector3 p2 = convertBaseCoordsToWorld(portal.base2);

			p1 = p1 + (p2 - p1).normalized * Globals.baseRadius;
			p2 = p2 + (p1 - p2).normalized * Globals.baseRadius;
			GameObject portalObj;
			
			// Create the portal (cylinder prefab)
			// Start new, user created portals at length corresponging to 1% finished
			if (portal.timeFinished <= CurrentTime.currentTimeMillis()) {
				// Portal finished
				portalObj = createPortal(p1, p2);
				portalObj.GetComponent<Renderer>().material = portalMaterial;
			}
			else {
				// Portal unfinished
				portalObj = createPortal(p1, p1 + (p2- p1)*.01f);
				portalObj.GetComponent<Renderer>().material = portalBuildingMaterial;
				currentUnfinishedPortals.Add(portal);
				currentUnfinishedPortalObjs.Add(portalObj);
			}
			portalObj.name = "Portal" + portal.portalId;
			currentPortalObjects[i] = portalObj;
		}
		// TODO (cem6at): Find better location for this
		if (TroopsHandler.instance.moveTroopsActions.Count == 0) {
			TroopsHandler.instance.restartMoveTroops ();
		}
		
	}

	private GameObject getUnfinishedPortalObj(int pId) {
		foreach (GameObject p in currentUnfinishedPortalObjs) {
			if (p.name.Equals("Portal" + pId)) return p;
		}
		return null;
	}

	public void updateUnfinishedPortals() {
		if (currentUnfinishedPortals != null) {
			if (currentUnfinishedPortals.Count != 0) {
				syncUnfinishedPortals (currentUnfinishedPortals);
			}
		}
	}

	private void syncUnfinishedPortals(List<Portal2> currentUnfinishedPortals) {
		List<Portal2> toRemove = new List<Portal2> ();
		foreach (Portal2 p in currentUnfinishedPortals) {
			// Finished yet?
			float percentFinished = getPortalPercentFinished(p);
			GameObject portalObj = GameObject.Find("Portal" + p.portalId);
			if (percentFinished < 1.0f) {
				// Grow portal
				Vector3 p1 = convertBaseCoordsToWorld(p.base1);
				Vector3 p2 = convertBaseCoordsToWorld(p.base2);
				
				p1 = p1 + (p2 - p1).normalized * Globals.baseRadius;
				p2 = p2 + (p1 - p2).normalized * Globals.baseRadius;

				p2 = p1 + (p2-p1)*percentFinished;
				portalObj.transform.position = (p1 + p2) / 2;
				scalePortalBetweenPoints(portalObj, p1, p2);
			}
			else {
				// Finish portal
				portalObj.GetComponent<Renderer>().material = portalMaterial;
				toRemove.Add(p);
				GenerateWorld.instance.message.text = "Portal finished!";
			}
		}
		foreach (Portal2 p in toRemove) {
			currentUnfinishedPortals.Remove(p);
		}
	}

	// true if portal finished, false if unfinished
	public float getPortalPercentFinished(Portal2 p) {
		long buildTimeMilli = (long)(Globals.portalBuildTimeInMins * 60000.0);
		long curTime = CurrentTime.currentTimeMillis ();
		if (p.timeFinished <= curTime) {
			// Portal finished
			return 1.0f;
		}
		long startTime = p.timeFinished - buildTimeMilli;
		return (((curTime - startTime) / (buildTimeMilli*1.0f))*1.0f);
	}

	public void createDragPortal(Vector2 pos) {
		float x1 = pos.x * 3 + pos.x;
		float y1 = pos.y * 3 + pos.y;
		float x2 = x1 + 0.1f;
		float y2 = y1 + 0.1f;
		
		// Create the portal (cylinder prefab)
		dragToCreateTempPortal = createPortal (new Vector2(x1, y1), new Vector2(x2, y2));
		dragToCreateTempPortal.name = "tempPortal";
	}
	
	public void updateDragPortal(Vector2 pos, bool onBase) {
		float x1 = GenerateWorld.instance.lastBase.world.x * 3 + GenerateWorld.instance.lastBase.local.x;
		float y1 = GenerateWorld.instance.lastBase.world.y * 3 + GenerateWorld.instance.lastBase.local.y;
		float x2 = pos.x;
		float y2 = pos.y;
		
		// Create the portal (cylinder prefab)
		dragToCreateTempPortal.transform.localPosition = new Vector3((x1+x2)/2.0f, (y1+y2)/2.0f, 0f);
		scalePortalBetweenPoints (dragToCreateTempPortal, new Vector2(x1, y1), new Vector2(x2, y2));
		rotatePortalBetweenPoints (dragToCreateTempPortal, new Vector2(x1, y1), new Vector2(x2, y2));
		dragToCreateTempPortal.GetComponent<Renderer>().material = onBase ? portalMaterial : dragPortalInvalidMaterial;
	}
	
	public void deleteDragPortal() {
		Destroy (dragToCreateTempPortal);
		dragToCreateTempPortal = null;
	}
	
	public void grayOutInvalidBases(int base1Id) {
		StartCoroutine ("getValidBases", base1Id);
	}
	
	private IEnumerator getValidBases(int base1Id) {
		WWWForm wwwform = new WWWForm ();
		wwwform.AddField ("username", "kmw8sf");
		wwwform.AddField ("base1Id", base1Id);
		// NOTE: Changed this to "new WWW" from "RequestService.makeRequest" to fix a 500 request failed error
		WWW request = new WWW ("localhost:8080/myapp/world/bases/valid", wwwform);
		yield return request;
		string[] split = request.text.Split (';');
		validBaseIds = new int[split.Length];
		for (int i = 0; i < split.Length; i++) {
			validBaseIds[i] = int.Parse(split[i]);
		}
		grayInvalidBases ();
	}
	
	private void grayInvalidBases() {
		GameObject[] currentBases = GenerateWorld.instance.getCurrentBases ();
		foreach (GameObject b in currentBases) {
			if (!validBase(int.Parse(b.name.Substring(4)))) {
				b.GetComponent<Renderer>().material = invalidBaseMaterial;
			}
		}
	}
	
	public bool validBase(int baseId) {
		if (!GenerateWorld.instance.secondClick) {
			return true;
		} else {
			foreach (int bId in validBaseIds) {
				if (bId == baseId) {
					return true;
				}
			}
		}
		return false;
	}
	
	public void restoreInvalidBaseColors() {
		GenerateWorld.instance.secondClick = false;
		validBaseIds = null;
		GenerateWorld.instance.resetWorldView();
	}

	public GameObject getFinishedPortalObj(int pId) {
		foreach (GameObject p in currentPortalObjects) {
			if (p.name == "Portal" + pId) return p;
		}
		return null;
	}

	public bool areConnected(GameObject baseObj1, GameObject baseObj2)
	{
		Base base1 = baseObj1.GetComponent<TouchBase> ().b;
		Base base2 = baseObj2.GetComponent<TouchBase> ().b;
		foreach (Portal2 portal in portals) {
			if ((portal.base1.Equals (base1) && portal.base2.Equals(base2)) || (portal.base1.Equals(base2) && portal.base2.Equals(base1))) {
				return true;
			}
		}
		return false;
	}
	
}
