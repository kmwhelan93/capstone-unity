using UnityEngine;
using System.Collections;

public class NewBase {
	public Base b;
	public Portal2 p;
	
	public override string ToString ()
	{
		return string.Format ("[NewBase: b={0}, p={1}]", b, p);
	}
}
