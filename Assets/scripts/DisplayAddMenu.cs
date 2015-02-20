using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DisplayAddMenu : MonoBehaviour {

	public GameObject toggleModeButton;
	public GameObject clearBasesButton;
	private bool showAddMenu = false;
	private bool newBaseBtnSelected = false;

	public Button showAddMenuBtn;
	public Toggle newBaseToggle;
	public Toggle newPortalToggle;

	// Use this for initialization
	void Start () {
		toggleModeButton.SetActive(showAddMenu);
		clearBasesButton.SetActive(showAddMenu);

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void onPlusButtonClick() {
		showAddMenu = !showAddMenu;
		showMenu (showAddMenu);
	}

	public void onNewBaseClick()
	{
		newBaseBtnSelected = !newBaseBtnSelected;
	}

	private void showMenu(bool show)
	{
		toggleModeButton.SetActive(showAddMenu);
		clearBasesButton.SetActive(showAddMenu);


		newBaseToggle.interactable = show;
		newPortalToggle.interactable = show;
	}
}
