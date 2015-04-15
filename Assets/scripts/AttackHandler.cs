using UnityEngine;
using System.Collections;
using LitJson;
using UnityEngine.UI;

public class AttackHandler : MonoBehaviour {
	public GameObject OIPProgressItemPrefab;
	public Sprite attackSprite;

	public static AttackHandler instance;

	public Attack[] currentAttacks { get; set; }

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
		currentAttacks = JsonMapper.ToObject<Attack[]>(request.text);
		foreach(Attack attack in currentAttacks) {
			GameObject progressBar = (GameObject)Instantiate (OIPProgressItemPrefab, new Vector3 (0, 0, 0), Quaternion.identity);
			WormHole w = attack.attackerWormHole;
			progressBar.transform.SetParent (w.objectInfoPanel.transform);
			progressBar.GetComponentInChildren<Image> ().sprite = attackSprite;
			progressBar.SetActive(true);
			attack.lastUpdateEvent += progressBar.GetComponent<OIPProgressScript> ().updateContent;
			w.attackState = AttackState.Attacking;
		}
	}
}
