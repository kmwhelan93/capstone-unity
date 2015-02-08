using UnityEngine;
using System.Collections;

public class TouchBase : MonoBehaviour {

	public Base b;

	void OnMouseDown() 
	{
		Debug.Log ("touched base at " + b.world.x + " " + b.world.y);
	}
}
