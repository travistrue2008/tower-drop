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

	private Vector2 _size;
	private AlphaTween _bestScoreTween;
	#endregion

	#region MonoBehaviour Hooks
	private void Awake () {
		_size = new Vector2(_progressContainer.sizeDelta.x, _fillProgress.sizeDelta.y);
		_bestScoreTween = _bestScoreLabel.GetComponent<AlphaTween>();
	}
	#endregion

	#region Actions
	public void SetLevel (int level) {
		int bestScore = BestScore.Get(level);
		bool hasBestScore = (bestScore > 0);

		_currentLevelLabel.text = $"{level}";
		_nextLevelLabel.text = $"{level + 1}";
		_bestScoreLabel.text = hasBestScore ? $"Best: {bestScore}" : "";
		if (hasBestScore) {
			_bestScoreTween.ResetToBegin();
		}
	}

	public void SetStreak (int streak) {
		if (streak > 0) {
			DisplayStreakLabel(streak);
		}
	}

	public void SetScore (int score) {
		_scoreLabel.text = $"{score}";
		if (_bestScoreTween != null && score > 0) {
			_bestScoreTween.PlayForward();
		}
	}

	public void SetProgress (float progress) {
		_size.x = _progressContainer.sizeDelta.x * progress;
		_fillProgress.sizeDelta = _size;
	}
	#endregion

	#region Private Methods
	private void DisplayStreakLabel (int streak) {
		// instantiate the prefab label
		var obj = GameObject.Instantiate(_streamLabelPrefab);
		obj.transform.SetParent(_streakContainer);
		obj.transform.localScale = Vector3.one;

		// setup the label text
		var label = obj.GetComponent<TextMeshProUGUI>();
		label.text = $"+{streak}";
		Destroy(obj, 1.0f); // TODO: add object pooling
	}
	#endregion
}
