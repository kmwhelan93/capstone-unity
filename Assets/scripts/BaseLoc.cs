using UnityEngine;
using System.Collections;

public class BaseLoc {
	public int baseId	{ get; set; }
	public Point world	{ get; set; } // world location (larger grid)
	public Point local	{ get; set; } // local location (inner grid, 3x3 within each square of world grid)
}
