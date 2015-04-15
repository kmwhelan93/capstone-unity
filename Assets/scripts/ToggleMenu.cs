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
			GenerateWorld.instance.message.text = "Click a base to add a unit";
		} else {
			Globals.opState = OpState.None;
			GenerateWorld.instance.sliderObject.SetActive(false);
			GenerateWorld.instance.sliderConfirmButton.SetActive(false);
			GenerateWorld.instance.message.text = "";
			GenerateWorld.instance.sliderValue.text = "";
		}
	}

	public void onMoveTroopsToggle(bool isOn) {
		if (isOn) {
			Globals.opState = OpState.MoveTroops;
			GenerateWorld.instance.message.text = "Click a base with units";
		} else {
			Globals.opState = OpState.None;
			GenerateWorld.instance.sliderObject.SetActive(false);
			GenerateWorld.instance.sliderConfirmButton.SetActive(false);
			GenerateWorld.instance.message.text = "";
			GenerateWorld.instance.sliderValue.text = "";
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

	public void onZoomEmpireToggle(bool isOn) {
		if (isOn) {
			Globals.opState = OpState.ZoomEmpire;
			GenerateWorld.instance.message.text = "Touch base to zoom";
			Camera.main.GetComponent<LocalView>().switchToEmpireView();
		} else {
			Globals.opState = OpState.None;
			GenerateWorld.instance.message.text = "";
		}
	}

	public void onAttackToggle(bool isOn) {
		if (isOn) {
			Globals.opState = OpState.Attack;
			GenerateWorld.instance.message.text = "Touch wormhole to attack";
		} else {
			Globals.opState= OpState.None;
			GenerateWorld.instance.message.text = "";
		}
	}

}
