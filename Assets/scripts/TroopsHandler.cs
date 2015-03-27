using UnityEngine;
using LitJson;
using System.Collections;
using System.Collections.Generic;

public class TroopsHandler : MonoBehaviour {
	public GameObject ProgressBarPrefab;
	public static TroopsHandler instance;
	public List<MoveTroopsAction> moveTroopsActions;
	public List<AddTroopsAction> addTroopsActions;
	public int costPerUnit;
	
	public class MoveTroopsAction {
		public GameObject b1 { get; set; }
		public GameObject b2 { get; set; }
		public string username { get; set; }
		public GameObject portal { get; set; }
		public int portalId { get; set; }
		public int flowRate { get; set; }
		public int troopsLeftToMove { get; set; }
		public float overflowTroopsMoved { get; set; }
		
		public MoveTroopsAction(GameObject base1, GameObject base2, string username_,
		                        GameObject p, int pId, int flow, int troops) {
			b1 = base1;
			b2 = base2;
			username = username_;
			portal = p;
			portalId = pId;
			flowRate = flow;
			// Sign of troopsLeftToMove (here and in the Portals table) reflects direction troops
			// are being moved through portal
			troopsLeftToMove = troops;
			overflowTroopsMoved = 0.0f;
		}
	}
	
	public class AddTroopsAction {
		public Base b { get; set; }
		public GameObject bObj { get; set; }
		public float overflowTroopsAdded { get; set; }
		
		public AddTroopsAction(Base b1, GameObject bObject) {
			b = b1;
			bObj = bObject;
			overflowTroopsAdded = 0f;
		}
	}
	
	// Use this for initialization
	void Start () {
		instance = this;
		moveTroopsActions = new List<MoveTroopsAction> ();
		addTroopsActions = new List<AddTroopsAction>();
	}
	
	// Update is called once per frame
	void Update () {
		updateMoveTroops();
		updateAddTroops();
	}
	
	public void updateMoveTroops() {
		if (moveTroopsActions.Count > 0) {
			List<MoveTroopsAction> toRemove = new List<MoveTroopsAction> ();
			foreach (MoveTroopsAction a in moveTroopsActions) {
				// If more troops to move
				if (Mathf.Abs(a.troopsLeftToMove) > 0) {
					// Update values based on Time.DeltaTime
					a.overflowTroopsMoved += Time.deltaTime * a.flowRate;
					int wholeUnitsMoved = (int)Mathf.Floor(a.overflowTroopsMoved);
					if (wholeUnitsMoved > 0) {
						if (wholeUnitsMoved > Mathf.Abs(a.troopsLeftToMove)) { wholeUnitsMoved = Mathf.Abs(a.troopsLeftToMove); }
						if (a.troopsLeftToMove < 0) { wholeUnitsMoved *= -1; }
						// Update base text wrappers
						a.b1.GetComponent<TouchBase>().b.units -= wholeUnitsMoved;
						a.b2.GetComponent<TouchBase>().b.units += wholeUnitsMoved;
						a.overflowTroopsMoved -= Mathf.Abs(wholeUnitsMoved);
						a.troopsLeftToMove -= wholeUnitsMoved;
					}
				} else {
					// Remove action from list
					toRemove.Add(a);
					// Restore portal color
					a.portal.GetComponent<Renderer>().material = PortalHandler.instance.portalMaterial;
					// Final db sync
					StartCoroutine("finishMoveTroops", a);
				}
			}
			foreach (MoveTroopsAction p in toRemove) {
				moveTroopsActions.Remove(p);
			}
		}
	}
	
	public void updateAddTroops() {
		if (addTroopsActions.Count > 0) {
			List<AddTroopsAction> toRemove = new List<AddTroopsAction> ();
			foreach (AddTroopsAction a in addTroopsActions) {
				// If more troops to move
				if (Mathf.Abs(a.b.unitsToAdd) > 0) {
					// Update values based on Time.DeltaTime
					//print ("*: " + Time.deltaTime * Globals.timeCostPerTroop);
					a.overflowTroopsAdded += Time.deltaTime * Globals.timeCostPerTroop;
					int wholeUnitsAdded = (int)Mathf.Floor(a.overflowTroopsAdded);
					if (wholeUnitsAdded > 0) {
						// Update base text wrappers
						a.bObj.GetComponent<TouchBase>().b.units += wholeUnitsAdded;
						a.overflowTroopsAdded -= Mathf.Abs(wholeUnitsAdded);
						a.b.unitsToAdd -= wholeUnitsAdded;
					}
				} else {
					// Remove action from list
					toRemove.Add(a);
					// Restore base color (will be added later)
					// Final db sync
					StartCoroutine("finishAddTroops", a);
				}
			}
			foreach (AddTroopsAction p in toRemove) {
				addTroopsActions.Remove(p);
			}
		}
	}
	
