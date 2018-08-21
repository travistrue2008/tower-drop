﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using TRUEStudios.Core;
using TRUEStudios.State;

public class GameController : MonoBehaviour {
	#region Constants
	public const string LevelPrefabPath = "Prefabs/Levels";
	#endregion

	#region Fields
	[SerializeField]
	private Transform _levelContainer;
	[SerializeField]
	private ResultsPopup _levelResultPopupPrefab;
	[SerializeField]
	private Popup _completedGamePopupPrefab;
	[SerializeField]
	private IntEvent _onLevelStarted = new IntEvent();
	[SerializeField]
	private IntEvent _onScoreChanged = new IntEvent();
	[SerializeField]
	private FloatEvent _onProgressChanged = new FloatEvent();

	private bool _isPlaying = false;
	private int _level = 1;
	private int _score = 0;
	private int _numRingsCleared = 0;
	private int totalRings = 0;
	private float _progress = 0.0f;
	private Transform _levelInstance;
	private PopupService _popupService;
	#endregion

	#region Properties
	public int Level { get { return _level; } }
	public IntEvent OnLevelStarted { get { return _onLevelStarted; } }
	public IntEvent OnScoreChanged { get { return _onScoreChanged; } }
	public FloatEvent OnProgressChanged { get { return _onProgressChanged; } }

	public int Score {
		set {
			int v = Mathf.Max(0, value);
			if (_score != v) {
				_score = v;
				_onScoreChanged.Invoke(_score);
			}
		}

		get { return _score; }
	}

	public float Progress {
		set {
			float v = Mathf.Clamp01(value);
			if (_progress != v) {
				_progress = v;
				_onProgressChanged.Invoke(_progress);
			}
		}

		get { return _progress; }
	}

	public int NumRingsCleared {
		set {
			_numRingsCleared = value;
			Progress = (float)_numRingsCleared / (float)totalRings;
		}

		get { return _numRingsCleared; }
	}
	#endregion

	#region MonoBehaviour Hooks
	private void Start () {
		_popupService = Services.Get<PopupService>();
		GotoLevel(1);
	}
	#endregion

	#region Actions
	public void GotoLevel (int level) {
		// validate level
		if (level < 1) {
			throw new ArgumentException("level must be > 1");
		}

		// reset the score and level
		Score = 0;
		Progress = 0.0f;
		_level = level;

		LoadLevel(_level);
		_onLevelStarted.Invoke(_level);
	}

	public void GotoNextLevel () {
		// attempt to load the next level
		try {
			GotoLevel(_level + 1);
		} catch (Exception) {
			// load the "Finished Game" popup if the next level prefab couldn't be found
			_popupService.PushPopup<Popup>(_completedGamePopupPrefab);
		}
	}

	public void RestartLevel () {
		GotoLevel(_level);
	}

	public void ClearRing (Ring ring, int streak) {
		Score += streak;
		++NumRingsCleared;
	}

	public void FinishLevel (bool passed) {
		// submit the current score, and instantiate the popup
		bool updatedScore = BestScore.Submit(_score, _level);
		int scoreResult = updatedScore ? _score : 0;
		var popup = _popupService.PushPopup<ResultsPopup>(_levelResultPopupPrefab);
		if (passed) {
			popup.OnClose.AddListener(GotoNextLevel);
		} else {
			popup.OnClose.AddListener(RestartLevel);
		}
		
		popup.Set(passed, _level, scoreResult);
		_isPlaying = false;
	}
	#endregion

	#region Private Methods
	private void LoadLevel (int level) {
		// load the level resources, and make sure it exists
		string prefabPath = $"{LevelPrefabPath}/Level_{level:D3}";
		var prefab = Resources.Load(prefabPath) as GameObject;
		if (prefab == null) {
			throw new NullReferenceException($"Unable to load level: {level}");
		}

		// remove the current level
		if (_levelInstance != null) {
			Destroy(_levelInstance.gameObject);
		}

		// instantiate the level
		var obj = GameObject.Instantiate(prefab);
		_levelInstance = obj.transform;
		_levelInstance.SetParent(_levelContainer);
		_levelInstance.localScale = Vector3.one;
		_levelInstance.Rotate(0.0f, 1.0f, 0.0f); // HACK: the level needs to be transformed, or the ball won't collide
		_levelInstance.Rotate(0.0f, -1.0f, 0.0f); // then move it back

		// get total number of rings
		var rings = _levelInstance.GetComponentsInChildren<Ring>();
		totalRings = rings.Length;
		_isPlaying = true;
	}
	#endregion
}