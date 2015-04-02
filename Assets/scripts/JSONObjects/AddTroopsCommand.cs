using System;
using UnityEngine;

public class AddTroopsCommand
{
	public int bId { get; set; }
	public Base b {
		get {
			return (Base) ObjectInstanceDictionary.getObjectInstanceById("Base", bId);
		}
	}
	public int troopsToAdd { get; set; }
}