	public void startMoveTroopsAction(Base b, int numTroops) {
		StartCoroutine (initMoveTroopsAction(b, numTroops));
	}
	
	IEnumerator initMoveTroopsAction(Base b, int numTroops) {
		WWWForm wwwform = new WWWForm ();
		wwwform.AddField ("username", b.username);
		wwwform.AddField ("baseId1", GenerateWorld.instance.lastBase.baseId);
		wwwform.AddField ("baseId2", b.baseId);
		wwwform.AddField ("numTroops", numTroops);
		WWW request = new WWW ("localhost:8080/myapp/world/troops/startMove", wwwform);
		yield return request;
		if (request.text.Equals ("")) {
			Debug.Log("Move troops action invalid");
		} else {
			Portal2 p = JsonMapper.ToObject<Portal2>(request.text);
			GameObject b1 = GenerateWorld.instance.getBaseObj("Base" + p.base1.baseId);
			GameObject b2 = GenerateWorld.instance.getBaseObj("Base" + p.base2.baseId);
			GameObject pObj = PortalHandler.instance.getFinishedPortalObj (p.portalId);
			pObj.GetComponent<Renderer>().material = PortalHandler.instance.portalBuildingMaterial;
			if (p.base1.baseId == b.baseId) { numTroops *= -1; }
			MoveTroopsAction a = new MoveTroopsAction (b1, b2, b.username, pObj, p.portalId, p.flowRate, numTroops);
			moveTroopsActions.Add (a);
		}
	}
	
	IEnumerator finishMoveTroops(MoveTroopsAction a) {
		WWWForm wwwform = new WWWForm ();
		wwwform.AddField ("username", a.username);
		wwwform.AddField ("portalId", a.portalId);
		WWW request = new WWW ("localhost:8080/myapp/world/troops/finishMove", wwwform);
		yield return request;
		if (request.text.Equals ("")) {
			Debug.Log ("Finish move troops failed");
		} else {
			// Display finish message
			GenerateWorld.instance.message.text = request.text;
		}
	}
	
	public void restartMoveTroops() {
		// START ANY MOVES CURRENTLY IN PROGRESS - has to be done after portals are created, so this is called in PortalHandler
		StartCoroutine ("restartMoveTroopsActions");
	}
	
	IEnumerator restartMoveTroopsActions() {
		WWWForm wwwform = new WWWForm ();
		wwwform.AddField ("username", Globals.username);
		WWW request = new WWW ("localhost:8080/myapp/world/troops/restartMove", wwwform);
		yield return request;
		Portal2[] portals = JsonMapper.ToObject<Portal2[]>(request.text);
		foreach (Portal2 p in portals) {
			GameObject pObj = PortalHandler.instance.getFinishedPortalObj (p.portalId);
			pObj.GetComponent<Renderer>().material = PortalHandler.instance.portalBuildingMaterial;
			GameObject base1 = GenerateWorld.instance.getBaseObj("Base" + p.base1.baseId);
			GameObject base2 = GenerateWorld.instance.getBaseObj("Base" + p.base2.baseId);
			MoveTroopsAction a = new MoveTroopsAction (base1, base2, p.base1.username, pObj, p.portalId, p.flowRate, p.troopsToMove);
			
			// Update values based on Time.DeltaTime
			a.overflowTroopsMoved += ((CurrentTime.currentTimeMillis() - p.lastMoveUpdate) / 1000) * a.flowRate;
			int wholeUnitsMoved = (int)Mathf.Floor(a.overflowTroopsMoved);
			if (wholeUnitsMoved > Mathf.Abs(a.troopsLeftToMove)) {
				wholeUnitsMoved = Mathf.Abs(a.troopsLeftToMove);
			}
			if (wholeUnitsMoved > 0) {
				if (a.troopsLeftToMove < 0) { wholeUnitsMoved *= -1; }
				// Update base text wrappers
				a.b1.GetComponent<TouchBase>().b.units -= wholeUnitsMoved;
				a.b2.GetComponent<TouchBase>().b.units += wholeUnitsMoved;
				a.overflowTroopsMoved -= Mathf.Abs(wholeUnitsMoved);
				a.troopsLeftToMove -= wholeUnitsMoved;
			}
			
			moveTroopsActions.Add (a);
		}
	}
	
