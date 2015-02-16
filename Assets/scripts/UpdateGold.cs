using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UpdateGold : MonoBehaviour {

	public GameObject goldText;

	// Use this for initialization
	void Start () {
		StartCoroutine (syncGold ());
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public IEnumerator syncGold()
	{
		WWW request = new WWW (RequestService.baseUrl + "sync/gold");
		yield return request;
		int gold = int.Parse (request.text);
		goldText.GetComponent<Text> ().text = gold + "";
	}
}
