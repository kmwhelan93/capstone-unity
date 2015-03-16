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
	
	private GameObject[] currentPortals;
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

	public GameObject createPortal(float x1, float y1, float x2, float y2) {
		GameObject portalObj = (GameObject) Instantiate (portalPrefab, new Vector3((x1+x2)/2.0f, (y1+y2)/2.0f, 0f), Quaternion.identity);
		scalePortalBetweenPoints (portalObj, x1, y1, x2, y2);
		rotatePortalBetweenPoints (portalObj, x1, y1, x2, y2);
		return portalObj;
	}
	
	public void scalePortalBetweenPoints(GameObject portalObj, float x1, float y1, float x2, float y2) {
		// Scale portal based on distance between bases
		Vector3 scale = portalObj.transform.localScale;
		scale.y = Vector2.Distance(new Vector2(x1,y1), new Vector2(x2,y2)) / 2.0f;
		portalObj.transform.localScale = scale;
	}
	
	public void rotatePortalBetweenPoints(GameObject portalObj, float x1, float y1, float x2, float y2) {
		// Rotate portal based on angle between bases
		float slope = (y2*1.0f-y1)/(x2*1.0f-x1);
		float angle = Mathf.Rad2Deg*Mathf.Atan(slope) + 90;
		Vector3 rotate = portalObj.transform.eulerAngles;
		rotate.z = angle;
		portalObj.transform.eulerAngles = rotate;
	}

	public void destroyCurrentPortals() {
		if (currentPortals != null) {
			foreach (GameObject p in currentPortals) {
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
	
	private IEnumerator createPortals() {
		WWWForm wwwform = new WWWForm ();
		wwwform.AddField ("username", "kmw8sf");
		// NOTE: Changed this to "new WWW" from "RequestService.makeRequest" to fix a 500 request failed error
		WWW request = new WWW ("localhost:8080/myapp/world/portals", wwwform);
		yield return request;
		Portal2[] portals = JsonMapper.ToObject<Portal2[]> (request.text);
		
		currentPortals = new GameObject[portals.Length];
		currentUnfinishedPortals = new List<Portal2>();
		currentUnfinishedPortalObjs = new List<GameObject>();
		for (int i = 0; i < portals.Length; i++) {
			// Locations of the two bases
			Portal2 portal = portals[i];
			float x1 = portal.base1.world.x * 3 + portal.base1.local.x;
			float y1 = portal.base1.world.y * 3 + portal.base1.local.y;
			float x2 = portal.base2.world.x * 3 + portal.base2.local.x;
			float y2 = portal.base2.world.y * 3 + portal.base2.local.y;
			
			// Create the portal (cylinder prefab)
			// Start new, user created portals at length corresponging to 1% finished
			GameObject portalObj;
			if (portal.timeFinished <= CurrentTime.currentTimeMillis()) {
				// Portal finished
				float xTemp = x2*1.0f;
				float yTemp = y2*1.0f;
				portalObj = createPortal(x1,y1,xTemp,yTemp);
				portalObj.GetComponent<Renderer>().material = portalMaterial;
			}
			else {
				// Portal unfinished
				// Convert x1,x2,y1,y2 from base centers to points on outer edge of base
				float slope = (y2*1.0f-y1)/(x2*1.0f-x1);
				float angle = Mathf.Rad2Deg*Mathf.Atan(slope);
				GameObject b1 = GenerateWorld.instance.getBaseObj("Base" + portal.base1.baseId);
				float rB1 = b1.GetComponent<SphereCollider>().radius;
				GameObject b2 = GenerateWorld.instance.getBaseObj("Base" + portal.base2.baseId);
				float rB2 = b2.GetComponent<SphereCollider>().radius;

				int offset1 = x1 < x2 ? 0 : 180;
				int offset2 = x1 < x2 ? 180 : 0;
				x1 += Mathf.Cos(Mathf.Deg2Rad * (angle + offset1)) * rB1;
				y1 += Mathf.Sin(Mathf.Deg2Rad * (angle + offset1)) * rB1;
				x2 += Mathf.Cos(Mathf.Deg2Rad * (angle + offset2)) * rB2;
				y2 += Mathf.Sin(Mathf.Deg2Rad * (angle + offset2)) * rB2;

				float xTemp = x1 + (x2-x1)*0.01f;
				float yTemp = y1 + (y2-y1)*0.01f;
				portalObj = createPortal(x1,y1,xTemp,yTemp);
				portalObj.GetComponent<Renderer>().material = portalBuildingMaterial;
				currentUnfinishedPortals.Add(portal);
				currentUnfinishedPortalObjs.Add(portalObj);
			}
			portalObj.name = "Portal" + portal.portalId;
			currentPortals[i] = portalObj;
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
				float x1 = p.base1.world.x * 3 + p.base1.local.x;
				float y1 = p.base1.world.y * 3 + p.base1.local.y;
				float x2 = p.base2.world.x * 3 + p.base2.local.x;
				float y2 = p.base2.world.y * 3 + p.base2.local.y;

				float slope = (y2*1.0f-y1)/(x2*1.0f-x1);
				float angle = Mathf.Rad2Deg*Mathf.Atan(slope);
				GameObject b1 = GenerateWorld.instance.getBaseObj("Base" + p.base1.baseId);
				float rB1 = b1.GetComponent<SphereCollider>().radius;
				GameObject b2 = GenerateWorld.instance.getBaseObj("Base" + p.base2.baseId);
				float rB2 = b2.GetComponent<SphereCollider>().radius;

				int offset1 = x1 <= x2 ? 0 : 180;
				int offset2 = x1 <= x2 ? 180 : 0;
				x1 += Mathf.Cos(Mathf.Deg2Rad * (angle + offset1)) * rB1;
				y1 += Mathf.Sin(Mathf.Deg2Rad * (angle + offset1)) * rB1;
				x2 += Mathf.Cos(Mathf.Deg2Rad * (angle + offset2)) * rB2;
				y2 += Mathf.Sin(Mathf.Deg2Rad * (angle + offset2)) * rB2;

				float xTemp = x1 + (x2-x1)*percentFinished;
				float yTemp = y1 + (y2-y1)*percentFinished;
				portalObj.transform.localPosition = new Vector3((x1+xTemp)/2.0f, (y1+yTemp)/2.0f, 0f);
				scalePortalBetweenPoints(portalObj, x1, y1, xTemp, yTemp);
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
		dragToCreateTempPortal = createPortal (x1, y1, x2, y2);
		dragToCreateTempPortal.name = "tempPortal";
	}
	
	public void updateDragPortal(Vector2 pos, bool onBase) {
		float x1 = GenerateWorld.instance.lastBase.world.x * 3 + GenerateWorld.instance.lastBase.local.x;
		float y1 = GenerateWorld.instance.lastBase.world.y * 3 + GenerateWorld.instance.lastBase.local.y;
		float x2 = pos.x;
		float y2 = pos.y;
		
		// Create the portal (cylinder prefab)
		dragToCreateTempPortal.transform.localPosition = new Vector3((x1+x2)/2.0f, (y1+y2)/2.0f, 0f);
		scalePortalBetweenPoints (dragToCreateTempPortal, x1, y1, x2, y2);
		rotatePortalBetweenPoints (dragToCreateTempPortal, x1, y1, x2, y2);
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
		foreach (GameObject p in currentPortals) {
			if (p.name == "Portal" + pId) return p;
		}
		return null;
	}
	
}
