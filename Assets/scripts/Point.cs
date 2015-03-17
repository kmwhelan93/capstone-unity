using UnityEngine;
using System.Collections;

public class Point {
	public int x	{ get; set; }
	public int y	{ get; set; }

	public override bool Equals (object obj)
	{
		if (obj == null)
			return false;
		if (ReferenceEquals (this, obj))
			return true;
		if (obj.GetType () != typeof(Point))
			return false;
		Point other = (Point)obj;
		return x == other.x && y == other.y;
	}
	

	public override int GetHashCode ()
	{
		unchecked {
			return x.GetHashCode () ^ y.GetHashCode ();
		}
	}
	

	public Point add(Point p)
	{
		return new Point (p.x + this.x, p.y + this.y);
	}

	public Point(int x, int y)
	{
		this.x = x;
		this.y = y;
	}

	public Point()
	{

	}
}
