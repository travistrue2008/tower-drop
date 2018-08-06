using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ring : MonoBehaviour {
	[Serializable]
	public struct SegmentData {
		Segment.Type type;
		int span;
		int offset;
	}

	#region Fields
	[SerializeField]
	private int _resolution = 360;
	[SerializeField]
	private SegmentData[] _segmentData;
	#endregion
}
