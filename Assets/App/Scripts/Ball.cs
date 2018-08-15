using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Ball : MonoBehaviour {
	#region Constants
	public const float MaxDropDistance = 2.0f;
	public const float MaxRotationSpeed = 180.0f;

	public readonly Vector3 bounce = new Vector3(0.0f, 6.0f, 0.0f);
	#endregion

	#region Fields
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
	}

	private void Update () {
		float delta = -_inputController.LeftAxis.x * MaxRotationSpeed * Time.deltaTime;
		_ringsContainer.Rotate(0.0f, delta, 0.0f);

		// check if the camera dropped too far
		if (_camera.transform.position.y - transform.position.y > MaxDropDistance) {
			_cameraPosition.y = transform.position.y + MaxDropDistance;
			_camera.transform.position = _cameraPosition;
		}
	}

	private void OnTriggerEnter (Collider other) {
		// check if a breaker was entered
		if (other.gameObject.tag == "Breaker") {
			// get the Ring component, and break
			var ring = other.transform.parent.GetComponent<Ring>();
			if (ring != null) {
				ring.transform.SetParent(_discardContainer);
				ring.Break();
				++_numRingsBroken;
				_score += ++_streak; // increment the streak, and then increment the score
				_onScore.Invoke();
			} else {
				throw new NullReferenceException("No Ring component found on parent of GameObject tagged as 'Breakder'");
			}
		}
	}

	private void OnCollisionEnter (Collision collision) {
		// check if a hazard ring segment was hit
		if (collision.gameObject.tag == "Hazard") {
			Debug.Log("The player failed the level...");
			_rigidBody.velocity = Vector3.zero;
		} else {
			_rigidBody.velocity = bounce;
		}

		_streak = 0;
	}
	#endregion

	#region Public Methods
	public void Reset () {
		transform.position = _initialPosition;
		_ringsContainer.eulerAngles = Vector3.zero;
		_numRingsBroken = _streak = _score = 0;
	}
	#endregion
}
