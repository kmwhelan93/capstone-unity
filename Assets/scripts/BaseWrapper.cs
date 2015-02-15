using UnityEngine;
using System.Collections;

public class BaseWrapper {

	public GameObject baseObj { get; set; }
	public GameObject displayText { get; set; }

	public BaseWrapper(GameObject baseObj, GameObject displayText) 
	{
		this.baseObj = baseObj;
		this.displayText = displayText;
	}
}
