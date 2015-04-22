using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine.UI;

public class UnderAttackHandler : MonoBehaviour {
	public GameObject OIPProgressItemPrefab;
	public Sprite defendSprite;
	
	public static UnderAttackHandler instance;
	
	public List<Attack> currentUnderAttacks { get; set; }

	void Awake () {
		instance = this;
	}
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (currentUnderAttacks != null) {
			List<Attack> toRemove = new List<Attack> ();
			foreach (Attack attack in currentUnderAttacks) {
				attack.lastUpdate = CurrentTime.currentTimeMillis ();
				if (attack.lastUpdate >= attack.timeAttackLands) {
					getAttackDefendingResults(attack);
					// TODO: Delete attack from list
					toRemove.Add(attack);
				}
			}
			foreach (Attack a in toRemove) {
				currentUnderAttacks.Remove(a);
			}
		}
	}
	
	public void loadUnderAttacks() {
		StartCoroutine (coLoadUnderAttacks ());
	}
	
	public IEnumerator coLoadUnderAttacks() {
		WWWForm wwwform = new WWWForm ();
		wwwform.AddField ("username", "kmw8sf");
		WWW request = new WWW ("localhost:8080/myapp/world/attacksdefending", wwwform);
		yield return request;
		currentUnderAttacks = JsonMapper.ToObject<List<Attack>>(request.text);
		foreach(Attack attack in currentUnderAttacks) {
			GameObject progressBar = (GameObject)Instantiate (OIPProgressItemPrefab, new Vector3 (0, 0, 0), Quaternion.identity);
			WormHole w = attack.defenderWormHole;
			progressBar.transform.SetParent (w.objectInfoPanel.transform);
			progressBar.GetComponentInChildren<Image> ().sprite = defendSprite;
			progressBar.SetActive(true);
			attack.lastUpdateEvent += progressBar.GetComponent<OIPProgressScript> ().updateContent;
			w.attackState = AttackState.underAttack;
		}
	}
	
	public void getAttackDefendingResults(Attack attack) {
		StartCoroutine("coGetAttackDefendingResults", attack);
	}
	
	public IEnumerator coGetAttackDefendingResults(Attack attack) {
		Debug.Log ("Attack defending landed");
		WWWForm wwwform = new WWWForm ();
		wwwform.AddField ("username", "kmw8sf");
		wwwform.AddField ("attackId", attack.attackId);
		WWW request = new WWW ("localhost:8080/myapp/world/attacklanded", wwwform);
		yield return request;
		AttackResultObj result = JsonMapper.ToObject<AttackResultObj>(request.text);
		
		// Process results
		if (result.attackId == attack.attackId) {
			processAttackDefendingResults(attack, result);
		} else {
			// try to get results again
			currentUnderAttacks.Add(attack);
		}
	}
	
	private static void processAttackDefendingResults(Attack attack, AttackResultObj result) {
		bool isWinner = result.winnerUsername.Equals(Globals.username);
		Debug.Log ("AttackID: " + attack.attackId + " Won? " + isWinner + " result: " + result);
		
		if (isWinner) {
			Debug.Log ("Survived attack! Surviving units - New num units: " + result.numUnitsLeft);
			// Update num units
			Base b = (Base)ObjectInstanceDictionary.getObjectInstanceById("Base", attack.defenderBaseId);
			GenerateWorld.instance.message.text = "Survived getting attacked! Lost " + (b.units - result.numUnitsLeft) + " units";
			b.units = result.numUnitsLeft;
			EventManager.positionText ();
			
			WormHole w = attack.attackerWormHole;
			w.attackState = AttackState.NoAttack;
			// Remove progress bar
		}
		else {
			GenerateWorld.instance.message.text = "Defeated in attack. Base lost.";
			Debug.Log ("Didn't survive attack - lost base BaseId: " + attack.defenderBaseId);
			// Delete base and connecting portals
			// Get all connecting portals
			foreach (int pId in result.lostPortalIds) {
				GameObject p = ObjectInstanceDictionary.getObjectInstanceById("Portal", pId).gameObject;
				Destroy (p);
			}
			// Delete base's wormholes
			GameObject b = ObjectInstanceDictionary.getObjectInstanceById("Base", attack.defenderBaseId).gameObject;
			Destroy (b.GetComponent<BaseScript>().objectInfoPanel);
			Destroy (b);
			EventManager.positionText ();
		}
	}
}
