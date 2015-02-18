using UnityEngine;
using System.Collections;

public class DisplayAddMenu : MonoBehaviour {

	public GameObject toggleModeButton;
	public GameObject clearBasesButton;
	public bool showMenu;

	// Use this for initialization
	void Start () {
		showMenu = false;
		toggleModeButton = GameObject.Find ("SelectModeButton");
		clearBasesButton = GameObject.Find ("ClearButton");
		toggleModeButton.SetActive(showMenu);
		clearBasesButton.SetActive(showMenu);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void onPlusButtonClick() {
		showMenu = !showMenu;
		toggleModeButton.SetActive(showMenu);
		clearBasesButton.SetActive(showMenu);
	}
}
