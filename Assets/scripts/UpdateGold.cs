using UnityEngine;
using System.Collections;

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
			yield break;
		}
}
