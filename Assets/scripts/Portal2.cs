using UnityEngine;
using System.Collections;

public class Portal2 {

	public int portalId { get; set; }
	public string username { get; set; }
	public Base base1 { get; set; }
	public Base base2 { get; set; }
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

	public override string ToString ()
	{
		return string.Format ("[Portal2: portalId={0}, username={1}, base1={2}, base2={3}, timeFinished={4}, flowRate={5}, troopsToMove={6}, lastMoveUpdate={7}]", 
		                      portalId, username, base1, base2, timeFinished, flowRate, troopsToMove, lastMoveUpdate);
	}
}
