﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class DisplayInfoHandler : MonoBehaviour {

	public List<BaseWrapper> baseWrappers { get; set; }
	public float hideTextDepth;



	private Vector3 getBestOffset(BaseWrapper bw)
	{
		GameObject baseObj = bw.baseObj;
		GameObject displayText = bw.displayText;
		Base b = baseObj.GetComponent<TouchBase>().b;
		NESWBases nearBases = getNESWBases (b);
		float radius = Globals.baseRadius;
		if (nearBases.E == null) 
		{
			displayText.GetComponent<RectTransform>().pivot = new Vector2(0, .5f);
			return new Vector3((float)radius*1.1f, (float)0, 0);
		} 
		else if (nearBases.W == null)
		{
			displayText.GetComponent<RectTransform>().pivot = new Vector2(1, .5f);
			return new Vector3(-1*(float)radius*1.1f, (float)0, 0);
		} 
		else if (nearBases.N == null)
		{
			displayText.GetComponent<RectTransform>().pivot = new Vector2(.5f, 0);
			return new Vector3((float)0, (float)radius*1.1f, 0);
		} 
		else if (nearBases.S == null) 
		{
			displayText.GetComponent<RectTransform>().pivot = new Vector2(.5f, 1);
			return new Vector3((float)0, -1*(float)radius*1.1f, 0);
		}
		return new Vector3((float)radius*1.1f, (float)radius, 0);
	}

	private NESWBases getNESWBases(Base b)
	{
		NESWBases retVal = new NESWBases ();
		foreach (BaseWrapper bw in baseWrappers) 
		{
			Base other = bw.baseObj.GetComponent<TouchBase>().b;
			if (b.world.add (new Point(0, 1)).Equals(other.world))
			{
				retVal.N = other;
			} else if (b.world.add (new Point(1, 0)).Equals(other.world))
			{
				retVal.E = other;
			} else if (b.world.add (new Point(0, -1)).Equals(other.world))
			{
				retVal.S = other;
			} else if (b.world.add (new Point(-1, 0)).Equals(other.world))
			{
				retVal.W = other;
			}
		}
		return retVal;
	}


	class NESWBases
	{
		public Base N { get; set; }
		public Base E { get; set; }
		public Base S { get; set; }
		public Base W { get; set; }
	}
}
