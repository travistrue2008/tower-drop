using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TRUEStudios.State;
using TRUEStudios.Tweens;
using TMPro;

public class HUD : MonoBehaviour {
	#region Fields
	[SerializeField]
	private RectTransform _progressContainer;
	[SerializeField]
	private RectTransform _streakContainer;
	[SerializeField]
	private RectTransform _fillProgress;
	[SerializeField]
	private TextMeshProUGUI _scoreLabel;
	[SerializeField]
	private TextMeshProUGUI _bestScoreLabel;
	[SerializeField]
	private TextMeshProUGUI _currentLevelLabel;
	[SerializeField]
	private TextMeshProUGUI _nextLevelLabel;
	[SerializeField]
	private TextMeshProUGUI _streamLabelPrefab;

	private int _oldScore = 0;
	private Vector2 _size;
	private LevelService _levelService;
	private AlphaTween _bestScoreTween;
	#endregion

	#region MonoBehaviour Hooks
	private void Awake () {
		_size = new Vector2(_progressContainer.sizeDelta.x, _fillProgress.sizeDelta.y);
		_bestScoreTween = _bestScoreLabel.GetComponent<AlphaTween>();
	}

	private void Start () {
		_levelService = Services.Get<LevelService>();
		Refresh();
	}

	private void OnEnable () {
		var service = Services.Get<LevelService>();
		service.OnLevelStarted.AddListener(Reset);
		service.OnScoreChanged.AddListener(OnScoreChanged);
		service.OnProgressChanged.AddListener(OnProgressChanged);
	}

	private void OnDisable () {
		var levelService = Services.Get<LevelService>();
		if (levelService != null) {
			levelService.OnScoreChanged.RemoveListener(OnScoreChanged);
		}
	}
	#endregion

	#region Event Handlers
	private void OnScoreChanged (int score) {
		Refresh();
	}

	private void OnProgressChanged (float progress) {
		Refresh();
	}
	#endregion

	#region Actions
	public void Reset () {
		_bestScoreTween.ResetToBegin();
		Refresh();
	}

	public void Refresh () {
		RefreshUI();

		// check if the score has increased
		if (_oldScore < _levelService.Score) {
			int streak = _levelService.Score - _oldScore;
			DisplayStreakLabel(streak);

			// check if this is the first time the score has been changed
			if (_oldScore == 0) {
				_bestScoreTween.PlayForward();
			}
		}

		_oldScore = _levelService.Score;
	}
	#endregion

	#region Private Methods
	private void RefreshUI () {
		int bestScore = _levelService.CurrentLevelBestScore;

		_scoreLabel.text = $"{_levelService.Score}";
		_currentLevelLabel.text = $"{_levelService.CurrentLevel}";
		_nextLevelLabel.text = $"{_levelService.CurrentLevel + 1}";
		_bestScoreLabel.text = (bestScore > 0) ? $"Best: {bestScore}" : "";

		// update the progress fill
		_size.x = _progressContainer.sizeDelta.x * _levelService.Progress;
		_fillProgress.sizeDelta = _size;
	}

	private void DisplayStreakLabel (int streak) {
		// instantiate the prefab label
		var obj = GameObject.Instantiate(_streamLabelPrefab);
		obj.transform.SetParent(_streakContainer);
		obj.transform.localScale = Vector3.one;
		var label = obj.GetComponent<TextMeshProUGUI>();
		label.text = $"+{streak}";
		Destroy(obj, 1.0f); // TODO: add object pooling
	}
	#endregion
}
