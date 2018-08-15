using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Ball : MonoBehaviour {
	#region Constants
	public const float MaxRotationSpeed = 180.0f;
	#endregion

	#region Fields
	[SerializeField]
	private Transform _ringsContainer;
	[SerializeField]
	private Transform _discardContainer;

	private int _score = 0;
	private int _ringsBroken = 0;
	private Rigidbody _rigidBody;
	private InputController _inputController;
	private Ring[] _rings;
	#endregion

	#region Properties
	public float Progress {
		get {
			return (float)_ringsBroken / (float)_rings.Length;
		}
	}
	#endregion

	#region MonoBehaviour Hooks
	private void Awake () {
		_rigidBody = GetComponent<Rigidbody>();
		_inputController = GetComponent<InputController>();
		_rings = _ringsContainer.GetComponentsInChildren<Ring>();
	}

	private void Update() {
		float delta = -_inputController.LeftAxis.x * MaxRotationSpeed * Time.deltaTime;
		_ringsContainer.Rotate(0.0f, delta, 0.0f);
	}

	private void OnTriggerEnter (Collider other) {
		// check if a breaker was entered
		if (other.gameObject.tag == "Breaker") {
			// get the Ring component, and break
			var ring = other.transform.parent.GetComponent<Ring>();
			if (ring != null) {
				transform.SetParent(_discardContainer);
				ring.Break();
				++_score;
				++_ringsBroken;
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
