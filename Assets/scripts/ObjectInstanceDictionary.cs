using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * All game objects that you want to be accessible by this class need to register via the registerGameObject method
 */
public class ObjectInstanceDictionary {
	private static Dictionary<string, GameObject> gameObjects = new Dictionary<string, GameObject> ();
	public static InstanceObject getObjectInstanceById(string type, int id) {
		GameObject gameObject = getGameObjectById(type, id);
		return gameObject.GetComponent<InstanceObjectScript>().instanceObject;
	}
	
	private static GameObject getGameObjectById(string type, int id) {
		GameObject gameObject = gameObjects[type+id];
		return gameObject;
	}


	public static void registerGameObject(string name, GameObject gameObject) {
		gameObjects.Add (name, gameObject);
	}

}
