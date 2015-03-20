using UnityEngine;
using System.Collections;

public class WormHole {
	public int wormholeId { get; set; }
	public Base b { get; set; }
	public int connectedWormholeId { get; set; }
	public Point relativeCoords { get; set; }

	public Vector3 getWorldCoords()
	{
		Vector3 offset = new Vector3 (relativeCoords.x, relativeCoords.y, 0).normalized * .65f;
		offset = offset + new Vector3 (0, 0, -.5f - Globals.baseRadius);
		Vector3 basePosition = b.getGameObject().transform.position;
		return basePosition + offset;
	}

	public override bool Equals (object obj)
	{
		if (obj == null)
			return false;
		if (ReferenceEquals (this, obj))
			return true;
		if (obj.GetType () != typeof(WormHole))
			return false;
		WormHole other = (WormHole)obj;
		return wormholeId == other.wormholeId && b == other.b && connectedWormholeId == other.connectedWormholeId && relativeCoords == other.relativeCoords;
	}
	

	public override int GetHashCode ()
	{
		unchecked {
			return wormholeId.GetHashCode () ^ (b != null ? b.GetHashCode () : 0) ^ connectedWormholeId.GetHashCode () ^ (relativeCoords != null ? relativeCoords.GetHashCode () : 0);
		}
	}
	
	public override string ToString ()
	{
		return string.Format ("[WormHole: wormholeId={0}, b={1}, connectedWormholeId={2}, relativeCoords={3}]", wormholeId, b, connectedWormholeId, relativeCoords);
	}
	
}