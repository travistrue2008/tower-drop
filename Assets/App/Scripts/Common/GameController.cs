using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TRUEStudios.Foundation.Core;
using TRUEStudios.Foundation.Events;
using TRUEStudios.Foundation.UI;
using TRUEStudios.Foundation.Variables;

public class GameController : MonoBehaviour {
	#region Constants
	public const string LevelPrefabPath = "Prefabs/Levels";
	#endregion

	#region Fields
	[SerializeField]
	private Transform _levelContainer;
	[SerializeField]
	private IntReference _levelReference;
	[SerializeField]
	private IntReference _scoreReference;
	[SerializeField]
	private FloatReference _progressReference;
	[SerializeField]
	private IntEvent _onLevelStarted = new IntEvent();
	[SerializeField]
	private UnityEvent _onGameFinished = new UnityEvent();

	private int _numRingsCleared = 0;
	private int _totalRings = 0;
	private Transform _levelInstance;
	#endregion

	#region Properties
	public IntEvent OnLevelStarted { get { return _onLevelStarted; } }
	public UnityEvent OnGameFinished { get { return _onGameFinished; } }

	public int NumRingsCleared {
		set {
			_numRingsCleared = value;
			_progressReference.Value = (float)_numRingsCleared / (float)_totalRings;
		}

		get { return _numRingsCleared; }
	}
	#endregion

	#region MonoBehaviour Hooks
	private void Awake () {
		GotoLevel(_levelReference.Value);
	}
	#endregion

	#region Actions
	public void GotoLevel (int level) {
		// validate level
		if (level < 1) {
			throw new ArgumentException("level must be >= 1");
		}

		// reset the score and level
		_numRingsCleared = 0;
		_scoreReference.Value = 0;
		_levelReference.Value = level;
		_progressReference.Value = 0.0f;

		LoadLevel(_levelReference.Value);
		_onLevelStarted.Invoke(_levelReference.Value);
	}

	public void GotoNextLevel () {
		// attempt to load the next level
		try {
			GotoLevel(_levelReference.Value + 1);
		} catch (Exception) {
			_onGameFinished.Invoke();
		}
	}

	public void RestartLevel () {
		GotoLevel(_levelReference.Value);
	}

	public void FinishLevel (bool passed) {
		if (passed) {
			GotoNextLevel();
		} else {
			RestartLevel();
		}
	}

	public void ClearRing (Ring ring, int streak) {
		_scoreReference.Value += streak;
		++NumRingsCleared;
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
		_totalRings = rings.Length;
	}
	#endregion
}
