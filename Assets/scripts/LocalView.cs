using UnityEngine;
using System.Collections;

public class LocalView : MonoBehaviour {
	public float moveSpeed = 3;
	public float rotateSmooth = 2;
	public GameObject lookAtBase;

	private Vector3 initialPosition;
	private Quaternion initialRotation;

	private Vector3 endPosition;
	private Quaternion endRotation;

	private float startTime;
	private float translateDistance;

	private Vector3 targetRotatePosition;


	// Use this for initialization
	void Start() 
	{
		Debug.Log (lookAtBase.GetComponent<SphereCollider> ().radius);
		initialPosition = Camera.main.transform.position;
		initialRotation = Camera.main.transform.rotation;
		endPosition = lookAtBase.transform.position + new Vector3(0, 0, -.8f);
		// end rotation: 0, 300, 90

		startTime = Time.time;
		translateDistance = Vector3.Distance (initialPosition, endPosition);
		targetRotatePosition = new Vector3 (-1, 0, .8f);
	}

	// Update is called once per frame
	void Update () {
		var distCovered = (Time.time - startTime) * moveSpeed;
		var fracJourney = distCovered / translateDistance;
		Camera.main.transform.position = Vector3.Slerp (initialPosition, endPosition, fracJourney);

		Vector3 relativePos = targetRotatePosition - transform.position;
		Quaternion rotation = Quaternion.LookRotation (relativePos, new Vector3(0, 0, -1));
		transform.localRotation = Quaternion.Slerp (transform.localRotation, rotation, Time.deltaTime*rotateSmooth);
	}
}
