using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ring : MonoBehaviour {
	#region Fields
	private Segment[] _segments;
	#endregion

	#region MonoBehaviour Hooks
	private void Awake () {
		_segments = GetComponentsInChildren<Segment>();
		ValidateBurstChild();
	}

	public void Burst (bool slam = false) {
		// make all segments fall
		foreach (var segment in _segments) {
			segment.Fall(slam);
		}
	}
	#endregion

	#region Private Methods
	private void ValidateBurstChild () {
		foreach (Transform child in transform) {
			if (child.gameObject.tag == "Burst") { return; }
		}

		throw new NullReferenceException("No child GameObject tagged as 'Burst' found");
	}
	#endregion
}
