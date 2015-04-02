using UnityEngine;
using System.Collections;
using LitJson;

public class Base : InstanceObject {
	public GameObject gameObject { get; set; }
	public string username { get; set; }
	public int colorId { get; set; }
	public int baseId	{ get; set; }
	public Point world	{ get; set; } // world location (larger grid)
	public Point local	{ get; set; } // local location (inner grid, 3x3 within each square of world grid)
	
	private int _prodRate;
	public int prodRate { get {
			return _prodRate;
		} set{
			this._prodRate = value;
			if (this._updateProdRateEvent != null) {
				this._updateProdRateEvent(this._prodRate);
			}
		} }
	private EventManager.UpdateContentEvent _updateProdRateEvent;
	[DoNotSerialize]
	public EventManager.UpdateContentEvent updateProdRateEvent {
		get {
			return this._updateProdRateEvent;
		} set {
			this._updateProdRateEvent = value;
			this._updateProdRateEvent(this._prodRate);
		}
	}

	// This is super wordy but it is how we link this with the UI
	private int _units;
	public int units { 
		get{
			return this._units;
		} set {
			this._units = value;
			if (updateUnitsEvent != null) {
				updateUnitsEvent(this._units);
			}
		} }
	private EventManager.UpdateContentEvent _updateUnitsEvent;
	[DoNotSerialize]
	public EventManager.UpdateContentEvent updateUnitsEvent {get {
			return this._updateUnitsEvent;
		} set {
			this._updateUnitsEvent = value;
			this._updateUnitsEvent(this._units);
		}}

	private int _unitsToAdd;
	public int unitsToAdd { get {
			return _unitsToAdd;
		} set{
			_unitsToAdd = value;
			if (this.updateAddUnitProgress != null) {
				this.updateAddUnitProgress(this._unitsToAdd);
			}
		} }
	[DoNotSerialize]
	public EventManager.UpdateContentEvent updateAddUnitProgress { get; set; }

	public long lastUpdated {get; set; }


	// {"username":"kmw8sf","colorId":1,"baseId":1,"world":{"x":0,"y":0},"local":{"x":0,"y":0},"prodRate":10,"units":151,"unitsToAdd":0,"lastUpdated":0}
	

	public override bool Equals (object obj)
	{
		if (obj == null)
			return false;
		if (ReferenceEquals (this, obj))
			return true;
		if (obj.GetType () != typeof(Base))
			return false;
		Base other = (Base)obj;
		return baseId == other.baseId;
	}
	

	public override int GetHashCode ()
	{
		unchecked {
			return baseId.GetHashCode ();
		}
	}

	public Vector3 convertBaseCoordsToWorld()
	{
		return new Vector3 ( 2 * Globals.baseRadius * (world.x * 5 + local.x), Globals.baseRadius * 2 * (world.y * 5 + local.y), 0);
	}

	public GameObject getGameObject()
	{
		foreach (GameObject baseObj in GenerateWorld.instance.getCurrentBases()) {
			if (baseObj.GetComponent<TouchBase>().b.Equals(this))
			{
				return baseObj;
			}
		}
		Debug.Log ("Couldn't find matching base gameobject!");
		return null;
	}
	

}
