using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ring : MonoBehaviour {
	#region Fields
	[SerializeField]
	private Collider _passCollider;

	private Segment[] _segments;
	#endregion

	#region MonoBehaviour Hooks
	private void Awake () {
		_segments = GetComponentsInChildren<Segment>();

		// make sure _passCollider is set
		if (_passCollider == null) {
			throw new NullReferenceException("_passCollider not set");
		}
	}

	public void Break () {
		foreach (var segment in _segments) {
			segment.Fall();
		}
	}
	#endregion
}
