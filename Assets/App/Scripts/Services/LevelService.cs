using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using TRUEStudios.Core;
using TRUEStudios.State;

public class LevelService : Service {
	#region Constants
	public const string BestScorePrefix = "best_score_";
	public const string LevelPrefabPath = "Prefabs/Levels";
	#endregion

	#region Fields
	[SerializeField]
	private UnityEvent _onLevelStarted = new UnityEvent();
	[SerializeField]
	private IntEvent _onScoreChanged = new IntEvent();
	[SerializeField]
	private FloatEvent _onProgressChanged = new FloatEvent();

	private int _level = 1;
	private int _score = 0;
	private float _progress = 0.0f;
	#endregion

	#region Properties
	public int CurrentLevel { get { return _level; } }
	public int CurrentLevelBestScore { get { return GetBestScore(CurrentLevel); } }
	public UnityEvent OnLevelStarted { get { return _onLevelStarted; } }
	public IntEvent OnScoreChanged { get { return _onScoreChanged; } }
	public FloatEvent OnProgressChanged { get { return _onProgressChanged; } }

	public int Score {
		set {
			int old = _score;
			_score = Mathf.Max(0, value);
			if (old != _score) {
				_onScoreChanged.Invoke(_score);
			}
		}

		get { return _score; }
	}

	public float Progress {
		set {
			float old = _progress;
			_progress = Mathf.Clamp01(value);
			if (old != _progress) {
				_onProgressChanged.Invoke(_progress);
			}
		}

		get { return _progress; }
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

		SetupLevel(level);
		_onLevelStarted.Invoke();
	}

	public void GotoNextLevel () {
		GotoLevel(_level + 1);
	}

	public void RestartLevel () {
		GotoLevel(_level);
	}

	public void ClearData () {
		PlayerPrefs.DeleteAll();
		PlayerPrefs.Save();
	}

	public bool SubmitScore () {
		int currentBest = GetBestScore(_level);
		bool updateScore = (currentBest < _score);
		PlayerPrefs.SetInt($"{BestScorePrefix}/{GetPaddedLevel(_level)}", _score);
		PlayerPrefs.Save();
		return updateScore;
	}

	public int GetBestScore (int level) {
		return PlayerPrefs.GetInt($"{BestScorePrefix}/{GetPaddedLevel(level)}");
	}
	#endregion

	#region Private Methods
	private void SetupLevel (int level) {
		// TODO: provide better resource loading here...
		// load the level resources, and make sure it exists
		string prefabPath = $"{LevelPrefabPath}/Level_{GetPaddedLevel(level)}";
		Debug.Log($"loading prefab: {prefabPath}");
		var prefab = Resources.Load(prefabPath) as GameObject;
		if (prefab == null) {
			throw new NullReferenceException($"Unable to load level: {level}");
		}

		// remove the current level
		var currentLevel = GameObject.FindGameObjectWithTag("GameController");
		if (currentLevel != null) {
			Destroy(currentLevel);
		}

		// instantiate the level
		var obj = GameObject.Instantiate(prefab);
		obj.transform.localScale = Vector3.one;
	}

	private string GetPaddedLevel (int level) {
		return $"{level:D3}";
	}
	#endregion
}
