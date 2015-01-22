using UnityEngine;
using System.Collections;
using LitJson;

public class NewBehaviourScript : MonoBehaviour {
	public Vector2 direction = new Vector2(1, 0);
	// Use this for initialization
	void Start () {
		//Debug.Log ("HERE");
		StartCoroutine ("method");


	}

	public class Person
	{
		// C# 3.0 auto-implemented properties
		public string   Name     { get; set; }
		public int      Age      { get; set; }
		public string Birthday { get; set; }
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate(direction*Time.deltaTime);
	}

	IEnumerator method() 
	{
		WWW hs_get = new WWW ("localhost:8080/myapp/myresource");
		yield return hs_get;
		//Debug.Log (hs_get.text);

		string json = @"
          {
            ""album"" : {
              ""name""   : ""The Dark Side of the Moon"",
              ""artist"" : ""Pink Floyd"",
              ""year""   : 1973,
              ""tracks"" : [
                ""Speak To Me"",
                ""Breathe"",
                ""On The Run""
              ]
            }
          }
        ";

		JsonData fromPage = JsonMapper.ToObject (hs_get.text);
		//Debug.Log (fromPage ["key"]);

		JsonData data = JsonMapper.ToObject(json);
		
		// Dictionaries are accessed like a hash-table
		//Debug.Log ("Album's name: " + data["album"]["name"]);
		
		// Scalar elements stored in a JsonData instance can be cast to
		// their natural types
		string artist = (string) data["album"]["artist"];
		int    year   = (int) data["album"]["year"];
		
		//Debug.Log("Recorded by" +  artist + year);
		
		// Arrays are accessed like regular lists as well
		//Debug.Log("First track: " + data["album"]["tracks"][0]);
		
		
	}	
}
