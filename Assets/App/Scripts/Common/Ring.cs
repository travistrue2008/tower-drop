using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ring : MonoBehaviour {
	#region Fields
	private Collider _burstCollider;
	private Segment[] _segments;
	#endregion

	#region MonoBehaviour Hooks
	private void Awake () {
		_segments = GetComponentsInChildren<Segment>();
		_burstCollider = GetBurstCollider();
	}

	public void Burst (bool slam = false) {
		_burstCollider.enabled = false;
		foreach (var segment in _segments) {
			segment.Fall(slam);
		}
	}
	#endregion

	#region Private Methods
	private Collider GetBurstCollider () {
		foreach (Transform child in transform) {
			if (child.gameObject.tag == "Burst") {
				return child.GetComponent<Collider>();
			}
		}

		return null;
	}
	#endregion
}
