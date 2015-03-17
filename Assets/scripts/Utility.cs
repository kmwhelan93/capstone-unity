using UnityEngine;
using System.Collections;

public class Utility {
	public static bool almostEqual(Vector3 first, Vector3 second)
	{
		return Mathf.Approximately (first.x, second.x) && Mathf.Approximately (first.y, second.y) && Mathf.Approximately (first.z, second.z);
	}

}
