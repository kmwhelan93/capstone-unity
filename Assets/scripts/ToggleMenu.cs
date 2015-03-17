using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ToggleMenu : MonoBehaviour {

	private bool showMenu = false;

	public List<Toggle> toggles;

	public void toggleMenu()
	{
		showMenu = !showMenu;
		foreach(Toggle t in toggles) {
			if (!showMenu)
				t.isOn = false;
			t.interactable = showMenu;
		}
	}
	// TODO: consolidate to just one method that takes more parameters
	public void onNewBaseToggle(bool isOn)
	{
		if (isOn) {
			Globals.opState = OpState.AddBase;
			GenerateWorld.instance.message.text = "Click a base to add a new base";
		} else {
			Globals.opState = OpState.None;
			GenerateWorld.instance.message.text = "";
		}
	}

	public void onNewPortalToggle(bool isOn)
	{
		if (isOn) {
			Globals.opState = OpState.AddPortal;
			GenerateWorld.instance.message.text = "Click a base and drag to make a portal";
		} else {
			Globals.opState = OpState.None;
			GenerateWorld.instance.message.text = "";
		}

	}

	public void onNewTroopsToggle(bool isOn) {
		if (isOn) {
			Globals.opState = OpState.AddTroops;
			GenerateWorld.instance.message.text = "Click a base with units";
		} else {
			Globals.opState = OpState.None;
			GenerateWorld.instance.message.text = "";
		}
	}

	public void onMoveTroopsToggle(bool isOn) {
		if (isOn) {
			Globals.opState = OpState.MoveTroops;
			GenerateWorld.instance.message.text = "Click a base with units";
		} else {
			Globals.opState = OpState.None;
			GenerateWorld.instance.message.text = "";
		}
	}

	public void onZoomWorldToggle(bool isOn) {
		if (isOn) {
			Globals.opState = OpState.ZoomBase;
			GenerateWorld.instance.message.text = "Click on a base to view it";
		} else {
			Globals.opState = OpState.None;
			GenerateWorld.instance.message.text = "";
		}
	}

}
