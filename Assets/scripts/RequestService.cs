using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;

public class RequestService : ScriptableObject {
	public static string baseUrl = "http://localhost:8080/myapp/";
	public static WWW makeRequest(string path, object body)
	{
		var jsonString = JsonMapper.ToJson (body);
		
		var encoding = new System.Text.UTF8Encoding();
		Dictionary<string, string> postHeader = new Dictionary<string, string>();
		postHeader.Add("Content-Type", "application/json");
		postHeader.Add("Content-Length", jsonString.Length + "");

		WWW request = new WWW(baseUrl + path, encoding.GetBytes(jsonString), postHeader);
		return request;
	}
}
