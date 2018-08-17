using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Ball : MonoBehaviour {
	#region Constants
	public const int SlamThreshold = 4;
	public const float MaxDropDistance = 2.0f;
	public const float MaxRotationSpeed = 180.0f;

	public readonly Vector3 bounce = new Vector3(0.0f, 6.0f, 0.0f);
	#endregion

	#region Fields
	[SerializeField]
	private bool _isPlaying = true;
	[SerializeField]
	private Camera _camera;
	[SerializeField]
	private Transform _ringsContainer;
	[SerializeField]
	private Transform _discardContainer;
	[SerializeField]
	private UnityEvent _onScore = new UnityEvent();

	private int _level = 1;
	private int _score = 0;
	private int _streak = 0;
	private int _numRingsBroken = 0;
	private Vector3 _initialPosition;
	private Vector3 _cameraPosition;
	private Rigidbody _rigidBody;
	private InputController _inputController;
	private Ring[] _rings;
	#endregion

	#region Properties
	public int Score { get { return _score; } }
	public int Streak { get { return _streak; } }
	public int NumRingsBroken { get { return _numRingsBroken; } }
	public UnityEvent OnScore { get { return _onScore; } }

	public float Progress {
		get {
			return (_rings != null) ? Mathf.Clamp01((float)_numRingsBroken / (float)_rings.Length) : 0;
		}
	}

	public bool IsPlaying {
		set { _isPlaying = value; }
		get { return _isPlaying; }
	}

	public int Level {
		set { _level = Mathf.Max(1, value); }
		get { return _level; }
	}
	#endregion

	#region MonoBehaviour Hooks
	private void Awake () {
		_initialPosition = transform.position;
		_cameraPosition = _camera.transform.position;

		_rigidBody = GetComponent<Rigidbody>();
		_inputController = GetComponent<InputController>();
		_rings = _ringsContainer.GetComponentsInChildren<Ring>();

		// HACK: the ring container needs to be transformed, or the ball will go right through the collider
		_ringsContainer.Rotate(0.0f, 1.0f, 0.0f);
		_ringsContainer.Rotate(0.0f, 0.0f, 0.0f);
	}

	private void Update () {
		// only control when the user is playing
		if (IsPlaying) {
			float delta = -_inputController.LeftAxis.x * MaxRotationSpeed * Time.deltaTime;
			_ringsContainer.Rotate(0.0f, delta, 0.0f);
		}

		// check if the camera dropped too far
		if (_camera.transform.position.y - transform.position.y > MaxDropDistance) {
			_cameraPosition.y = transform.position.y + MaxDropDistance;
			_camera.transform.position = _cameraPosition;
		}
	}

	private void OnTriggerEnter (Collider other) {
		Debug.Log($"OnTriggerEnter: {other.gameObject.tag}");

		// check if a burst mesh was entered
		if (other.gameObject.tag == "Burst") {
			var ring = other.transform.parent.GetComponent<Ring>();
			BurstRing(ring);
		}
	}

	private void OnCollisionEnter (Collision collision) {
		Debug.Log("OnCollisionEnter()");
		_streak = 0;

		// check if the finish ring was reached
		if (collision.gameObject.tag == "Finish") {
			Stop();
			return;
		}

		// get the segment
		var segment = collision.transform.GetComponent<Segment>();
		if (segment != null) {
			// check if the player landed on a hazard
			if (segment.IsHazard) {
				Debug.Log("Player has failed");
				Stop();
			} else if (_streak >= SlamThreshold) { // check if the player can slam through the ring
				Debug.Log("STREAK");
				BurstRing(segment.ParentRing);
			} else {
				_rigidBody.velocity = bounce;
			}
		} else {
			throw new NullReferenceException("No Segment component attached to GameObject");
		}
	}
	#endregion

	#region Public Methods
	public void Stop () {
		_rigidBody.velocity = Vector3.zero;
		_rigidBody.useGravity = false;
		_isPlaying = false;
	}

	public void Reset () {
		transform.position = _initialPosition;
		_cameraPosition.y = _initialPosition.y;
		_camera.transform.position = _cameraPosition;

		_ringsContainer.eulerAngles = Vector3.zero;
		_numRingsBroken = _streak = _score = 0;
		_rigidBody.useGravity = true;
		_isPlaying = true;
	}
	#endregion

	#region Private Methods
	private void BurstRing (Ring ring) {
		// get the Ring component, and burst
		ring.transform.SetParent(_discardContainer);
		ring.Burst();
		++_numRingsBroken;
		_score += ++_streak; // increment the streak, and then increment the score
		_onScore.Invoke();
	}
	#endregion
}
