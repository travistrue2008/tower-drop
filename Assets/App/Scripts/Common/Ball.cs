using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TRUEStudios.Core;

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
	private IntEvent _onStreakChanged = new IntEvent();
	[SerializeField]
	private RingEvent _onClearRing = new RingEvent();
	[SerializeField]
	private UnityEvent _onFail = new UnityEvent();
	[SerializeField]
	private UnityEvent _onFinish = new UnityEvent();
	
	private int _streak = 0;
	private Vector3 _initialPosition;
	private Rigidbody _rigidBody;
	private TrailRenderer _trailRenderer;
	#endregion

	#region Properties
	public IntEvent OnStreakChanged { get { return _onStreakChanged; } }
	public RingEvent OnClearRing { get { return _onClearRing; } }
	public UnityEvent OnFail { get { return _onFail; } }
	public UnityEvent OnFinish { get { return _onFinish; } }

	public int Streak {
		set {
			if (_streak != value) {
				_streak = value;
				_onStreakChanged.Invoke(_streak);
			}
		}

		get { return _streak; }
	}
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
			BurstRing(ring);
		}
	}

	private void OnCollisionEnter (Collision collision) {
		// check if the finish object was hit
		if (collision.gameObject.tag == "Finish") {
			_onFinish.Invoke();
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
		Streak = 0;
		enabled = true;
		transform.position = _initialPosition;
		_trailRenderer.Clear();
	}
	#endregion

	#region Private Methods
	private void ProcessSegmentCollision (Segment segment) {
		// reset the bounce to a pre-determined one, and reset the streak
		_rigidBody.velocity = Bounce;
		Streak = 0;
		
		// check if the segment is a hazard
		if (segment.IsHazard) {
			_onFail.Invoke();
			enabled = false;
		} else if (Streak >= SlamThreshold) { // check if the streak is large enough to burst its ring
			BurstRing(segment.ParentRing);
		}
	}

	private void BurstRing (Ring ring) {
		ring.transform.SetParent(_discardContainer);
		ring.Burst();

		_onClearRing.Invoke(ring, ++Streak);
	}
	#endregion
}
