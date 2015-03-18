using UnityEngine;
using System.Collections;

public class Utility {
	private static float errorMargin = .01f;
	public static bool almostEqual(Vector3 first, Vector3 second)
	{
		return Mathf.Approximately (first.x, second.x) && Mathf.Approximately (first.y, second.y) && Mathf.Approximately (first.z, second.z);
	}

	public static bool almostEqual(Quaternion first, Quaternion second) {
		return Mathf.Abs (first.x - second.x) < errorMargin && Mathf.Abs (first.y - second.y) < errorMargin && Mathf.Abs (first.z - second.z) < errorMargin && Mathf.Abs(first.w - second.w) < errorMargin;
	}

	public static Vector3 angleToVector(float angle) 
	{
		return (Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.right).normalized;
	}

	public static float VectorToAngle(Vector3 vector)
	{
		float angle = Vector3.Angle (vector, Vector3.right);
		if (vector.y < 0)
			angle *= -1;
		return angle;
	}
}
