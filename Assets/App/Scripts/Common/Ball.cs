using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using TRUEStudios.State;
using TMPro;

public class Ball : MonoBehaviour {
	#region Constants
	public const int SlamThreshold = 4;
	public const float MaxDropDistance = 1.0f;
	public const float MaxRotationSpeed = 180.0f;

	public readonly Vector3 Bounce = new Vector3(0.0f, 4.2f, 0.0f);
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
	private ResultsPopup _failedLevelPopupPrefab;
	[SerializeField]
	private ResultsPopup _completedLevelPopupPrefab;
	[SerializeField]
	private Popup _completedGamePopupPrefab;

	private int _streak = 0;
	private int _numRingsBroken = 0;
	private Vector3 _cameraPosition;
	private InputService _inputService;
	private LevelService _levelService;
	private Rigidbody _rigidBody;
	private Ring[] _rings;
	#endregion

	#region Properties
	public int Streak { get { return _streak; } }

	public int NumRingsBroken {
		set {
			// update the field, and set the progress accordingly
			_numRingsBroken = value;
			if (_levelService != null && _rings != null) {
				_levelService.Progress = (float)_numRingsBroken / (float)_rings.Length;
			}
		}

		get { return _numRingsBroken; }
	}

	public bool IsPlaying {
		set { _isPlaying = value; }
		get { return _isPlaying; }
	}
	#endregion

	#region MonoBehaviour Hooks
	private void Awake () {
		_cameraPosition = _camera.transform.position;

		_rigidBody = GetComponent<Rigidbody>();
		_rings = _ringsContainer.GetComponentsInChildren<Ring>();

		// HACK: the level needs to be transformed, or the ball won't collide
		_ringsContainer.Rotate(0.0f, 1.0f, 0.0f);
		_ringsContainer.Rotate(0.0f, -1.0f, 0.0f);
	}

	private void Start () {
		// get services
		_inputService = Services.Get<InputService>();
		_levelService = Services.Get<LevelService>();
	}

	private void Update () {
		// only control when the user is playing
		if (_isPlaying) {
			float delta = -_inputService.LeftAxis.x * MaxRotationSpeed * Time.deltaTime;
			_ringsContainer.Rotate(0.0f, delta, 0.0f);
		}

		// check if the camera dropped too far
		if (_camera.transform.position.y - transform.position.y > MaxDropDistance) {
			_cameraPosition.y = transform.position.y + MaxDropDistance;
			_camera.transform.position = _cameraPosition;
		}
	}

	private void OnTriggerEnter (Collider other) {
		// check if a burst mesh was entered
		if (other.gameObject.tag == "Burst") {
			var ring = other.transform.parent.GetComponent<Ring>();
			BurstRing(ring, false);
		}
	}

	private void OnCollisionEnter (Collision collision) {
		// check if the finish ring was reached
		if (collision.gameObject.tag == "Finish") {
			FinishLevel();
			return;
		}

		// get the segment
		var segment = collision.transform.GetComponent<Segment>();
		if (segment != null) {
			// check if the player landed on a hazard
			if (segment.IsHazard) {
				FailLevel();
				return;
			}
			
			// check if the player can slam through the ring
			if (_streak >= SlamThreshold) {
				BurstRing(segment.ParentRing, true);
			}

			// don't bounce if not playing anymore
			if (_isPlaying) {
				_rigidBody.velocity = Bounce;
			}

			_streak = 0;
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
	#endregion

	#region Private Methods
	private void BurstRing (Ring ring, bool slam) {
		// get the Ring component, and burst
		ring.transform.SetParent(_discardContainer);
		ring.Burst(slam);
		_levelService.Score += ++_streak; // increment the streak, and then increment the score
		++NumRingsBroken;
	}

	private void FailLevel () {
		Stop();

		bool updatedScore = _levelService.SubmitScore();
		var popup = Services.Get<PopupService>().PushPopup<ResultsPopup>(_failedLevelPopupPrefab);
		if (updatedScore) {
			popup.SetBestScore(_levelService.Score);
		}

		popup.OnClose.AddListener(_levelService.RestartLevel);
	}

	private void FinishLevel () {
		Stop();

		// submit the current score, and instantiate the popup
		bool updatedScore = _levelService.SubmitScore();
		var popup = Services.Get<PopupService>().PushPopup<ResultsPopup>(_completedLevelPopupPrefab);
		popup.SetLevel(_levelService.CurrentLevel);
		if (updatedScore) {
			popup.SetBestScore(_levelService.Score);
		}

		popup.OnClose.AddListener(OnGotoNextLevel);
	}

	private void OnGotoNextLevel () {
		// attempt to load the next level
		try {
			_levelService.GotoNextLevel();
		} catch (Exception) {
			// load the "Finished Game" popup if the next level prefab couldn't be found
			Services.Get<PopupService>().PushPopup<Popup>(_completedGamePopupPrefab);
		}
	}
	#endregion
}
