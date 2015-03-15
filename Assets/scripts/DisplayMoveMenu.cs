using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DisplayMoveMenu : MonoBehaviour {

	private bool showMoveMenu = false;

	public Toggle moveTroopsToggle;
	
	public void onArrowButtonClick() {
		toggleMoveMenu ();
	}
	
	public void onMoveTroopsToggle() {
		if (moveTroopsToggle.isOn) {
			Globals.moveState = MoveState.Troops;
			GenerateWorld.instance.message.text = "Click a base with units";
		} else {
			Globals.moveState = MoveState.None;
			GenerateWorld.instance.message.text = "";
		}
	}
	
	private void toggleMoveMenu()
	{
		showMoveMenu = !showMoveMenu;
		if (!showMoveMenu) 
		{
			Globals.moveState = MoveState.None;
			moveTroopsToggle.isOn = false;
			GenerateWorld.instance.message.text = "";
		}

		moveTroopsToggle.interactable = showMoveMenu;
	}
}
