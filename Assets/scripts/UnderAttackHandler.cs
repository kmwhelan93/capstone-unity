using UnityEngine;
using System.Collections;
using LitJson;
using UnityEngine.UI;

public class UnderAttackHandler : MonoBehaviour {
	public GameObject OIPProgressItemPrefab;
	public Sprite defendSprite;
	
	public static UnderAttackHandler instance;
	
	public Attack[] currentUnderAttacks { get; set; }

	void Awake () {
		instance = this;
	}
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (currentUnderAttacks != null) {
			foreach (Attack attack in currentUnderAttacks) {
				attack.lastUpdate = CurrentTime.currentTimeMillis ();
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
		currentUnderAttacks = JsonMapper.ToObject<Attack[]>(request.text);
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
}
