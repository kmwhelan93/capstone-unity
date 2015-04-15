using UnityEngine;
using System.Collections;

public interface UIPlacer {
	Vector3 getUIScreenPosition();
	Vector2 getPivot();
}
