using UnityEngine;
using System.Collections;
using LitJson;

public class AttackHandler : MonoBehaviour {

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
		Debug.Log (currentAttacks [0]);
	}
}
