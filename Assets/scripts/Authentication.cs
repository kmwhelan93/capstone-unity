using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using LitJson;

public class Authentication : MonoBehaviour {

	public Text usernameInput;
	public Text passwordInput;

	public void authenticate() 
	{
		StartCoroutine ("authenticateWithServer");
	}

	IEnumerator authenticateWithServer() 
	{
		WWWForm form = new WWWForm ();
		form.AddField ("username", usernameInput.text);
		form.AddField ("password", passwordInput.text);
		WWW hs_get = new WWW ("localhost:8080/myapp/authenticate", form);
		yield return hs_get;

		SuccessObject result = JsonMapper.ToObject<SuccessObject> (hs_get.text);
		if (result.success) 
		{
			Debug.Log ("Successfully logged in!");
			Application.LoadLevel ("WorldView");
		} else {
			Debug.Log ("Login failed. Try again.");
		}


	}

	public class SuccessObject
	{
		public bool success { get; set; }
	}
}
