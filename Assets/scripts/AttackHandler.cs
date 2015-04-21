using UnityEngine;
using System.Collections;
using LitJson;
using UnityEngine.UI;
using System.Collections.Generic;

public class AttackHandler : MonoBehaviour {
	public GameObject OIPProgressItemPrefab;
	public Sprite attackSprite;

	public static AttackHandler instance;

	public List<Attack> currentAttacks { get; set; }

	void Awake () {
		instance = this;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (currentAttacks != null) {
			List<Attack> toRemove = new List<Attack> ();
			foreach (Attack attack in currentAttacks) {
				attack.lastUpdate = CurrentTime.currentTimeMillis ();
				if (attack.lastUpdate >= attack.timeAttackLands) {
					getAttackResults(attack);
					// TODO: Delete attack from list
					toRemove.Add(attack);
				}
			}
			foreach (Attack a in toRemove) {
				currentAttacks.Remove(a);
			}
		}
	}

	public void loadAttacks() {
		StartCoroutine (coLoadAttacks ());
	}

	public IEnumerator coLoadAttacks() {
		WWWForm wwwform = new WWWForm ();
		wwwform.AddField ("username", "kmw8sf");
		WWW request = new WWW ("localhost:8080/myapp/world/attacks", wwwform);
		yield return request;
		Debug.Log ("/attacks: " + request.text);
		currentAttacks = JsonMapper.ToObject<List<Attack>>(request.text);
		foreach(Attack attack in currentAttacks) {
			createAttackObj(attack);
		}
	}

	public void addAttack(Attack attack) {
		currentAttacks.Add (attack);
		createAttackObj (attack);
	}

	private void createAttackObj(Attack attack) {
		GameObject progressBar = (GameObject)Instantiate (OIPProgressItemPrefab, new Vector3 (0, 0, 0), Quaternion.identity);
		WormHole w = attack.attackerWormHole;
		progressBar.transform.SetParent (w.objectInfoPanel.transform);
		progressBar.GetComponentInChildren<Image> ().sprite = attackSprite;
		progressBar.SetActive (true);
		attack.lastUpdateEvent += progressBar.GetComponent<OIPProgressScript> ().updateContent;
		w.attackState = AttackState.Attacking;
	}
	
	public void getAttackResults(Attack attack) {
		StartCoroutine("coGetAttackResults", attack);
	}
	
	public IEnumerator coGetAttackResults(Attack attack) {
		Debug.Log ("Attack landed");
		WWWForm wwwform = new WWWForm ();
		wwwform.AddField ("username", "kmw8sf");
		wwwform.AddField ("attackId", attack.attackId);
		WWW request = new WWW ("localhost:8080/myapp/world/attacklanded", wwwform);
		yield return request;
		AttackResultObj result = JsonMapper.ToObject<AttackResultObj>(request.text);
		Debug.Log ("ATTACKRESULTS: " + request.text);
		Debug.Log ("ATTACKRESULTS2: " + request.text + "____" + result.attackId);
		// Process results
		if (result.attackId == attack.attackId) {
			processAttackResults(attack, result);
		} else {
			// try to get results again
			Debug.Log ("EMPTY");
			currentAttacks.Add(attack);
		}
	}
	
	private static void processAttackResults(Attack attack, AttackResultObj result) {
		// TODO: Process results (delete/add new base, adjust num units, remove progress bar, restore wormhole color)
		bool isWinner = result.winnerUsername.Equals(Globals.username);
		Debug.Log ("AttackID: " + attack.attackId + " Won? " + isWinner + " result: " + result);
		
		if (isWinner) {
			if (attack.attacker.Equals(Globals.username)) {
				Debug.Log ("My attack successful! Aquired new base - NewBase: " + result.newBase);
				// Draw new base and portal
				GenerateWorld.instance.addBase(result.newBase);
				PortalHandler.instance.addPortal(result.newPortal);
				EventManager.positionText ();
			}
			else {
				Debug.Log ("Survived attack! Surviving units - New num units: " + result.numUnitsLeft);
				// Update num units
				Base b = (Base)ObjectInstanceDictionary.getObjectInstanceById("Base", attack.defenderBaseId);
				b.units = result.numUnitsLeft;
				EventManager.positionText ();
			}
		}
		else {
			if (attack.attacker.Equals(Globals.username)) {
				Debug.Log ("My attack unsuccessful - lost all the troops I sent to battle");
			}
			else {
				Debug.Log ("Didn't survive attack - lost base BaseId: " + attack.defenderBaseId);
				// Delete base and connecting portals
				GameObject b = ObjectInstanceDictionary.getObjectInstanceById("Base", attack.defenderBaseId).gameObject;
				Destroy(b.GetComponent<BaseScript>().objectInfoPanel);
				Destroy (b);
				// Get all connecting portals
				foreach (int pId in result.lostPortalIds) {
					GameObject p = ObjectInstanceDictionary.getObjectInstanceById("Portal", pId).gameObject;
					Destroy (p);
				}
				EventManager.positionText ();
			}
		}
	}
}
