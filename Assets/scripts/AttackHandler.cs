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
			foreach (Attack attack in currentAttacks) {
				attack.lastUpdate = CurrentTime.currentTimeMillis ();
				if (attack.lastUpdate >= attack.timeAttackLands) {
					getAttackResults(attack);
				}
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
		// Process results
		bool isWinner = result.winnerUsername.Equals(Globals.username);
		//Debug.Log ("AttackID: " + attack.attackId + " Won? " + isWinner);
		if (isWinner) {
			
		} else {
		
		}
	}
}
