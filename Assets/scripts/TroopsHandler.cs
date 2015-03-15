﻿using UnityEngine;
using LitJson;
using System.Collections;
using System.Collections.Generic;

public class TroopsHandler : MonoBehaviour {

	public static TroopsHandler instance;
	public List<MoveTroopsAction> moveTroopsActions;

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
			troopsLeftToMove = troops;
			overflowTroopsMoved = 0.0f;
		}
	}

	// Use this for initialization
	void Start () {
		instance = this;
		moveTroopsActions = new List<MoveTroopsAction> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (moveTroopsActions.Count > 0) {
			List<MoveTroopsAction> toRemove = new List<MoveTroopsAction> ();
			foreach (MoveTroopsAction a in moveTroopsActions) {
				// If more troops to move
				if (a.troopsLeftToMove > 0) {
					// Update values based on Time.DeltaTime
					a.overflowTroopsMoved += Time.deltaTime * a.flowRate;
					int wholeUnitsMoved = (int)Mathf.Floor(a.overflowTroopsMoved);
					if (wholeUnitsMoved > 0) {
						// Update base text wrappers
						a.b1.GetComponent<TouchBase>().b.units -= wholeUnitsMoved;
						a.b2.GetComponent<TouchBase>().b.units += wholeUnitsMoved;
						a.overflowTroopsMoved -= wholeUnitsMoved;
						a.troopsLeftToMove -= wholeUnitsMoved;
					}
				} else {
					// Remove action from list
					toRemove.Add(a);
					// Restore portal color
					a.portal.renderer.material = PortalHandler.instance.portalMaterial;
					// Final db sync
					StartCoroutine("finishMoveTroops", a);
				}
			}
			Camera.main.GetComponent<DisplayInfoHandler> ().updateContent ();
			foreach (MoveTroopsAction p in toRemove) {
				moveTroopsActions.Remove(p);
			}
		}
	}

	public void startMoveTroopsAction(Base b) {
		// Make MoveTroopsAction (find out if valid move here)
		StartCoroutine ("initMoveTroopsAction", b);
	}

	IEnumerator initMoveTroopsAction(Base b) {
		WWWForm wwwform = new WWWForm ();
		wwwform.AddField ("username", b.username);
		wwwform.AddField ("baseId1", GenerateWorld.instance.lastBase.baseId);
		wwwform.AddField ("baseId2", b.baseId);
		int numTroops = GenerateWorld.instance.numTroopsInputField.text.Equals ("") ? 
			1 : int.Parse (GenerateWorld.instance.numTroopsInputField.text);
		wwwform.AddField ("numTroops", numTroops);
		WWW request = new WWW ("localhost:8080/myapp/world/troops/startMove", wwwform);
		yield return request;
		if (request.text.Equals ("")) {
			Debug.Log("Move troops action invalid");
		} else {
			GameObject b1 = GenerateWorld.instance.getBaseObj("Base" + GenerateWorld.instance.lastBase.baseId);
			GameObject b2 = GenerateWorld.instance.getBaseObj("Base" + b.baseId);
			Portal2 p = JsonMapper.ToObject<Portal2>(request.text);
			GameObject pObj = PortalHandler.instance.getFinishedPortalObj (p.portalId);
			pObj.renderer.material = PortalHandler.instance.portalBuildingMaterial;
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
			pObj.renderer.material = PortalHandler.instance.portalBuildingMaterial;
			GameObject base1 = GenerateWorld.instance.getBaseObj("Base" + p.base1.baseId);
			GameObject base2 = GenerateWorld.instance.getBaseObj("Base" + p.base2.baseId);
			MoveTroopsAction a = new MoveTroopsAction (base1, base2, p.base1.username, pObj, p.portalId, p.flowRate, p.troopsToMove);

			// Update values based on Time.DeltaTime
			a.overflowTroopsMoved += ((CurrentTime.currentTimeMillis() - p.lastMoveUpdate) / 1000) * a.flowRate;
			int wholeUnitsMoved = (int)Mathf.Floor(a.overflowTroopsMoved);
			if (wholeUnitsMoved > a.troopsLeftToMove) {
				wholeUnitsMoved = a.troopsLeftToMove;
			}
			if (wholeUnitsMoved > 0) {
				// Update base text wrappers
				a.b1.GetComponent<TouchBase>().b.units -= wholeUnitsMoved;
				a.b2.GetComponent<TouchBase>().b.units += wholeUnitsMoved;
				a.overflowTroopsMoved -= wholeUnitsMoved;
				a.troopsLeftToMove -= wholeUnitsMoved;
			}

			moveTroopsActions.Add (a);
		}
	}
}