using UnityEngine;
using System.Collections;

public class EventManager {
	public delegate void DefaultDelegate();
	public static DefaultDelegate positionText;

	public delegate void UpdateContentEvent(long value);

}
