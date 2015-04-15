using UnityEngine;
using System.Collections;
using LitJson;

public class Attack : MonoBehaviour {

	public string attacker { get; set; }

	public int attackerBaseId { get; set; }
	[DoNotSerialize]
	public Base attackerBase { get { return (Base) ObjectInstanceDictionary.getObjectInstanceById ("Base", attackerBaseId); } }
	
	public int attackerWormholeId { get; set; }
	[DoNotSerialize]
	public WormHole attackerWormHole {
		get{
			return (WormHole) ObjectInstanceDictionary.getObjectInstanceById("WormHole", attackerWormholeId);
		}
	}

	public string defender { get; set; }

	public int defenderBaseId { get; set; }
	[DoNotSerialize]
	public Base defenderBase {
		get {
			return (Base) ObjectInstanceDictionary.getObjectInstanceById("Base", defenderBaseId);
		}
	}

	public int defenderWormholeId { get; set; }
	[DoNotSerialize]
	public WormHole defenderWormHole {
		get {
			return (WormHole) ObjectInstanceDictionary.getObjectInstanceById("WormHole", defenderWormholeId);
		}
	}

	public long timeInitiated { get; set; }
	public long timeAttackLands { get; set; }

	private long _lastUpdate;
	public long lastUpdate {
		get{return this._lastUpdate;}
		set{
			this._lastUpdate = value;
			if (_lastUpdateEvent != null) {
				_lastUpdateEvent(this.timeAttackLands - _lastUpdate);
			}
		}
	}
	private EventManager.UpdateContentEvent _lastUpdateEvent;
	[DoNotSerialize]
	public EventManager.UpdateContentEvent lastUpdateEvent {
		get{ return this._lastUpdateEvent;}
		set{
			this._lastUpdateEvent = value;
			this._lastUpdateEvent(this.timeAttackLands - this.timeInitiated);
		}
	}
	
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
		return string.Format ("[Attack: attacker={0}, attackerBaseId={1}, attackerWormholeId={2}, defender={3}, defenderBaseId={4}, defenderWormholeId={5}, timeInitiated={6}, timeAttackLands={7}, lastUpdate={8}, numUnits={9}]", attacker, attackerBaseId, attackerWormholeId, defender, defenderBaseId, defenderWormholeId, timeInitiated, timeAttackLands, lastUpdate, numUnits);
	}
	
	
}
