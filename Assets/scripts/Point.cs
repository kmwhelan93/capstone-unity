using UnityEngine;
using System.Collections;

public class Point {
	public int x	{ get; set; }
	public int y	{ get; set; }

	public override bool Equals (object obj)
	{
		Point other = (Point)obj;
		return this.x == other.x && this.y == other.y;
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
