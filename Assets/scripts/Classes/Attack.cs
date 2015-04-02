using UnityEngine;
using System.Collections;

public class Attack : MonoBehaviour {

	public string attacker { get; set; }
	public Base attackerBase { get; set; }
	public int attackerWormholeId { get; set; }
	public string defender { get; set; }
	public Base defenderBase { get; set; }
	public int defenderWormholeId { get; set; }
	public long timeInitiated { get; set; }
	public long timeAttackLands { get; set; }
	public long lastUpdate { get; set; }
	public int numUnits { get; set; }

	public override bool Equals (object obj)
	{
		if (obj == null)
			return false;
		if (ReferenceEquals (this, obj))
			return true;
		if (obj.GetType () != typeof(Attack))
			return false;
		Attack other = (Attack)obj;
		return attacker == other.attacker && attackerBase == other.attackerBase && attackerWormholeId == other.attackerWormholeId && defender == other.defender && defenderBase == other.defenderBase && defenderWormholeId == other.defenderWormholeId && timeInitiated == other.timeInitiated && timeAttackLands == other.timeAttackLands && lastUpdate == other.lastUpdate && numUnits == other.numUnits;
	}
	

	public override int GetHashCode ()
	{
		unchecked {
			return (attacker != null ? attacker.GetHashCode () : 0) ^ (attackerBase != null ? attackerBase.GetHashCode () : 0) ^ attackerWormholeId.GetHashCode () ^ (defender != null ? defender.GetHashCode () : 0) ^ (defenderBase != null ? defenderBase.GetHashCode () : 0) ^ defenderWormholeId.GetHashCode () ^ timeInitiated.GetHashCode () ^ timeAttackLands.GetHashCode () ^ lastUpdate.GetHashCode () ^ numUnits.GetHashCode ();
		}
	}
	public override string ToString ()
	{
		return string.Format ("[Attack: attacker={0}, attackerBase={1}, attackerWormholeId={2}, defender={3}, defenderBase={4}, defenderWormholeId={5}, timeInitiated={6}, timeAttackLands={7}, lastUpdate={8}, numUnits={9}]", attacker, attackerBase, attackerWormholeId, defender, defenderBase, defenderWormholeId, timeInitiated, timeAttackLands, lastUpdate, numUnits);
	}
	
}
