using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TRUEStudios.Foundation.Events;
using TRUEStudios.Foundation.Variables;

[Serializable]
public class RingEvent : UnityEvent<Ring, int> {}

public class Ball : MonoBehaviour {
	#region Constants
	public const int SlamThreshold = 4;
	public const float MaxDropDistance = 1.0f;

	public readonly Vector3 Bounce = new Vector3(0.0f, 4.2f, 0.0f);
	#endregion

	#region Fields
	[SerializeField]
	private Transform _discardContainer;
	[SerializeField]
	private IntReference _streakReference;
	[SerializeField]
	private RingEvent _onClearRing = new RingEvent();
	[SerializeField]
	private BoolEvent _onFinish = new BoolEvent();
	
	private Vector3 _initialPosition;
	private Rigidbody _rigidBody;
	private TrailRenderer _trailRenderer;
	#endregion

	#region Properties
	public RingEvent OnClearRing { get { return _onClearRing; } }
	public BoolEvent OnFinish { get { return _onFinish; } }
	#endregion

	#region MonoBehaviour Hooks
	private void Awake () {
		_initialPosition = transform.position;
		_rigidBody = GetComponent<Rigidbody>();
		_trailRenderer = GetComponent<TrailRenderer>();
	}

	private void OnEnable () {
		_rigidBody.useGravity = true;
		_trailRenderer.enabled = false;
		_trailRenderer.Clear();
		_trailRenderer.enabled = true;
	}

	private void OnDisable () {
		_rigidBody.velocity = Vector3.zero;
		_rigidBody.useGravity = false;
	}

	private void OnTriggerEnter (Collider other) {
		// get the Ring component, and handle based on tag
		if (other.gameObject.tag == "Burst") {
			var ring = other.transform.parent.GetComponent<Ring>();
			BurstRing(ring, false);
		}
	}

	private void OnCollisionEnter (Collision collision) {
		// check if the finish object was hit
		if (collision.gameObject.tag == "Finish") {
			_onFinish.Invoke(true);
			enabled = false;
		}

		// get the ring segment
		var segment = collision.transform.GetComponent<Segment>();
		if (segment != null) {
			ProcessSegmentCollision(segment);
		}
	}
	#endregion

	#region Actions
	public void Reset () {
		enabled = true;
		_streakReference.Value = 0;
		transform.position = _initialPosition;
		_trailRenderer.Clear();
	}
	#endregion

	#region Private Methods
	private void ProcessSegmentCollision (Segment segment) {
		_rigidBody.velocity = Bounce; // reset the bounce to a pre-determined one

		// check if the segment is a hazard
		if (segment.IsHazard) {
			_onFinish.Invoke(false);
			enabled = false;
		} else if (_streakReference.Value >= SlamThreshold) { // check if the streak is large enough to burst its ring
			BurstRing(segment.ParentRing, true);
		}
		
		_streakReference.Value = 0;
	}

	private void BurstRing (Ring ring, bool slam) {
		ring.transform.SetParent(_discardContainer);
		ring.Burst(slam);

		_onClearRing.Invoke(ring, ++_streakReference.Value);
	}
	#endregion
}
