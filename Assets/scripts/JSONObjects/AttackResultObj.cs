using UnityEngine;
using System.Collections;

public class AttackResultObj : MonoBehaviour {

	public int attackId { get; set; }
	public string winnerUsername { get; set; }
	public int numUnitsLeft { get; set; }
	public NewBase newBase  { get; set; }
	//public bool winnerHasViewed { get; set; }
	//public bool loserHasViewed { get; set; }
	
	//public override string ToString ()
	//{
	//	return string.Format ("[AttackResultObj: attackId={0}, winnerUsername={1}, numUnitsLeft={2}, winnerHasViewed={3}, loserHasViewed={4}]", attackId, winnerUsername, numUnitsLeft, winnerHasViewed, loserHasViewed);
	//}
	
	public override string ToString ()
	{
		return string.Format ("[AttackResultObj: attackId={0}, winnerUsername={1}, numUnitsLeft={2}, newBase={3}]", attackId, winnerUsername, numUnitsLeft, newBase);
	}
}
