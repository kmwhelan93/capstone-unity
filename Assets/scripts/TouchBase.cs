using UnityEngine;
using System.Collections;

public class TouchBase : MonoBehaviour {

	public Base b;

	void OnMouseDown() 
	{
		StartCoroutine ("createBase");
	}

	IEnumerator createBase() 
	{
		WWW request = RequestService.makeRequest("world/bases/create", b);
		yield return request;
		GenerateWorld.instance.resetWorldView ();
	}
}
