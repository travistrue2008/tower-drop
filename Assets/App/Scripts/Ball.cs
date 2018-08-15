using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class RingEvent : UnityEvent<Ring> { }

public class Ball : MonoBehaviour {
	#region Fields
	[SerializeField]
	private RingEvent _onBreakRing = new RingEvent();

	private Rigidbody _rigidBody;
	#endregion

	#region Properties
	public RingEvent OnBreakRing { get { return _onBreakRing; } }
	#endregion

	#region MonoBehaviour Hooks
	private void Awake () {
		_rigidBody = GetComponent<Rigidbody>();
	}

	private void OnTriggerEnter (Collider other) {
		// check if a breaker was entered
		if (other.gameObject.tag == "Breaker") {
			// get the Ring component, and break
			var ring = other.transform.parent.GetComponent<Ring>();
			if (ring != null) {
				_onBreakRing.Invoke(ring);
			} else {
				throw new NullReferenceException("No Ring component found on parent of GameObject tagged as 'Breakder'");
			}
		}
	}

	private void OnCollisionEnter (Collision collision) {
		_rigidBody.velocity = new Vector3(0.0f, 6.0f, 0.0f);
	}
	#endregion
}
