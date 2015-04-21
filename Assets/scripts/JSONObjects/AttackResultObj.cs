using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AttackResultObj : MonoBehaviour {

	public int attackId { get; set; }
	public string winnerUsername { get; set; }
	public int numUnitsLeft { get; set; }
	public Base newBase  { get; set; }
	public Portal2 newPortal { get; set; }
	public int[] lostPortalIds { get; set; }
	//public bool winnerHasViewed { get; set; }
	//public bool loserHasViewed { get; set; }
	
	//public override string ToString ()
	//{
	//	return string.Format ("[AttackResultObj: attackId={0}, winnerUsername={1}, numUnitsLeft={2}, winnerHasViewed={3}, loserHasViewed={4}]", attackId, winnerUsername, numUnitsLeft, winnerHasViewed, loserHasViewed);
	//}
	
	public override string ToString ()
	{
		return string.Format ("[AttackResultObj: attackId={0}, winnerUsername={1}, numUnitsLeft={2}, newBase={3}, newPortal={4}, lostPortalIds={5}]", attackId, winnerUsername, numUnitsLeft, newBase, newPortal, lostPortalIds);
	}
}
