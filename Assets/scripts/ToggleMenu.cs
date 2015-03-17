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

	public void onNewBaseToggle(bool isOn)
	{
		if (isOn) {
			Globals.addState = AddState.Base;
			GenerateWorld.instance.message.text = "Click a base to add a new base";
		} else {
			Globals.addState = AddState.None;
			GenerateWorld.instance.message.text = "";
		}
	}

	public void onNewPortalToggle(bool isOn)
	{
		if (isOn) {
			Globals.addState = AddState.Portal;
			GenerateWorld.instance.message.text = "Click a base and drag to make a portal";
		} else {
			Globals.addState = AddState.None;
			GenerateWorld.instance.message.text = "";
		}

	}

	public void onNewTroopsToggle(bool isOn) {
		if (isOn) {
			Globals.addState = AddState.Troops;
			GenerateWorld.instance.message.text = "Click a base with units";
		} else {
			Globals.addState = AddState.None;
			GenerateWorld.instance.message.text = "";
		}
	}
}
