using UnityEngine;
using System.Collections;

public class Base {
	public string username { get; set; }
	public int colorId { get; set; }
	public int baseId	{ get; set; }
	public Point world	{ get; set; } // world location (larger grid)
	public Point local	{ get; set; } // local location (inner grid, 3x3 within each square of world grid)
	public int prodRate { get; set; }
	public int units { get; set; }

	public override bool Equals (object obj)
	{
		Base other = (Base)obj;
		return this.baseId == other.baseId;
	}
}
