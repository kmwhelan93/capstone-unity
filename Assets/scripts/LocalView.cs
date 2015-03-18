using UnityEngine;
using System.Collections;

public class LocalView : MonoBehaviour {
	public float moveSpeed = 3;
	public float rotateSmooth = 2;
	public GameObject worldCamera;

	private GameObject lookAtWorld;

	private Vector3 initialPosition;
	private Vector3 endPosition;
	private Vector3 empireViewPosition;
	private Quaternion empireViewRotation = new Quaternion (0, 0, 0, 1);

	private float startTime;
	private float translateDistance;

	private Vector3 targetRotatePosition;

	private bool inProgress = false;
	private bool isFirstSwitchToLocalView = true;
	private GameObject currentWorld;

	public void switchToEmpireView()
	{
		Globals.isInLocalView = false;
		initialPosition = transform.position;
		startTime = Time.time;
		endPosition = empireViewPosition;
		translateDistance = Vector3.Distance (initialPosition, empireViewPosition);
		inProgress = true;
	}

	public void switchToLocalView(GameObject world)
	{
		// Save where the empire view was last if this is our first movement away from empire view
		if (!Globals.isInLocalView) {
			empireViewPosition = transform.position;
			isFirstSwitchToLocalView = true;
			targetRotatePosition = new Vector3(0, 0, 0);
			if (world.GetComponent<TouchBase>().b.world.Equals (new Point(0, 0))) {
				targetRotatePosition = new Vector3(0, 2, 0);
			}
		} else {
			isFirstSwitchToLocalView = false;
			if (!worldCamera.GetComponent<PortalHandler>().areConnected(currentWorld, world)) {
				GenerateWorld.instance.message.text = "You can only move along bases connected by portals";
				return;
			}
			targetRotatePosition = (world.transform.position - transform.position).normalized*10 + world.transform.position;
		}
		currentWorld = world;
		Globals.isInLocalView = true;
		// hide the displayInfo text
		GetComponent<DisplayInfoHandler> ().positionText ();
		initialPosition = transform.position;
		lookAtWorld = world;

		Vector3 finalDirection = (targetRotatePosition - lookAtWorld.transform.position).normalized;
		endPosition = lookAtWorld.transform.position + new Vector3(0, 0, -.62f) - finalDirection / 2;


		// end rotation: 0, 300, 90
		startTime = Time.time;
		translateDistance = Vector3.Distance (initialPosition, endPosition);

		inProgress = true;
	}

	void Update () {
		if (inProgress) {
			var distCovered = (Time.time - startTime) * moveSpeed;
			var fracJourney = distCovered / translateDistance;
			if (isFirstSwitchToLocalView) {
				Camera.main.transform.position = Vector3.Slerp (initialPosition, endPosition, fracJourney);
			} else {
				Camera.main.transform.position = Vector3.Lerp (initialPosition, endPosition, fracJourney);
			}
			if (Globals.isInLocalView) {
				Vector3 relativePos = targetRotatePosition - transform.position;
				Quaternion rotation = Quaternion.LookRotation (relativePos, new Vector3 (0, 0, -1));
				transform.localRotation = Quaternion.Slerp (transform.localRotation, rotation, Time.deltaTime * rotateSmooth);
			} else {
				transform.localRotation = Quaternion.Slerp (transform.localRotation, empireViewRotation, Time.deltaTime * rotateSmooth);
				Debug.Log (fracJourney);
				if (fracJourney > .999) {
					GetComponent<DisplayInfoHandler>().positionText();
					inProgress = false;
				}
			}
		}
	}
}
