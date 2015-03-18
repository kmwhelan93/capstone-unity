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

	public float rotateAngle { get; set; }

	private bool inProgress = false;
	private bool isFirstSwitchToLocalView = true;
	public GameObject currentWorld { get; set; }
	public float distanceToLook { get; set; }

	public bool rotateQuickly { get; set; }

	public void switchToEmpireView()
	{
		Globals.isInLocalView = false;
		initialPosition = transform.position;
		startTime = Time.time;
		endPosition = empireViewPosition;
		translateDistance = Vector3.Distance (initialPosition, empireViewPosition);
		inProgress = true;
		rotateQuickly = false;
	}

	public void switchToLocalView(GameObject world)
	{
		// Save where the empire view was last if this is our first movement away from empire view
		if (!Globals.isInLocalView) {
			empireViewPosition = transform.position;
			isFirstSwitchToLocalView = true;

			rotateAngle = Utility.VectorToAngle(new Vector3(0, 0,0 ) - world.transform.position);
			distanceToLook = world.transform.position.magnitude;
			if (world.GetComponent<TouchBase>().b.world.Equals (new Point(0, 0))) {
				rotateAngle = 90;
				distanceToLook = 4;
			}
		} else {
			isFirstSwitchToLocalView = false;
			if (!worldCamera.GetComponent<PortalHandler>().areConnected(currentWorld, world)) {
				GenerateWorld.instance.message.text = "You can only move along bases connected by portals";
				return;
			}
			rotateAngle = Utility.VectorToAngle(world.transform.position - transform.position);
			distanceToLook = 4;
		}
		currentWorld = world;
		Globals.isInLocalView = true;
		// hide the displayInfo text
		GetComponent<DisplayInfoHandler> ().positionText ();
		initialPosition = transform.position;
		lookAtWorld = world;

		moveToNewEndPosition (rotateAngle);
		inProgress = true;
		rotateQuickly = false;
	}

	public void moveToNewEndPosition(float angle)
	{
		initialPosition = transform.position;
		endPosition = getEndPosition(angle);
		// end rotation: 0, 300, 90
		startTime = Time.time;
		translateDistance = Vector3.Distance (initialPosition, endPosition);
	}

	public Vector3 getEndPosition(float angle)
	{
		Vector3 finalDirection = Utility.angleToVector(rotateAngle);
		// offset back for now, may need to change this
		return lookAtWorld.transform.position + new Vector3(0, 0, -.8f) - finalDirection*.8f;
	}

	void Update () {
		if (inProgress) {
			var distCovered = (Time.time - startTime) * moveSpeed;
			var fracJourney = distCovered / translateDistance;
			if (isFirstSwitchToLocalView) {
				Camera.main.transform.position = Vector3.Slerp (initialPosition, endPosition, rotateQuickly ? 1 : fracJourney);
			} else {
				Camera.main.transform.position = Vector3.Lerp (initialPosition, endPosition, rotateQuickly ? 1 : fracJourney);
			}
			if (Globals.isInLocalView) {
				Vector3 relativePos = currentWorld.transform.position + Utility.angleToVector(rotateAngle)*distanceToLook - transform.position;
				Quaternion rotation = Quaternion.LookRotation (relativePos, new Vector3 (0, 0, -1));
				if (Utility.almostEqual(rotation, transform.rotation) && fracJourney > .999f) {
					// close to destination
				}
				transform.localRotation = Quaternion.Slerp (transform.localRotation, rotation, Time.deltaTime * (rotateQuickly ? 10000 : rotateSmooth));
			} else {
				transform.localRotation = Quaternion.Slerp (transform.localRotation, empireViewRotation, Time.deltaTime * rotateSmooth);
				if (fracJourney > .999) {
					GetComponent<DisplayInfoHandler>().positionText();
					inProgress = false;
				}
			}
		}
	}
}
