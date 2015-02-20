using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using LitJson;

public class UpdateGold : MonoBehaviour {

	public static UpdateGold instance;
	public GameObject goldText;

	// Use this for initialization
	void Start () {
		instance = this;
		syncGold ();
	}
	
	// Update is called once per frame
	void Update () {
		Globals.gold = Globals.gold + Time.deltaTime * Globals.goldPerSec;
		displayGold ();
	}


	public void syncGold()
	{
		StartCoroutine (syncGoldCo ());
	}
	private IEnumerator syncGoldCo()
	{
		WWW request = new WWW (RequestService.baseUrl + "sync/gold");
		yield return request;
		GoldSync goldSync = JsonMapper.ToObject<GoldSync>(request.text);
		Globals.gold = goldSync.gold;
		Globals.goldPerSec = goldSync.goldPerSec;
		Debug.Log (Globals.goldPerSec);
		displayGold ();
	}

	public void displayGold()
	{
		goldText.GetComponent<Text> ().text = (int)Globals.gold + "";
	}

	public class GoldSync {
		public float gold { get; set; }
		public float goldPerSec { get; set; }

	}
}
