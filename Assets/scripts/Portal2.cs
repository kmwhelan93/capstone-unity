using UnityEngine;
using System.Collections;

public class Portal2 {

	public int portalId { get; set; }
	public string username { get; set; }
	public Base base1 { get; set; }
	public Base base2 { get; set; }
	public int flowRate { get; set; }
	public long timeFinished { get; set; }

	public override string ToString ()
	{
		return string.Format ("[Portal2: portalId={0}, username={1}, base1={2}, base2={3}, flowRate={4}, timeFinished={5}]", portalId, username, base1, base2, flowRate, timeFinished);
	}
}
