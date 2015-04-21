using UnityEngine;
using System.Collections;
using LitJson;

public class Portal2 : InstanceObject {
	public GameObject gameObject { get; set; }

	public int portalId { get; set; }
	public string username { get; set; }

	public int base1Id { get; set; }
	[DoNotSerialize]
	public Base base1 {
		get {
			return (Base) ObjectInstanceDictionary.getObjectInstanceById("Base", base1Id);
		}
	}

	public int base2Id { get; set; }
	[DoNotSerialize]
	public Base base2 {
		get {
			return (Base) ObjectInstanceDictionary.getObjectInstanceById("Base", base2Id);
		}
	}

	public long timeFinished { get; set; }
	public int flowRate { get; set; }

	private int _troopsToMove;
	public int troopsToMove {
		get{
			return _troopsToMove;
		}
		set{
			this._troopsToMove = value;
			if (updateTroopsToMove != null)
				updateTroopsToMove(this._troopsToMove);
		}
	}
	public EventManager.UpdateContentEvent updateTroopsToMove { get; set; }

	public long lastMoveUpdate { get; set; }

	//public override string ToString ()
	//{
	//	return string.Format ("[Portal2: portalId={0}, username={1}, base1={2}, base2={3}, timeFinished={4}, flowRate={5}, troopsToMove={6}, lastMoveUpdate={7}]", 
	//	                      portalId, username, base1, base2, timeFinished, flowRate, troopsToMove, lastMoveUpdate);
	//}
}
