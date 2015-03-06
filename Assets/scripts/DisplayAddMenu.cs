using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DisplayAddMenu : MonoBehaviour {

	private bool showAddMenu = false;

	public Toggle newBaseToggle;
	public Toggle newPortalToggle;
	public Toggle moveTroopsToggle;

	public void onPlusButtonClick() {
		toggleAddMenu ();
	}

	public void onNewBaseToggle()
	{
		if (newBaseToggle.isOn) {
			Globals.addState = AddState.Base;
			GenerateWorld.instance.message.text = "Click a base to add a new base";
		} else {
			Globals.addState = AddState.None;
			GenerateWorld.instance.message.text = "";
		}
	}

	public void onNewPortalToggle()
	{
		if (newPortalToggle.isOn) {
			Globals.addState = AddState.Portal;
			GenerateWorld.instance.message.text = "Click a base and drag to make a portal";
		} else {
			Globals.addState = AddState.None;
			GenerateWorld.instance.message.text = "";
		}

	}

	public void onMoveTroopsToggle() {
		if (moveTroopsToggle.isOn) {
			Globals.addState = AddState.Troops;
			GenerateWorld.instance.message.text = "Click a base with units";
		} else {
			Globals.addState = AddState.None;
			GenerateWorld.instance.message.text = "";
		}
	}

	private void toggleAddMenu()
	{
		showAddMenu = !showAddMenu;
		if (!showAddMenu) 
		{
			Globals.addState = AddState.None;
			GenerateWorld.instance.message.text = "";
		}

		newBaseToggle.interactable = showAddMenu;
		newPortalToggle.interactable = showAddMenu;
		moveTroopsToggle.interactable = showAddMenu;
	}
}