	public void onCheckButtonClickAddTroops() {
		if (Globals.opState == OpState.AddTroops) {
			addTroops (GenerateWorld.instance.lastBase, (int)GenerateWorld.instance.slider.value);
		} else if (Globals.opState == OpState.MoveTroops) {
			instance.startMoveTroopsAction (GenerateWorld.instance.secondBase, (int)GenerateWorld.instance.slider.value);
		}
		GenerateWorld.instance.sliderValue.text = "";
		GenerateWorld.instance.sliderObject.SetActive(false);
		GenerateWorld.instance.sliderConfirmButton.SetActive(false);
	}

	public void addTroops(Base b, int numTroops) {
		StartCoroutine (initBuyTroops(b, numTroops));
	}

	IEnumerator initBuyTroops(Base b, int numTroops) {
		WWWForm wwwform = new WWWForm ();
		wwwform.AddField ("username", Globals.username);
		wwwform.AddField ("baseId", b.baseId);
		wwwform.AddField ("numTroops", numTroops);
		wwwform.AddField ("costPerTroop", costPerUnit);
		print ("nt: " + numTroops);
		WWW request = new WWW ("localhost:8080/myapp/world/troops/startBuy", wwwform);
		yield return request;
		GameObject b1 = GenerateWorld.instance.getBaseObj("Base" + b.baseId);

		// create progress bar ui
		GameObject progressBar = (GameObject)Instantiate (ProgressBarPrefab, new Vector3 (0, 0, 0), Quaternion.identity);
		progressBar.transform.SetParent (b.objectInfoPanel.transform);


		b.unitsToAdd = numTroops;
		addTroopsActions.Add(new AddTroopsAction(b, b1));
		// database gold entry undated with troops/buy request, this syncs frontend to new value
		UpdateGold.instance.syncGold ();
		DisplayTransactionHandler.instance.setCostText(numTroops * costPerUnit);
	}
	
	IEnumerator finishAddTroops(AddTroopsAction a) {
		WWWForm wwwform = new WWWForm ();
		wwwform.AddField ("username", Globals.username);
		wwwform.AddField ("baseId", a.b.baseId);
		WWW request = new WWW ("localhost:8080/myapp/world/troops/finishBuy", wwwform);
		yield return request;	
	}
	
	public void restartAddTroops() {
		// START ANY ADDS CURRENTLY IN PROGRESS - has to be done after portals are created, so this is called in PortalHandler
		StartCoroutine ("restartAddTroopsActions");
	}
	
	IEnumerator restartAddTroopsActions() {
		WWWForm wwwform = new WWWForm ();
		wwwform.AddField ("username", Globals.username);
		WWW request = new WWW ("localhost:8080/myapp/world/troops/restartAdd", wwwform);
		yield return request;
		Base[] bases = JsonMapper.ToObject<Base[]>(request.text);
		foreach (Base b in bases) {
			GameObject b1 = GenerateWorld.instance.getBaseObj("Base" + b.baseId);
			AddTroopsAction a = new AddTroopsAction(b, b1);		
			// Update values based on Time.DeltaTime
			a.overflowTroopsAdded += ((CurrentTime.currentTimeMillis() - b.lastUpdated) / 1000) * Globals.timeCostPerTroop;
			int wholeUnitsAdded = (int)Mathf.Floor(a.overflowTroopsAdded);;
			if (wholeUnitsAdded > Mathf.Abs(a.b.unitsToAdd)) {
				wholeUnitsAdded = Mathf.Abs(a.b.unitsToAdd);
			}
			if (wholeUnitsAdded > 0) {
				// Update base text wrappers
				a.bObj.GetComponent<TouchBase>().b.units += wholeUnitsAdded;
				a.overflowTroopsAdded -= Mathf.Abs(wholeUnitsAdded);
				a.b.unitsToAdd -= wholeUnitsAdded;
			}
			
			addTroopsActions.Add (a);
		}
	}
	
	public void displaySliderValue(float f) {
		GenerateWorld.instance.sliderValue.text = (int)f + "";
	}
}
