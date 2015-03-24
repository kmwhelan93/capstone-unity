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
}
