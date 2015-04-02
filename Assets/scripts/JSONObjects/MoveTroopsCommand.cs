using System;
using UnityEngine;
using LitJson;

public class MoveTroopsCommand
{
	public int portalId { get; set; }
	[DoNotSerialize]
	public Portal2 portal {
		get {
			return (Portal2) ObjectInstanceDictionary.getObjectInstanceById("Portal", portalId);
		}
	}
	public int troopsToMove { get; set; }
}


