using UnityEngine;
using System.Collections;

public class AngleTowardsBase : MonoBehaviour {
	public GameObject baseObj;
	// Use this for initialization
	void Start () {
		Vector3 topMiddleOfBase = baseObj.transform.position + new Vector3 (0, 0, -Globals.baseRadius);
		Vector3 direction = topMiddleOfBase - transform.position;
		transform.rotation = Quaternion.LookRotation(direction);
	}
}
