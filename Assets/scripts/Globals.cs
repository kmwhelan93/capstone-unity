using UnityEngine;
using System.Collections;

public class Globals {
	public static OpState opState { get; set; }
	public static double gold { get; set; }
	public static double goldPerSec { get; set; }
	public static double portalBuildTimeInMins { get; set; }
	public static bool isInLocalView = false;
	// Update this to pull username from session eventually
	public static string username = "kmw8sf";
	public static float baseRadius = .5f;
	public static float timeCostPerTroop = 0.5f; // in seconds
	
	// This is actually extremely important. It is false when you're doing to first click
	// of a two click operation and true when you are on the second click.
	private static bool _secondClick = false;
	public static bool secondClick {
		get {
			return _secondClick;
		}
		set {
			_secondClick = value;
			Debug.Log ("secondClick: " + _secondClick);
		}
	}
}
