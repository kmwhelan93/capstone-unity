using System;
using UnityEngine;
using LitJson;

public class AddTroopsCommand
{
	public int bId { get; set; }
	[DoNotSerialize]
	public Base b {
		get {
			return (Base) ObjectInstanceDictionary.getObjectInstanceById("Base", bId);
		}
	}
	public int troopsToAdd { get; set; }
}

