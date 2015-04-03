using UnityEngine;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TroopsHandler : MonoBehaviour {
	public static TroopsHandler instance;
	public List<MoveTroopsAction> moveTroopsActions;
	public List<AddTroopsAction> addTroopsActions;
	public int costPerUnit;
	
	public class MoveTroopsAction {
		public string username { get; set; }
		public int pId { get; set; }
		public Portal2 p {
			get {
				return (Portal2) ObjectInstanceDictionary.getObjectInstanceById("Portal", pId);
			}
		}
		public float overflowTroopsMoved { get; set; }
		
		public MoveTroopsAction(string username_, int portalId) {
			username = username_;
			pId = portalId;
			// Sign of troopsLeftToMove (here and in the Portals table) reflects direction troops
			// are being moved through portal
			overflowTroopsMoved = 0.0f;
		}
	}
	
	public class AddTroopsAction {
		public int bId { get; set; }
		public Base b {
			get {
				return (Base) ObjectInstanceDictionary.getObjectInstanceById("Base", bId);
			}
		}

		public float overflowTroopsAdded { get; set; }
		
		public AddTroopsAction(int base1) {
			bId = base1;
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
				Portal2 p = a.p;
				// If more troops to move
				if (Mathf.Abs(p.troopsToMove) > 0) {
					// Update values based on Time.DeltaTime
					a.overflowTroopsMoved += Time.deltaTime * p.flowRate;
					int wholeUnitsMoved = (int)Mathf.Floor(a.overflowTroopsMoved);
					if (wholeUnitsMoved > 0) {
						if (wholeUnitsMoved > Mathf.Abs(p.troopsToMove)) { wholeUnitsMoved = Mathf.Abs(p.troopsToMove); }
						if (p.troopsToMove < 0) { wholeUnitsMoved *= -1; }
						// Update base text wrappers
						a.p.base1.units -= wholeUnitsMoved;
						a.p.base2.units += wholeUnitsMoved;
						a.overflowTroopsMoved -= Mathf.Abs(wholeUnitsMoved);
						p.troopsToMove -= wholeUnitsMoved;
					}
				} else {
					// Remove action from list
					toRemove.Add(a);
					// Restore portal color
					a.p.gameObject.GetComponent<Renderer>().material = PortalHandler.instance.portalMaterial;
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
						a.b.units += wholeUnitsAdded;
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
			MoveTroopsCommand mtc = JsonMapper.ToObject<MoveTroopsCommand>(request.text);
			Debug.Log("mtc: " + mtc.portalId + " " + mtc.troopsToMove);
			mtc.portal.gameObject.GetComponent<Renderer>().material = PortalHandler.instance.portalBuildingMaterial;
			if (mtc.portal.base1.baseId == b.baseId) { mtc.troopsToMove *= -1; }
			mtc.portal.troopsToMove += mtc.troopsToMove;
			MoveTroopsAction a = new MoveTroopsAction (b.username, mtc.portalId);
			moveTroopsActions.Add (a);
		}
	}
	
	IEnumerator finishMoveTroops(MoveTroopsAction a) {
		WWWForm wwwform = new WWWForm ();
		wwwform.AddField ("username", a.username);
		wwwform.AddField ("portalId", a.p.portalId);
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
		MoveTroopsCommand[] mtcs = JsonMapper.ToObject<MoveTroopsCommand[]>(request.text);
		foreach (MoveTroopsCommand mtc in mtcs) {
			// need to do this so we get the portal that belongs to the portalObj
			mtc.portal.troopsToMove += mtc.troopsToMove;
			mtc.portal.gameObject.GetComponent<Renderer>().material = PortalHandler.instance.portalBuildingMaterial;
			MoveTroopsAction a = new MoveTroopsAction (Globals.username, mtc.portal.portalId);
			
			// Update values based on Time.DeltaTime
			a.overflowTroopsMoved += ((CurrentTime.currentTimeMillis() - mtc.portal.lastMoveUpdate) / 1000) * 
				mtc.portal.flowRate;
			int wholeUnitsMoved = (int)Mathf.Floor(a.overflowTroopsMoved);
			if (wholeUnitsMoved > Mathf.Abs(mtc.portal.troopsToMove)) {
				wholeUnitsMoved = Mathf.Abs(mtc.portal.troopsToMove);
			}
			if (wholeUnitsMoved > 0) {
				if (mtc.portal.troopsToMove < 0) { wholeUnitsMoved *= -1; }
				// Update base text wrappers
				a.p.base1.units -= wholeUnitsMoved;
				a.p.base2.units += wholeUnitsMoved;
				a.overflowTroopsMoved -= Mathf.Abs(wholeUnitsMoved);
				mtc.portal.troopsToMove -= wholeUnitsMoved;
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

		b.unitsToAdd += numTroops;
		addTroopsActions.Add(new AddTroopsAction(b.baseId));
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
		AddTroopsCommand[] atcs = JsonMapper.ToObject<AddTroopsCommand[]>(request.text);
		foreach (AddTroopsCommand atc in atcs) {
			AddTroopsAction a = new AddTroopsAction(atc.bId);		
			// Update values based on Time.DeltaTime
			a.overflowTroopsAdded += ((CurrentTime.currentTimeMillis() - atc.b.lastUpdated) / 1000) * Globals.timeCostPerTroop;
			int wholeUnitsAdded = (int)Mathf.Floor(a.overflowTroopsAdded);;
			if (wholeUnitsAdded > Mathf.Abs(a.b.unitsToAdd)) {
				wholeUnitsAdded = Mathf.Abs(a.b.unitsToAdd);
			}
			if (wholeUnitsAdded > 0) {
				// Update base text wrappers
				a.b.units += wholeUnitsAdded;
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
