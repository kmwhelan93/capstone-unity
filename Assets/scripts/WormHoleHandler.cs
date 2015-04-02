using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using LitJson;

public class WormHoleHandler : MonoBehaviour {
	public GameObject prefab;

	public static WormHoleHandler instance;

	public WormHole[] currentWormHoles { get; set; }
	public GameObject[] currentWormHoleObjects { get; set; }
	// Use this for initialization
	void Awake () {
		instance = this;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void loadWormHoles()
	{
		StartCoroutine (coLoadWormHoles ());
	}

	public IEnumerator coLoadWormHoles()
	{
		WWWForm wwwform = new WWWForm ();
		wwwform.AddField ("username", "kmw8sf");
		WWW request = new WWW ("localhost:8080/myapp/world/wormholes", wwwform);
		yield return request;
		WormHole[] wormholes = JsonMapper.ToObject<WormHole[]>(request.text);
		destroyWormHoleObjects ();
		currentWormHoleObjects = new GameObject[wormholes.Length];

		for (int i = 0; i < wormholes.Length; i++) {
			GameObject wormholeObj = (GameObject) Instantiate(prefab, wormholes[i].getWorldCoords(), new Quaternion());
			currentWormHoleObjects[i] = wormholeObj;
			wormholeObj.name = "WormHole" + wormholes[i].wormholeId;
			ObjectInstanceDictionary.registerGameObject(wormholeObj.name, wormholeObj);
			wormholes[i].gameObject = wormholeObj;
			angleTowardsBase (wormholeObj, wormholes[i].b.getGameObject());
		}
	}

	public void angleTowardsBase(GameObject wormhole, GameObject baseObj) 
	{
		Vector3 topMiddleOfBase = baseObj.transform.position + new Vector3 (0, 0, -Globals.baseRadius);
		Vector3 direction = topMiddleOfBase - wormhole.transform.position;
		wormhole.transform.rotation = Quaternion.LookRotation(direction);
	}

	public void destroyWormHoleObjects() 
	{
		if (currentWormHoleObjects == null)
			return;
		foreach (GameObject go in currentWormHoleObjects) {
			Destroy (go);
		}
	}

}
