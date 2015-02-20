using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DisplayAddMenu : MonoBehaviour {

	private bool showAddMenu = false;

	public Toggle newBaseToggle;
	public Toggle newPortalToggle;
	

	public void onPlusButtonClick() {
		toggleAddMenu ();
	}

	public void onNewBaseToggle()
	{
		if (newBaseToggle.isOn) {
			Globals.addState = AddState.Base;
		} else {
			Globals.addState = AddState.None;
		}
	}

	public void onNewPortalToggle()
	{
		if (newPortalToggle.isOn) {
			Globals.addState = AddState.Portal;
		} else {
			Globals.addState = AddState.None;
		}

	}

	private void toggleAddMenu()
	{
		showAddMenu = !showAddMenu;
		if (!showAddMenu) 
		{
			Globals.addState = AddState.None;
		}

		newBaseToggle.interactable = showAddMenu;
		newPortalToggle.interactable = showAddMenu;
	}
}
