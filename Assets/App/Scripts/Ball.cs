using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Ball : MonoBehaviour {
	#region Fields
	[SerializeField]
	private UnityEvent _onCollide = new UnityEvent();

	private Rigidbody _rigidBody;
	#endregion

	#region Properties
	public UnityEvent OnCollide { get { return _onCollide; } }
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
			ring.Break();
		}
	}

	private void OnCollisionEnter (Collision collision) {
		_rigidBody.velocity = new Vector3(0.0f, 6.0f, 0.0f);
		_onCollide.Invoke();
	}
	#endregion
}
